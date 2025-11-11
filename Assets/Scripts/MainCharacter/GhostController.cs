using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class GhostController : MonoBehaviour
{
    [SerializeField, Tooltip("Ghost Input Actions")]
    private InputActionAsset inputActions;

    private InputAction ghostIA;

    private Animator ghostAnimator;

    [Range(1.0f, 25.0f), Tooltip("Determines the ghost speed")]
    public float MoveSpeed { get; private set; } = 5f;
    [Range(1.0f, 10.0f), Tooltip("Determines the scream range")]
    public float ScreamRange { get; private set; } = 10.0f;
    [SerializeField, Tooltip("Visual effect for the scream action")]
    private GameObject ScreamEffect;

    [SerializeField] 
    private HUDController hud;
    [SerializeField] 
    private WorldManager worldManager;
    [SerializeField] 
    private Camera ghostCamera;

    public int Score { get; private set; } = 0;
    public int ComboScore { get; private set; } = 0;
    public int MaxCombo { get; private set; } = 0;
    public float ComboTimer { get; private set; } = 0.0f;
    public float Stamina { get; private set; } = 100.0f;
    public bool IsScreaming { get; private set; } = false;
    public bool IsFinished { get; private set; } = false;
    public bool HasBegan { get; private set; } = false;

    private Coroutine reorientScreamCoroutine;

    private void OnEnable()
    {
        inputActions.FindActionMap("GhostIA").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("GhostIA").Disable();
    }

    private void Awake()
    {
        ghostIA = inputActions.FindActionMap("GhostIA").FindAction("Scream");
        ghostIA.performed += OnScream;
    }

    private void OnDestroy()
    {
        ghostIA.performed -= OnScream;
    }

    void Start()
    {
        ghostAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!HasBegan) return;

        if (ghostCamera.transform.position.z < 1636.75f)
        {
            ghostCamera.transform.position += Mathf.Clamp(MoveSpeed + ComboScore, 1.0f, 15.0f) * Time.deltaTime * Vector3.forward;
        }
        else if (!IsFinished)
        {
            IsFinished = true;
            // Bonus for finishing the level plus combo bonus and according the gametimer
            Score += 2000 + (MaxCombo * 15) - (int)(worldManager.GameTimer) * 20 + (ComboScore * 5);
            hud.SetTargetScore(Score);
            StopAllCoroutines();
            ghostAnimator.Play("Celebration");
            transform.localRotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
            hud.SetFinalText(MaxCombo);
        }
        if (ComboTimer > 0.0f)
        {
            ComboTimer -= Time.deltaTime;
        }
        else if (ComboTimer <= 0.0f && ComboScore > 0)
        {
            ComboScore = 0; // Reset combo if timer runs out
            hud.SetComboScore(ComboScore);
        }
        // Regenerate stamina over time
        if (Stamina < 100.0f && !IsScreaming)
        {
            Stamina = Mathf.Min(100.0f, Stamina + 25.0f * Time.deltaTime);
            hud.SetStamina(Stamina, 100.0f);
        }
    }

    void LateUpdate()
    {
        if (IsScreaming)
        {
            AnimatorStateInfo stateInfo = ghostAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("idle"))
            {
                IsScreaming = false;
            }
        }
    }

    public void OnScream(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!HasBegan)
            {
                HasBegan = true;
                hud.HideIntroText();
            }
            else if (!IsScreaming && Stamina >= 10.0f)
                Scream();
        }
    }

    void Scream()
    {
        IsScreaming = true;
        // VFX for scream
        if (ScreamEffect != null)
        {
            Instantiate(ScreamEffect, transform.position, Quaternion.identity);
        }
        ghostAnimator.Play("attack_shift", -1, 0f);
        // Decrease stamina
        Stamina = Mathf.Max(0.0f, Stamina - 10.0f);
        hud.SetStamina(Stamina, 100.0f);

        // Detect humans in range and scare them
        List<HumanBehavior> hitsHumans = hitsHumans = Physics.OverlapSphere(transform.position, ScreamRange).Select(collider => collider.GetComponent<HumanBehavior>()).Where(human => human != null).Distinct().ToList();
        List<Transform> scaredHumans = new();
        foreach (var hit in hitsHumans)
        {
            hit.GetComponent<HumanBehavior>()?.Scared();
            scaredHumans.Add(hit.transform);
            ComboScore += 1;  // Increment combo count
            Score += 10 * hitsHumans.Count; // Increment points multiplied by combo score for each scared human
            if (ComboScore > MaxCombo)
            {
                MaxCombo = ComboScore; // Update max combo if current combo exceeds it
            }
            ComboTimer = 7.5f; // Reset combo timer
            hud.SetTargetScore(Score);
            hud.SetComboScore(ComboScore);
        }
        if (scaredHumans.Count > 0)
        {
            Vector3 averagePosition = Vector3.zero;
            foreach (Transform human in scaredHumans)
            {
                averagePosition += human.position;
            }
            averagePosition /= scaredHumans.Count;

            if (reorientScreamCoroutine != null)
            {
                StopCoroutine(reorientScreamCoroutine);
            }
            reorientScreamCoroutine = StartCoroutine(RotateTowardsHumansAndReorient(averagePosition));
        }
    }

    private IEnumerator RotateTowardsHumansAndReorient(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        float elapsed = 0f;
        float duration = 0.33f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsed / duration);
            yield return null;
        }

        // Hold the rotation for a brief moment with a lerp based on combo score and move speed
        yield return new WaitForSeconds(Mathf.Lerp(0.05f, 0.25f, 1f - (ComboScore / (ComboScore + MoveSpeed))));

        Quaternion forwardRotation = Quaternion.LookRotation(Vector3.forward);
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation, elapsed / duration);
            yield return null;
        }
    }
}
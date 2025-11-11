using UnityEngine;

public class CurveManager : MonoBehaviour
{
    [SerializeField, Range(-1.0f, 1.0f), Tooltip("Bend of the curve on the X axis")]
    private float bendX = 0.0f;
    [SerializeField, Range(-1.0f, 1.0f), Tooltip("Bend of the curve on the Y axis")]
    private float bendY = 0.0f;
    [SerializeField, Range(-1.0f, 1.0f), Tooltip("Bend of the curve on the Z axis")]
    private float bendZ = 0.0f;
    [SerializeField, Tooltip("Materials to apply the curve effect")]
    private Material[] materials;

    [SerializeField]
    private bool activateRandomBend = true;

    private void Start()
    {
        float randomSeedX = Random.Range(-0.25f, 0.25f);
        float randomSeedY = Random.Range(0.1f, 0.3f);
        float randomSeedZ = Random.Range(-0.3f, -0.1f);
        foreach (var mat in materials)
        {
            mat.SetFloat(Shader.PropertyToID("X_Axis"), (activateRandomBend ? randomSeedX : bendX) / 100.0f);
            mat.SetFloat(Shader.PropertyToID("Y_Axis"), (activateRandomBend ? randomSeedY : bendY) / 100.0f);
            mat.SetFloat(Shader.PropertyToID("Z_Axis"), (activateRandomBend ? randomSeedZ : bendZ) / 100.0f);
        }
    }

    void LateUpdate()
    {
        /*foreach (var mat in materials)
        {
            mat.SetFloat(Shader.PropertyToID("X_Axis"), bendX / 100.0f);
            mat.SetFloat(Shader.PropertyToID("Y_Axis"), bendY / 100.0f);
            mat.SetFloat(Shader.PropertyToID("Z_Axis"), bendZ / 100.0f);
        }*/
    }
}
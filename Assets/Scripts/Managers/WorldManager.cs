using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject mapPrefab;
    [SerializeField] 
    private float chunkLength = 50f;
    [SerializeField] 
    private int chunkCount = 2;

    private int chunkIndex = 0;

    private List<Transform> chunks;

    public float GameTimer { get; private set; } = 0.0f;

    [SerializeField]
    private GhostController ghost;

    [SerializeField]
    private HUDController hud;

    void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        chunks = new List<Transform>();

        for (int i = 0; i < chunkCount; i++)
        {
            GameObject chunk = Instantiate(mapPrefab, new Vector3(0, 0, i * chunkLength), Quaternion.identity, this.transform);
            chunk.transform.localPosition = new Vector3(0, 0, i * chunkLength);
            chunks.Add(chunk.transform);
        }
    }

    void Update()
    {
        if (!ghost.HasBegan) return;

        UpdateTimer();

        if (chunkIndex < 8)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                Transform chunk = chunks[i];

                if (ghost.transform.position.z - chunk.position.z > chunkLength * 0.7f)
                {
                    float maxZ = GetFarthestChunkZ();
                    chunk.localPosition = new Vector3(0, 0, maxZ + chunkLength);
                    chunk?.GetComponent<MapImportantGO>()?.RespawnPeasants();
                    chunkIndex++;
                }
            }
        }
    }

    void UpdateTimer()
    {
        if (ghost)
        {
            if (!ghost.IsFinished)
            {
                GameTimer += Time.deltaTime;
                hud.SetGameTimer(GameTimer);
            }
        }
    }

    private float GetFarthestChunkZ()
    {
        float maxZ = float.MinValue;
        foreach (Transform t in chunks)
        {
            if (t.position.z > maxZ)
                maxZ = t.position.z;
        }
        return maxZ;
    }
}
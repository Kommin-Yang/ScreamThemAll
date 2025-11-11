using UnityEngine;

static class MemoryCleaner
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ClearOnPlay()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
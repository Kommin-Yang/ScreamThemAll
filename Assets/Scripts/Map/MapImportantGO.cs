using System.Collections.Generic;
using UnityEngine;

public class MapImportantGO : MonoBehaviour
{
    List<HumanBehavior> humans;

    void Start()
    {
        humans = new List<HumanBehavior>(GetComponentsInChildren<HumanBehavior>(true));
    }
    
    public void RespawnPeasants()
    {
        foreach (var human in humans)
        {
            human.ResetPeasant();
        }
    }
}
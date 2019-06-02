using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    [SerializeField]
    public GameObject ground;

    [SerializeField]
    private GameObject groundBack;
    
    [SerializeField]
    private GameObject[] trees;

    [SerializeField]
    private double[] treesDistribution;

    public double[] getTreesDistribution()
    {
        return treesDistribution;
    }

    public GameObject[] getTrees()
    {
        return trees;
    }

    public GameObject getGroundBack()
    {
        return groundBack;
    }
}
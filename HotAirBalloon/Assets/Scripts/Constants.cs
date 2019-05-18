using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    [SerializeField]
    private double factorSize;
    [SerializeField]
    private double timeFactor;
    [SerializeField]
    private double timeFactorMap;

    public double GetFactorSize()
    {
        return factorSize;
    }
    
    public double GetTimeFactor()
    {
        return timeFactor;
    }

    public double GetTimeFactorMap()
    {
        return timeFactorMap;
    }
}

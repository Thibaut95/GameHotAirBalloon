

using System;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    [SerializeField]
    private float realWidth;
    [SerializeField]
    private float realHeight;


    public float GetRealWidth()
    {
        return realWidth;
    }

    public float GetRealHeight()
    {
        return realHeight;
    }
}

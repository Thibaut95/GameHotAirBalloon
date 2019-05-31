

using System;
using UnityEngine;
using UnityEngine.UI;

public class MapInfo : MonoBehaviour
{
    [SerializeField]
    private float realWidth;
    [SerializeField]
    private float realHeight;
    [SerializeField]
    private float latitudeRef;
    [SerializeField]
    private float longitudeRef;

    

    public float GetRealWidth()
    {
        return realWidth;
    }

    public float GetRealHeight()
    {
        return realHeight;
    }

    public void UpdateStartAndTarget(RacePositions racePositions)
    {
        float width = GetComponent<RectTransform>().rect.width;
        float height = GetComponent<RectTransform>().rect.height;

        float widthRatio =  width / realWidth;
        float heightRatio =  height / realHeight;

        float latitudeDistance = 111.11f;


        float longitudeDistanceStart = latitudeDistance * (float)Math.Cos((Math.PI/180)*racePositions.latitudeStart);
        float longitudeDistanceTarget = latitudeDistance * (float)Math.Cos((Math.PI/180)*racePositions.latitudeTarget);

        float deltaLatitudeStart = racePositions.latitudeStart - latitudeRef;
        float deltaLongitudeStart = racePositions.longitudeStart - longitudeRef;

        float deltaLatitudeTarget = racePositions.latitudeTarget - latitudeRef;
        float deltaLongitudeTarget = racePositions.longitudeTarget - longitudeRef;

        Vector3 posStart = transform.Find("start").localPosition;
        Vector3 posTarget = transform.Find("target").localPosition;

        posStart.x = longitudeDistanceStart * deltaLongitudeStart * 1000 * widthRatio - width/2;
        posStart.y = latitudeDistance * deltaLatitudeStart * 1000 * heightRatio - height/2;

        posTarget.x = longitudeDistanceTarget * deltaLongitudeTarget * 1000 * widthRatio - width/2;
        posTarget.y = latitudeDistance * deltaLatitudeTarget * 1000 * heightRatio - height/2;

        transform.Find("start").localPosition=posStart;
        transform.Find("target").localPosition=posTarget;

        transform.Find("Canvas").Find("Text").GetComponent<Text>().text = racePositions.name;
    }
}

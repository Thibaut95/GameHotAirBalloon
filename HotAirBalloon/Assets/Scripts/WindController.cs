using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WindController : MonoBehaviour
{
    private const int maxHeight = 10000;

    [SerializeField]
    private int sizeArea;
    [SerializeField]
    private WindManager windManager;
    [SerializeField]
    private GameObject constantsGameObject;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject needle;
    [SerializeField]
    private GameObject miniBalloonPrefab;
    [SerializeField]
    private GameObject mapLocation;


    private Constants constants;
    private BalloonControler balloonControler;
    private MapInfo mapInfo;
    private GameObject miniBalloon;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 balloonPosition;
    private float widthRatio;
    private float heightRatio;
    private float width;
    private float height;
    

    // Start is called before the first frame update
    void Start()
    {
        constants = constantsGameObject.GetComponent<Constants>();
        balloonControler = this.GetComponent<BalloonControler>();

        int sizeTable = maxHeight / sizeArea;
        windManager.InitWinds(sizeTable);

        GameObject map = Instantiate(Resources.Load("Map"+StaticClass.CrossSceneInformation), canvas.transform) as GameObject;
        map.transform.position = mapLocation.transform.position;

        miniBalloon = Instantiate(miniBalloonPrefab,map.transform);

        mapInfo = map.GetComponent<MapInfo>();
        
        width = map.GetComponent<RectTransform>().rect.width;
        height = map.GetComponent<RectTransform>().rect.height;

        

        widthRatio =  width / mapInfo.GetRealWidth();
        heightRatio =  height / mapInfo.GetRealHeight();

        Transform target = map.transform.Find("target");
        Transform start = map.transform.Find("start");

        startPosition = new Vector2((start.localPosition.x+width/2)/widthRatio,(start.localPosition.y+height/2)/heightRatio);
        balloonPosition = new Vector2((start.localPosition.x+width/2)/widthRatio,(start.localPosition.y+height/2)/heightRatio);
        targetPosition = new Vector2((target.localPosition.x+width/2)/widthRatio,(target.localPosition.y+height/2)/heightRatio);

        // balloonPosition = new Vector2(0,0);
        Debug.Log(startPosition);

        updateBalloonOnMap();
    }

    private void updateBalloonOnMap()
    {
        Vector3 newPos = new Vector3(balloonPosition.x*widthRatio-width/2,balloonPosition.y*heightRatio-height/2,-0.1f);
        miniBalloon.transform.localPosition = newPos;
    }

    private int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        return (int)Math.Sqrt(Math.Pow(pos1.x-pos2.x,2)+Math.Pow(pos1.y-pos2.y,2));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int index = (int)Math.Floor(transform.position.y / constants.GetFactorSize() / sizeArea);

        float strength = windManager.GetStrength(index);
        float direction = windManager.GetDirection(index);

        Vector3 position = this.transform.position;
        if (position.y / constants.GetFactorSize() > balloonControler.Geth0())
        {
            float realDistance = strength * Time.deltaTime;
            balloonPosition.x += realDistance * (float)constants.GetTimeFactorMap() * (float)Math.Cos((Math.PI/180)*direction);
            balloonPosition.y += realDistance * (float)constants.GetTimeFactorMap() * (float)Math.Sin((Math.PI/180)*direction);
            Debug.Log(balloonPosition);
            updateBalloonOnMap();
            position.x += realDistance * (float)constants.GetFactorSize() * (float)constants.GetTimeFactor();
        }
        this.transform.position = position;

        canvas.transform.Find("TextVWind").GetComponent<Text>().text = string.Format("{0:0.00}", strength) + " M/S";
        canvas.transform.Find("TextDest").GetComponent<Text>().text = string.Format("{0:0}", GetDistance(balloonPosition,targetPosition)) + " M";
        canvas.transform.Find("TextStart").GetComponent<Text>().text = string.Format("{0:0}", GetDistance(balloonPosition,startPosition)) + " M";
        needle.transform.eulerAngles = new Vector3(0, 0, direction-90);
    }

    public int DistanceToTarget()
    {
        return GetDistance(balloonPosition,targetPosition);
    }

    public int DistanceToStart()
    {
        return GetDistance(balloonPosition,startPosition);
    }

    public int DistanceStartToTarget()
    {
        return GetDistance(startPosition,targetPosition);
    }

    public float GetMaxStrength()
    {
        return windManager.GetMaxStrength()*(float)constants.GetTimeFactorMap();
    }
}

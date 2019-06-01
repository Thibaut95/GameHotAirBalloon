using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class WindController : MonoBehaviour
{
    [SerializeField]
    private int sizeArea;
    [SerializeField]
    private int sizeListTime;
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
    [SerializeField]
    private GameObject canvasWind;
    [SerializeField]
    private GameObject cellPrefabs;
    [SerializeField]
    private GameObject textLabelPrefabs;


    private Constants constants;
    private BalloonController balloonControler;
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
        balloonControler = this.GetComponent<BalloonController>();

        int sizeTable = constants.maxHeight / sizeArea;
        Debug.Log(constants.maxHeight);
        Debug.Log(sizeArea);
        Debug.Log(sizeTable);
        windManager.InitWinds(sizeListTime, sizeTable);
        UpdateCanvasWind();

        canvas.transform.Find("ForwardAccelerationIndicator").Find("TextMapAcceleration").GetComponent<Text>().text = "X" + ((int)constants.GetTimeFactorMap()).ToString();

        GameObject map = Instantiate(Resources.Load("Map"), canvas.transform) as GameObject;
        map.transform.position = mapLocation.transform.position;
        map.GetComponent<MapInfo>().UpdateStartAndTarget(StaticClass.racePositions);

        miniBalloon = Instantiate(miniBalloonPrefab, map.transform);

        mapInfo = map.GetComponent<MapInfo>();

        width = map.GetComponent<RectTransform>().rect.width;
        height = map.GetComponent<RectTransform>().rect.height;



        widthRatio = width / mapInfo.GetRealWidth();
        heightRatio = height / mapInfo.GetRealHeight();

        Transform target = map.transform.Find("target");
        Transform start = map.transform.Find("start");

        startPosition = new Vector2((start.localPosition.x + width / 2) / widthRatio, (start.localPosition.y + height / 2) / heightRatio);
        balloonPosition = new Vector2((start.localPosition.x + width / 2) / widthRatio, (start.localPosition.y + height / 2) / heightRatio);
        targetPosition = new Vector2((target.localPosition.x + width / 2) / widthRatio, (target.localPosition.y + height / 2) / heightRatio);

        

        updateBalloonOnMap();
    }

    private void updateBalloonOnMap()
    {
        Vector3 newPos = new Vector3(balloonPosition.x * widthRatio - width / 2, balloonPosition.y * heightRatio - height / 2, -0.1f);
        miniBalloon.transform.localPosition = newPos;
    }

    private int GetDistance(Vector2 pos1, Vector2 pos2)
    {
        return (int)Math.Sqrt(Math.Pow(pos1.x - pos2.x, 2) + Math.Pow(pos1.y - pos2.y, 2));
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
            balloonPosition.x += realDistance * (float)constants.GetTimeFactorMap() * (float)Math.Cos((Math.PI / 180) * direction);
            balloonPosition.y += realDistance * (float)constants.GetTimeFactorMap() * (float)Math.Sin((Math.PI / 180) * direction);

            updateBalloonOnMap();
            //position.x += realDistance * (float)constants.GetFactorSize() * (float)constants.GetTimeFactor();
            position.x += 0.1f * (float)constants.GetFactorSize() * (float)constants.GetTimeFactor();
        }
        this.transform.position = position;

        canvas.transform.Find("TextVWind").GetComponent<Text>().text = string.Format("{0:0.00}", strength) + " M/S";
        canvas.transform.Find("TextDest").GetComponent<Text>().text = string.Format("{0:0}", GetDistance(balloonPosition, targetPosition)) + " M";
        canvas.transform.Find("TextStart").GetComponent<Text>().text = string.Format("{0:0}", GetDistance(balloonPosition, startPosition)) + " M";
        needle.transform.eulerAngles = new Vector3(0, 0, direction - 90);
    }

    void Update()
    {
        GameObject content = canvasWind.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        GameObject contentLabelTime = canvasWind.transform.Find("Scroll View Time").Find("Viewport").Find("Content").gameObject;
        GameObject contentLabelHeight = canvasWind.transform.Find("Scroll View Height").Find("Viewport").Find("Content").gameObject;
        RectTransform rect = content.GetComponent<RectTransform>();
        RectTransform rectlabelTime = contentLabelTime.GetComponent<RectTransform>();
        RectTransform rectlabelHeight = contentLabelHeight.GetComponent<RectTransform>();
        rectlabelTime.offsetMin = new Vector2(rect.offsetMin.x, rectlabelTime.offsetMin.y);
        rectlabelTime.offsetMax = new Vector2(rect.offsetMax.x, rectlabelTime.offsetMax.y);
        rectlabelHeight.offsetMin = new Vector2(rectlabelHeight.offsetMin.x, rect.offsetMin.y);
        rectlabelHeight.offsetMax = new Vector2(rectlabelHeight.offsetMax.x, rect.offsetMax.y);
    }

    private void UpdateCanvasWind()
    {
        List<Wind[]> listWinds = windManager.GetListWinds();
        Debug.Log(listWinds.Count);

        GameObject content = canvasWind.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        GameObject contentLabelTime = canvasWind.transform.Find("Scroll View Time").Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < contentLabelTime.transform.childCount; i++)
        {
            Destroy(contentLabelTime.transform.GetChild(i).gameObject);
        }

        GameObject contentLabelHeight = canvasWind.transform.Find("Scroll View Height").Find("Viewport").Find("Content").gameObject;
        for (int i = 0; i < contentLabelHeight.transform.childCount; i++)
        {
            Destroy(contentLabelHeight.transform.GetChild(i).gameObject);
        }

        RectTransform rect = content.GetComponent<RectTransform>();
        RectTransform rectCell = cellPrefabs.GetComponent<RectTransform>();
        float offsetX = rect.sizeDelta.x / 2;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(rectCell.rect.width * listWinds.Count + offsetX * 2 + 300, rectCell.rect.height * listWinds[0].Length);
        contentLabelTime.GetComponent<RectTransform>().sizeDelta = new Vector2(rectCell.rect.width * listWinds.Count + offsetX * 2, rectCell.rect.height);
        contentLabelHeight.GetComponent<RectTransform>().sizeDelta = new Vector2(rectCell.rect.width, rectCell.rect.height * listWinds[0].Length);

        Vector3 offset = new Vector3(offsetX + 1100, -50, 0);
        for (int i = 0; i < listWinds.Count; i++)
        {
            for (int j = 0; j < listWinds[0].Length; j++)
            {
                GameObject cell = Instantiate(cellPrefabs, content.transform);
                Vector3 pos = cell.transform.localPosition;
                pos.x = i * rectCell.rect.width;
                pos.y = -(listWinds[0].Length - 1 - j) * rectCell.rect.height;
                pos += offset;
                cell.transform.localPosition = pos;
                cell.transform.Find("Text").GetComponent<Text>().text = string.Format("{0:0.0}", listWinds[i][j].strength);
                cell.transform.Find("compass_needle").eulerAngles = new Vector3(0, 0, listWinds[i][j].direction - 90);
            }
            GameObject labelPrefabs = Instantiate(textLabelPrefabs, contentLabelTime.transform);
            Vector3 posLabel = labelPrefabs.transform.localPosition;
            posLabel.x = i * rectCell.rect.width - offsetX - 1100;
            posLabel.y = 0;
            posLabel += offset;
            labelPrefabs.transform.localPosition = posLabel;
            labelPrefabs.transform.Find("Text").GetComponent<Text>().text = (i * windManager.GetDuration()).ToString();
        }
        for (int j = 0; j < listWinds[0].Length; j++)
        {
            GameObject labelPrefabs = Instantiate(textLabelPrefabs, contentLabelHeight.transform);
            Vector3 posLabel = labelPrefabs.transform.localPosition;
            posLabel.x = 100;
            posLabel.y = -(listWinds[0].Length - 1 - j) * rectCell.rect.height;
            posLabel += offset;
            labelPrefabs.transform.localPosition = posLabel;
            labelPrefabs.transform.Find("Text").GetComponent<Text>().text = (j * sizeArea).ToString();
        }
    }

    public int DistanceToTarget()
    {
        return GetDistance(balloonPosition, targetPosition);
    }

    public int DistanceToStart()
    {
        return GetDistance(balloonPosition, startPosition);
    }

    public int DistanceStartToTarget()
    {
        return GetDistance(startPosition, targetPosition);
    }

    public float GetMaxStrength()
    {
        return windManager.GetMaxStrength() * (float)constants.GetTimeFactorMap();
    }
}

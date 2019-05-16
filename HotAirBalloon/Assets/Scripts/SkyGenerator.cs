using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkyGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera;

    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private int sizeTile;

    [SerializeField]
    private GameObject skyPrefab;

    [SerializeField]
    private GameObject[] cloudPrefabs;

    private int heightMax;
    private int heightMin;
    private int widthMax;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        Vector3 posCamera = mainCamera.transform.position;

        heightMax = (int)(posCamera.y+(height / 2));
        heightMin = (int)(posCamera.y-(height / 2));
        widthMax = (int)(posCamera.x+(width / 2));

        for (int x = (int)(posCamera.x-(width / 2)); x < widthMax; x += sizeTile)
        {
            for (int y = heightMin; y < heightMax; y += sizeTile)
            {
                NewTile(x, y);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mainCamera.transform.position.y + height / 2 > heightMax)
        {
            for (int x = widthMax - width; x < widthMax; x += sizeTile)
            {
                NewTile(x, heightMax);
            }
            heightMax += sizeTile;
            if (mainCamera.transform.position.y - height / 2 - sizeTile * 2 > heightMin) heightMin += sizeTile;
        }
        if (mainCamera.transform.position.y - height / 2 < heightMin)
        {
            for (int x = widthMax - width; x < widthMax; x += sizeTile)
            {
                NewTile(x, heightMin);
            }
            heightMin -= sizeTile;
            if (mainCamera.transform.position.y + height / 2 + sizeTile < heightMax) heightMax -= sizeTile;
        }
        if (mainCamera.transform.position.x + width / 2 > widthMax)
        {
            for (int y = heightMin; y < heightMax; y += sizeTile)
            {
                NewTile(widthMax, y);
            }
            widthMax += sizeTile;
        }

        int size = this.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            GameObject skyTile = this.transform.GetChild(i).gameObject;
            if (skyTile.transform.position.x < widthMax - width || skyTile.transform.position.y < heightMin || skyTile.transform.position.y > heightMax)
            {
                Destroy(skyTile);
            }
        }

    }

    private void NewTile(float x, float y)
    {
        GameObject sky = Instantiate(skyPrefab) as GameObject;
        sky.transform.position = new Vector3(x, y, 0);
        sky.transform.parent = this.transform;

        int nbCloud = random.Next(2);
        for (int i = 0; i < nbCloud; i++)
        {
            float cloudx=(float)random.NextDouble()*sizeTile;
            float cloudy=(float)random.NextDouble()*sizeTile; 
            int index = random.Next(cloudPrefabs.Length);

            GameObject cloud = Instantiate(cloudPrefabs[index]);
            cloud.transform.position = new Vector3(x+cloudx,y+cloudy,-1);
            cloud.transform.parent = sky.transform;
        }
    }
}

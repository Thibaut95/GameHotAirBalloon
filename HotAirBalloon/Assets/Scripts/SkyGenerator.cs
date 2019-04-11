using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SkyGenerator : MonoBehaviour
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    [SerializeField]
    private int sizeTile;

    [SerializeField]
    private GameObject skyPrefab;

    [SerializeField]
    private GameObject skyGameObject;

    private int heightMax;
    private int heightMin;
    private int widthMax;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int x = -width/2; x < width/2; x+=sizeTile)
        {
            for (int y = -height/2; y < height/2; y+=sizeTile)
            {
                GameObject sky = Instantiate(skyPrefab) as GameObject;
                sky.transform.position = new Vector3(x,y,0);
                sky.transform.parent = skyGameObject.transform;
            }
        }

        heightMax = height/2;
        heightMin = -heightMax;
        widthMax = width/2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(this.transform.position.y + height/2 > heightMax)
        { 
            for (int x = widthMax-width; x < widthMax; x+=sizeTile)
            {
                GameObject sky = Instantiate(skyPrefab) as GameObject;
                sky.transform.position = new Vector3(x,heightMax,0);
                sky.transform.parent = skyGameObject.transform;
            }
            heightMax += sizeTile;
            if(this.transform.position.y - height/2 - sizeTile*2 > heightMin) heightMin += sizeTile;
        }
        if(this.transform.position.y - height/2 < heightMin)
        { 
            for (int x = widthMax-width; x < widthMax; x+=sizeTile)
            {
                GameObject sky = Instantiate(skyPrefab) as GameObject;
                sky.transform.position = new Vector3(x,heightMin,0);
                sky.transform.parent = skyGameObject.transform;
            }
            heightMin -= sizeTile;
            if(this.transform.position.y + height/2 + sizeTile < heightMax) heightMax -= sizeTile;
        }
        if(this.transform.position.x + width/2 > widthMax)
        { 
            for (int y = heightMin; y < heightMax; y+=sizeTile)
            {
                GameObject sky = Instantiate(skyPrefab) as GameObject;
                sky.transform.position = new Vector3(widthMax,y,0);
                sky.transform.parent = skyGameObject.transform;
            }
            widthMax += sizeTile;
        }

        int size = skyGameObject.transform.childCount;
        for (int i = 0; i < size; i++)
        {
            GameObject skyTile = skyGameObject.transform.GetChild(i).gameObject;
            if(skyTile.transform.position.x < widthMax-width || skyTile.transform.position.y < heightMin || skyTile.transform.position.y > heightMax)
            {
                Destroy(skyTile);
            }
        }

    }
}

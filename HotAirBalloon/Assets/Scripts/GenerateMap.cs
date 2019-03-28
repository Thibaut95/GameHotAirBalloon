using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MathNet.Numerics;


public class GenerateMap : MonoBehaviour
{
    [SerializeField]
    private GameObject[] landPrefabs;
    [SerializeField]
    private int initNumber;

    [SerializeField]
    private int sizeTile;

    [SerializeField]
    private int sizeCamera;

    [SerializeField]
    private Biome[] biomes;

    private int i;
    private GameObject lastTile;
    private int biome;

    private System.Random random;


    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();

        biome = UnityEngine.Random.Range(0, landPrefabs.Length);
        int startPosition = (initNumber - 1) / 2;
        i = -startPosition;
        while (i <= startPosition)
        {
            lastTile = Instantiate(landPrefabs[biome]) as GameObject;
            lastTile.transform.position += new Vector3(i * sizeTile, 0, 0);
            i++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.x + sizeCamera / 2 > lastTile.transform.position.x)
        {
            lastTile = Instantiate(landPrefabs[biome]) as GameObject;
            lastTile.transform.position += new Vector3(i * sizeTile, 0, 0);

            double number = random.NextDouble();

            double percent = 0.0;
            int nbTree = -1;
            while (number > percent)
            {
                nbTree++;
                percent += biomes[biome].getTreesDistribution()[nbTree];
            }

            Debug.Log("Percent : "+percent);

            List<int> listIndexes = new List<int>();
            for (int k = 0; k < nbTree; k++)
            {
                int index;
                do index = random.Next(0, lastTile.transform.Find("Positions").transform.childCount);
                while (listIndexes.Contains(index));

                listIndexes.Add(index);
            }

            foreach (int index in listIndexes)
            {
                int treeNumber = random.Next(0,biomes[biome].getTrees().Length);
                GameObject tree = Instantiate(biomes[biome].getTrees()[treeNumber]) as GameObject;
                tree.transform.position = lastTile.transform.Find("Positions").transform.GetChild(index).transform.position;
            }

            i++;
        }
    }
}

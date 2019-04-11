using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    [SerializeField]
    private Biome[] biomes;

    [SerializeField]
    private int sizeTile;

    [SerializeField]
    private int widthCamera;

    [SerializeField]
    private GameObject frontDecor;

    [SerializeField]
    private GameObject backDecor;

    [SerializeField]
    private GameObject cloudDecor;

    [SerializeField]
    private GameObject cloud;

    [SerializeField]
    private Vector3 positionNextTile;

    
    private int biome;
    private System.Random random;
    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        biome = Random.Range(0, biomes.Length);

        while (positionNextTile.x < widthCamera/2)
        {
            NewTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.x + widthCamera/2 > positionNextTile.x)
        {
            NewTile();
        }
    }

    public void NewTile()
    {
        GameObject lastTile = Instantiate(biomes[biome].ground) as GameObject;
        lastTile.transform.parent = frontDecor.transform;
        lastTile.transform.position = positionNextTile;
        
        GameObject tileBack = Instantiate(biomes[biome].getGroundBack()) as GameObject;
        tileBack.transform.parent = backDecor.transform;
        tileBack.transform.position = positionNextTile;
        tileBack.layer = backDecor.layer;

        GameObject tileCloud = Instantiate(cloud) as GameObject;
        tileCloud.transform.parent = cloudDecor.transform;
        tileCloud.transform.position = positionNextTile;
        tileCloud.layer = cloudDecor.layer;
        tileCloud.transform.GetChild(0).gameObject.layer = cloudDecor.layer;

        positionNextTile.x += sizeTile;

        double number = random.NextDouble();

        double percent = 0.0;
        int nbTree = -1;
        while (number > percent)
        {
            nbTree++;
            percent += biomes[biome].getTreesDistribution()[nbTree];
        }

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
            int treeNumber = random.Next(0, biomes[biome].getTrees().Length);
            GameObject tree = Instantiate(biomes[biome].getTrees()[treeNumber]) as GameObject;
            tree.transform.parent=frontDecor.transform;
            tree.transform.position = lastTile.transform.Find("Positions").transform.GetChild(index).transform.position + new Vector3(0, 0, -1);
            tree.transform.parent = lastTile.transform;
        }
    }
}

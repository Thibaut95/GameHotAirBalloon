using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstaclesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] obstaclesPrefabs;
    [SerializeField]
    private GameObject balloon;
    [SerializeField]
    private GameObject constantsGameObject;
    [SerializeField]
    private int step;
    [SerializeField]
    private double threshold;
    [SerializeField]
    private Text text;
    [SerializeField]
    private int maxHeight;


    private Constants constants;
    private int currentStep;
    private System.Random random;
    // Start is called before the first frame update
    void Start()
    {
        constants = constantsGameObject.GetComponent<Constants>();
        currentStep = step;
        random = new System.Random();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if((balloon.transform.position.x/constants.GetFactorSize())>currentStep)
        {
            currentStep += step;
            
            for (int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(0).position.x < balloon.transform.position.x - 20)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
            }
            if(random.NextDouble()>threshold)
            {
                GameObject obstacle = Instantiate(obstaclesPrefabs[random.Next(obstaclesPrefabs.Length)], transform);
                Vector3 pos = obstacle.transform.position;
                pos.x = currentStep*(float)constants.GetFactorSize();
                int altitude = (int)((float)random.NextDouble()*maxHeight)+10;
                pos.y = altitude*(float)constants.GetFactorSize();
                obstacle.transform.position = pos;
                string type="";
                switch (obstacle.name)
                {
                    case "CloudObstacle(Clone)":
                        type = "Nuages";
                    break;
                    case "BirdObstacle(Clone)":
                        type = "Oiseaux";
                    break;
                }
                text.text = type + " en approche à l'altitude de " + altitude + "m";
            }
            else
            {
                text.text = "";
            }  
        }
    }
}

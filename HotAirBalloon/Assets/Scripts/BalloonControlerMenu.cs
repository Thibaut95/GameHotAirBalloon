using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonControlerMenu : MonoBehaviour
{
    [SerializeField]
    private float heightMin;
    [SerializeField]
    private float heightMax;

    [SerializeField]
    private float speed;

    private int up = 1;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y += speed * up;
        transform.position = pos;

        if(pos.y > heightMax)
        {
            up = -1;
        }
        else if(pos.y < heightMin)
        {
            up = 1;
        }
    }
}

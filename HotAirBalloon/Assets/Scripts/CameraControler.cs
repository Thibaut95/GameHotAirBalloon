using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private bool isMoving;
    // Start is called before the first frame update
    public void startMove()
    {
        isMoving = true;
    }

    public void stopMove()
    {
        isMoving = false;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    void FixedUpdate()
    {
        if(isMoving)
        {
            transform.Translate(speed,0,0);
        }
    }
}

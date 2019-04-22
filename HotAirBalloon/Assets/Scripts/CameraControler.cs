using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private GameObject balloon;
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private float minHeight;

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
        Vector3 position = transform.position;
        position.x=balloon.transform.position.x+offset.x;
        position.y=balloon.transform.position.y+offset.y;
        if(position.y<minHeight)position.y=minHeight;
        transform.position=position;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate(speed, 0, 0);
        }
    }
}

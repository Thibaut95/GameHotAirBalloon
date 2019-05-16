using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private float offsetYmin;

    [SerializeField]
    private float offsetY;

    [SerializeField]
    private GameObject balloon;

    [SerializeField]
    private float speed;

    [SerializeField]
    private bool isMoving;

    private Vector3 offset;
    private float minHeight;


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
        if(!isMoving)
        {
            minHeight=balloon.transform.position.y+offsetYmin;
            offset = new Vector3(transform.position.x-balloon.transform.position.x,offsetY,0);
            updatePosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            updatePosition();
        }
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.Translate(speed, 0, 0);
        }
    }

    private void updatePosition()
    {
        Vector3 position = transform.position;
        position.x = balloon.transform.position.x + offset.x;
        position.y = balloon.transform.position.y + offset.y;
        if (position.y < minHeight) position.y = minHeight;
        transform.position = position;
    }
}

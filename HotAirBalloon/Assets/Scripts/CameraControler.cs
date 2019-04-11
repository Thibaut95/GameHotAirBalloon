using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControler : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private bool isMoving;

    [SerializeField]
    private FixedJoystick joystick;
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
        if (isMoving)
        {
            transform.Translate(speed, 0, 0);
        }
        else
        {
            float translationY = Input.GetAxis("Vertical") * speed;
            float translationX = Input.GetAxis("Horizontal") * speed;
            if (translationX == 0.0 && translationY == 0.0)
            {
                translationY = joystick.Vertical * speed;
                translationX = joystick.Horizontal * speed;
            }
            transform.Translate(translationX, translationY, 0);
        }



    }
}

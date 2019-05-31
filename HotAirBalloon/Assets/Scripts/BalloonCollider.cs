using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject.Find("CanvasLevel").GetComponent<LevelManager>().FinishGame();
    }
}

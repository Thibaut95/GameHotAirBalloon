using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonControler : MonoBehaviour
{

    private const float maxAngle = 270;
    private const float offsetAngle = 135;
    
    [SerializeField]
    private double h0;
    [SerializeField]
    private double v0;
    [SerializeField]
    private double Ti0;
    [SerializeField]
    private float fuelTotal;
    [SerializeField]
    private float fuelConsumption;
    [SerializeField]
    private GameObject constantsGameObject;
    

    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject needle;


    private PhysicEngine physicEngine;
    private float currentFuel;
    private bool activateBurner;
    private bool activateCooler;
    private Constants constants;


    // Start is called before the first frame update
    void Start()
    {
        constants = constantsGameObject.GetComponent<Constants>();

        physicEngine = new PhysicEngine(h0, v0, Ti0);
        activateBurner = activateCooler = false;
        currentFuel = fuelTotal;
        needle.transform.eulerAngles = new Vector3(0, 0, -offsetAngle);

        Vector3 pos = transform.position;
        pos.y = (float)(h0 * constants.GetFactorSize());
        transform.position = pos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentFuel == 0) activateBurner = false;
        physicEngine.UpdateEngine(Time.deltaTime * constants.GetTimeFactor(), activateBurner, activateCooler);
        double h = physicEngine.Geth();
        // h = 50;

        Vector3 pos = transform.position;
        pos.y = (float)(h * constants.GetFactorSize());
        transform.position = pos;

        UpdateNeedle();

        canvas.transform.Find("TextTi").GetComponent<Text>().text = string.Format("{0:0.0}", physicEngine.GetTi()) + "°C";
        canvas.transform.Find("TextTo").GetComponent<Text>().text = string.Format("{0:0.0}", physicEngine.GetTo()) + "°C";
        canvas.transform.Find("TextH").GetComponent<Text>().text = string.Format("{0:0}", h) + " M";
        canvas.transform.Find("TextVVert").GetComponent<Text>().text = string.Format("{0:0.0}", physicEngine.Getv()) + " M/S";
        
    }

    private void UpdateNeedle()
    {
        if (activateBurner)
        {
            currentFuel -= Time.deltaTime * fuelConsumption;
            if (currentFuel < 0) currentFuel = 0;
            needle.transform.eulerAngles = new Vector3(0, 0, -((currentFuel / fuelTotal) * maxAngle - offsetAngle));
        }
    }

    public double Geth0()
    {
        return h0;
    }

    public double Geth()
    {
        return physicEngine.Geth();
    }

    public float GetCurrentFuel()
    {
        return currentFuel;
    }

    public void EnableBurner()
    {
        activateBurner = true;
    }

    public void DisableBurner()
    {
        activateBurner = false;
    }

    public void EnableCooler()
    {
        activateCooler = true;
    }

    public void DisableCooler()
    {
        activateCooler = false;
    }
}

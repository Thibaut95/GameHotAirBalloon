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
    private double factorSize;
    [SerializeField]
    private double timeFactor;
    [SerializeField]
    private GameObject canvas;
    [SerializeField]
    private GameObject needle;
    [SerializeField]
    private GameObject compasNeedle;
    [SerializeField]
    private GameObject map;
    [SerializeField]
    private float minHeight;
    [SerializeField]
    private float maxHeight;
    [SerializeField]
    private float stepWindArea;
    [SerializeField]
    private float minTimeWind;
    [SerializeField]
    private float maxTimeWind;
    [SerializeField]
    private float minStrengthWind;
    [SerializeField]
    private float maxStrengthWind;
    [SerializeField]
    private bool activateWind;


    private PhysicEngine physicEngine;
    private float currentFuel;
    private bool activateBurner;
    private bool activateCooler;
    private Wind[] windTable;
    private MinMax timeWind;
    private MinMax strengthWind;
    private MinMax angleWind;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        physicEngine = new PhysicEngine(h0, v0, Ti0);
        activateBurner = activateCooler = false;
        currentFuel = fuelTotal;
        needle.transform.eulerAngles = new Vector3(0, 0, -offsetAngle);

        timeWind = new MinMax(minTimeWind, maxTimeWind, 1);
        strengthWind = new MinMax(minStrengthWind, maxStrengthWind, 1);
        angleWind = new MinMax(0, 360, 1);

        random = new System.Random();
        int sizeTable = (int)((maxHeight - minHeight) / stepWindArea);
        windTable = new Wind[sizeTable];
        for (int i = 0; i < windTable.Length; i++)
        {
            windTable[i] = new Wind(strengthWind.GetRand(), angleWind.GetRand(), Time.time + timeWind.GetRand());
        }
    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentFuel == 0) activateBurner = false;
        physicEngine.UpdateEngine(Time.deltaTime * timeFactor, activateBurner, activateCooler);
        double h = physicEngine.Geth();
        Vector3 pos = transform.position;

        pos.y = (float)(h * factorSize);

        float strength = 0;
        Wind wind = windTable[(int)Math.Floor(pos.y / stepWindArea)];
        if (wind.endTime < Time.time)
        {
            wind.direction = angleWind.GetRand();
            wind.strength = strengthWind.GetRand();
            wind.endTime = timeWind.GetRand() + Time.time;
        }
        if (pos.y > 0 && activateWind)
        {
            pos.x += wind.strength * Time.deltaTime * (float)factorSize;
            strength = wind.strength;
        }

        compasNeedle.transform.eulerAngles = new Vector3(0,0,wind.direction);
        
        transform.position = pos;

        UpdateNeedle();

        canvas.transform.Find("TextTi").GetComponent<Text>().text = string.Format("{0:0.0}", physicEngine.GetTi()) + "°C";
        canvas.transform.Find("TextTo").GetComponent<Text>().text = string.Format("{0:0.0}", physicEngine.GetTo()) + "°C";
        canvas.transform.Find("TextH").GetComponent<Text>().text = string.Format("{0:0}", h) + " M";
        canvas.transform.Find("TextVVert").GetComponent<Text>().text = string.Format("{0:0}", physicEngine.Getv()) + " M/S";
        canvas.transform.Find("TextVWind").GetComponent<Text>().text = string.Format("{0:0}", strength) + " M/S";
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

    void OnMouseUp()
    {


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

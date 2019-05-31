using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WindManager
{
    [SerializeField]
    private float durationWind;
    [SerializeField]
    private float maxStrengthWind;
    [SerializeField]
    private float maxDirectionWind;

    private MinMax strengthWind;
    private MinMax directionWind;
    // private Wind[] winds;
    private List<Wind[]> listWinds;
    private float timeToChange;

    private int sizeHeight;

    public void InitWinds(int sizeTime, int sizeHeight)
    {
        this.sizeHeight = sizeHeight;
        timeToChange = Time.time + durationWind;
        strengthWind = new MinMax(0, maxStrengthWind, 0.1f);
        directionWind = new MinMax(-maxDirectionWind, maxDirectionWind, 1);

        listWinds = new List<Wind[]>();
        for (int i = 0; i < sizeTime; i++)
        {
            Wind[] winds = new Wind[sizeHeight];
            listWinds.Add(winds);
            for (int j = 0; j < sizeHeight; j++)
            {
                winds[j] = CreateNewWind(i, j);
            }
        }
    }

    private Wind CreateNewWind(int indexTime, int indexHeight)
    {
        Wind newWind=null;
        if (indexHeight > 0)
        {
            Wind wind = listWinds[indexTime][indexHeight-1];
            newWind = new Wind(strengthWind.GetRand(), wind.direction + directionWind.GetRand());
        }
        else if(indexTime > 0 && indexHeight==0)
        {
            Wind wind = listWinds[indexTime-1][0];
            newWind = new Wind(strengthWind.GetRand(), wind.direction + directionWind.GetRand());
        }
        else if (indexTime == 0 && indexHeight == 0)
        {
            newWind = new Wind(strengthWind.GetRand(), directionWind.GetRand());
        }
        return newWind;
    }

    public float GetDirection(int index)
    {
        CheckTime();
        Wind wind = listWinds[0][index];
        return wind.direction;
    }

    public float GetStrength(int index)
    {
        CheckTime();
        Wind wind = listWinds[0][index];
        return wind.strength;
    }

    public void CheckTime()
    {
        if (timeToChange < Time.time)
        {
            Wind[] winds = new Wind[sizeHeight];
            listWinds.Add(winds);
            for (int j = 0; j < sizeHeight; j++)
            {
                winds[j] = CreateNewWind(listWinds.Count-1, j);
            }
            listWinds.RemoveAt(0);
            timeToChange = Time.time + durationWind;
        }
    }

    public float GetMaxStrength()
    {
        return maxStrengthWind;
    }

    public float GetDuration()
    {
        return durationWind;
    }

    public List<Wind[]> GetListWinds()
    {
        return listWinds;
    }
}

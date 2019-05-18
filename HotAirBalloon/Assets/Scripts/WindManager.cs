using UnityEngine;

[System.Serializable]
public class WindManager
{
    [SerializeField]
    private float minTimeWind;
    [SerializeField]
    private float maxTimeWind;
    [SerializeField]
    private float maxStrengthWind;
    [SerializeField]
    private float maxDirectionWind;

    private MinMax timeWind;
    private MinMax strengthWind;
    private MinMax directionWind;
    private Wind[] winds;

    public void InitWinds(int size)
    {
        timeWind=new MinMax(minTimeWind,maxTimeWind,1);
        strengthWind=new MinMax(0,maxStrengthWind,0.1f);
        directionWind=new MinMax(-maxDirectionWind,maxDirectionWind,1);

        winds = new Wind[size];
        for (int i = 0; i < size; i++)
        {
            winds[i]=CreateNewWind(i);
        }
    }

    private Wind CreateNewWind(int index)
    {
        Wind newWind;
        if(index>0)
        {
            Wind wind = winds[index-1];
            newWind = new Wind(strengthWind.GetRand(),wind.direction+directionWind.GetRand(),timeWind.GetRand());
        }
        else
        {
            newWind = new Wind(strengthWind.GetRand(),directionWind.GetRand(),Time.time + timeWind.GetRand());
        }
        return newWind;
    }

    public float GetDirection(int index)
    {
        Wind wind = winds[index];
        if(wind.endTime < Time.time)
        {
            wind = CreateNewWind(index);
            winds[index]=wind;
        }
        return wind.direction;
    }

    public float GetStrength(int index)
    {
        Wind wind = winds[index];
        if(wind.endTime < Time.time)
        {
            wind = CreateNewWind(index);
            winds[index]=wind;
        }
        return wind.strength;
    }

    public float GetMaxStrength()
    {
        return maxStrengthWind;
    }
}



using System;

public class MinMax
{
    private float min;
    private float max;
    private float step;

    private System.Random random;

    public MinMax(float min, float max, float step)
    {
        this.min=min;
        this.max=max;
        this.step=step;
        random = new System.Random();
    }

    public float GetRand()
    {
        float value = (float)random.NextDouble()*(max-min)+min;
        return (float)Math.Floor(value / step)*step;
    }

    public float GetMin()
    {
        return min;
    }

    public float GetMax()
    {
        return max;
    }
}

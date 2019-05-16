using System;
public class Score
{
    public float fuel;
    public int distance;
    public float time;
    public float global;
    public DateTime date;
    public Score(float fuel, int distance, float time, float global)
    {
        this.fuel = fuel;
        this.distance = distance;
        this.time = time;
        this.global = global;
        this.date = DateTime.Now;
    }
}

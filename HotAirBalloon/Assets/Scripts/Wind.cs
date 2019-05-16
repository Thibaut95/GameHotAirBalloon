public class Wind
{
    public float strength { get; set; }
    public float direction { get; set; }
    public float endTime { get; set; }
    public Wind(float strength, float direction, float endTime)
    {
        this.strength=strength;
        this.direction=direction;
        this.endTime=endTime;
    }
}

public class RacePositions
{
    public string name;
    public float latitudeStart;
    public float longitudeStart;
    public float latitudeTarget;
    public float longitudeTarget;

    public RacePositions(float latitudeStart, float latitudeTarget, float longitudeStart, float longitudeTarget, string name)
    {
        this.latitudeStart = latitudeStart;
        this.latitudeTarget = latitudeTarget;
        this.longitudeStart = longitudeStart;
        this.longitudeTarget = longitudeTarget;
        this.name = name;
    }
}
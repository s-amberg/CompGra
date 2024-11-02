namespace Utils;

public class PlanetCalc
{
    public static double CenterDistance(double distance, PlanetInfo a, PlanetInfo b) {
        return distance / (a.mass / b.mass + 1);
    }
    public static float ToScale(double value) {
        return (float)(value / Constants.Earth.diameter);
    }
}
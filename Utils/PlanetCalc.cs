namespace Utils;

public class PlanetCalc
{
    public static double CenterDistance(double distance, PlanetInfo a, PlanetInfo b) {
        return distance / (a.mass / b.mass + 1);
    }
}
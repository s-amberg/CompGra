namespace Utils;

public record class PlanetInfo(
     double mass,
     double diameter,
     double distance,
     double rotationPeriod,
     float xTilt,
     float zTilt
     );

public static class Constants
{
     public static readonly PlanetInfo Earth = new PlanetInfo(
          5.97 * Math.Pow(10, 24),
          12700,
          0,
          1,
          (float)(2 * Math.PI / (360 / 23.8)),
          0
     );
     
     public static readonly PlanetInfo Moon = new PlanetInfo (
          7.35 * Math.Pow(10, 22), 
          3470, 
          384000, 
          0,
          0, 
          0
     );
     
}

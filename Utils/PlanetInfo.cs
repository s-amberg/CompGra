namespace Utils;

public record class PlanetInfo(
     double mass, // [kg]
     double diameter, // [km]
     double distance, // [km to sun]
     double rotationPeriod, // syncodic
     float xTilt, // whatever i find at this point, to orbit ?
     float zTilt, // ???
     double orbitPeriod, // sidereal
     double orbitTilt // sun equator
     );

public static class Constants
{
     public const double AU = 149597870;

     public static float RadiansFromDegrees(double degrees) {
          return (float)(2 * Math.PI / (360 / degrees));
     }
     public static readonly PlanetInfo Sun = new PlanetInfo(
          mass: 1.989 * Math.Pow(10, 33),
          diameter: 695500 * 2,
          distance: 0,
          rotationPeriod: 0,
          xTilt: 0,
          zTilt: 0,
          orbitPeriod: 0,
          orbitTilt: 0
     );
     public static readonly PlanetInfo Mercury = new PlanetInfo(
          mass: 3.302 * Math.Pow(10, 23),
          diameter: 3879,
          distance: 57900000,
          rotationPeriod: 176,
          xTilt: RadiansFromDegrees(0.034),
          zTilt: 0,
          orbitPeriod: 87.9691,
          orbitTilt: RadiansFromDegrees(3.38)
     );
     
     public static readonly PlanetInfo Venus = new PlanetInfo(
          mass: 4.868 * Math.Pow(10, 24),
          diameter: 12104,
          distance: 108200000,
          rotationPeriod: -116.75,
          xTilt: RadiansFromDegrees(2.64),
          zTilt: 0,
          orbitPeriod: 224.701,
          orbitTilt: RadiansFromDegrees(3.86)
     );
     
     public static readonly PlanetInfo Earth = new PlanetInfo(
          5.97 * Math.Pow(10, 24),
          12700,
          1*AU,
          1,
          RadiansFromDegrees(23.8),
          0,
          orbitPeriod: 365.256363004,
          orbitTilt: RadiansFromDegrees(7.155)
     );
     
     public static readonly PlanetInfo Moon = new PlanetInfo (
          7.35 * Math.Pow(10, 22), 
          3470, 
          1*AU + 384000, 
          0,
          0, 
          0,
          orbitPeriod: 27.3,
          orbitTilt: 0
     );
     
     public static readonly PlanetInfo Mars = new PlanetInfo(
          mass: 6.4171 * Math.Pow(10, 23),
          diameter: 3396.2 * 2,
          distance: 249261000,
          rotationPeriod: 1.025957,
          xTilt: RadiansFromDegrees(25.19),
          zTilt: 0,
          orbitPeriod: 686.980,
          orbitTilt: RadiansFromDegrees(5.65)
     );
     
     public static readonly PlanetInfo Jupiter = new PlanetInfo(
          mass: 1.8982 * Math.Pow(10, 27),
          diameter: 69911 * 2,
          distance: 816.363  * Math.Pow(10, 6),
          rotationPeriod: 24 / 9.9258,
          xTilt: RadiansFromDegrees(3.13),
          zTilt: 0,
          orbitPeriod: 4332.59,
          orbitTilt: RadiansFromDegrees(6.09)
     );
     
     public static readonly PlanetInfo Saturn = new PlanetInfo(
          mass: 5.6834 * Math.Pow(10, 26),
          diameter: 58232 * 2,
          distance: 1514.50 * Math.Pow(10, 6),
          rotationPeriod: 24 / 10.5433,
          xTilt: RadiansFromDegrees(26.73),
          zTilt: 0,
          orbitPeriod: 10755.70,
          orbitTilt: RadiansFromDegrees(5.51)
     );
     
     public static readonly PlanetInfo Uranus = new PlanetInfo(
          mass: 8.6810 * Math.Pow(10, 25),
          diameter: 25362 * 2,
          distance: 3.00639 * Math.Pow(10, 9),
          rotationPeriod: -0.71832,
          xTilt: RadiansFromDegrees(82.23),
          zTilt: 0,
          orbitPeriod: 30688.5,
          orbitTilt: RadiansFromDegrees(6.48)
     );
     
     public static readonly PlanetInfo Neptune = new PlanetInfo(
          mass: 1.0240 * Math.Pow(10, 26),
          diameter: 24622 * 2,
          distance: 4.54 * Math.Pow(10, 9),
          rotationPeriod: 0.67125,
          xTilt: RadiansFromDegrees(28.32),
          zTilt: 0,
          orbitPeriod: 60195,
          orbitTilt: RadiansFromDegrees(6.43)
     );
     
}

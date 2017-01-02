using System;
using static ABUtils.GISUtils;

namespace ArchyManager
{
    public static class GISConversion
    {
        private static double PI = 3.14159265358979;
        private static double UTMScale = 0.9996;
        public static Ellipsoid NAD83 = new Ellipsoid(GeodeticDatum.NAD83);
        public static Ellipsoid WGS84 = new Ellipsoid(GeodeticDatum.WGS84);


        public static GeographicCoordinate UTMXYToLatLon(double easting, double northing, int zone, Hemisphere hsphere, Ellipsoid e)
        {
            double cmeridian;

            easting -= 500000.0;
            easting /= UTMScale;

            /* If in southern hemisphere, adjust y accordingly. */
            if (hsphere == Hemisphere.Southern)
            {
                northing -= 10000000.0;
            }

            northing /= UTMScale;

            cmeridian = UTMCentralMeridian(zone);

            return MapXYToLatLon(easting, northing, cmeridian, e);
        }
        public static GeographicCoordinate MapXYToLatLon(double easting, double northing, double utmc, Ellipsoid e)
        {
            double Nf, Nfpow, nuf2, ep2, tf, tf2, tf4, cf;
            double x1frac, x2frac, x3frac, x4frac, x5frac, x6frac, x7frac, x8frac;
            double x2poly, x3poly, x4poly, x5poly, x6poly, x7poly, x8poly;

            /* Get the value of phif, the footpoint latitude. */
            double phif = FootpointLatitude(northing, e);

            /* Precalculate ep2 */
            ep2 = (Math.Pow(e.AxisMajor, 2.0) - Math.Pow(e.AxisMinor, 2.0))
                  / Math.Pow(e.AxisMinor, 2.0);

            /* Precalculate cos (phif) */
            cf = Math.Cos(phif);

            /* Precalculate nuf2 */
            nuf2 = ep2 * Math.Pow(cf, 2.0);

            /* Precalculate Nf and initialize Nfpow */
            Nf = Math.Pow(e.AxisMajor, 2.0) / (e.AxisMinor * Math.Sqrt(1 + nuf2));
            Nfpow = Nf;

            /* Precalculate tf */
            tf = Math.Tan(phif);
            tf2 = tf * tf;
            tf4 = tf2 * tf2;

            /* Precalculate fractional coefficients for x**n in the equations
               below to simplify the expressions for latitude and longitude. */
            x1frac = 1.0 / (Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**2) */
            x2frac = tf / (2.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**3) */
            x3frac = 1.0 / (6.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**4) */
            x4frac = tf / (24.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**5) */
            x5frac = 1.0 / (120.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**6) */
            x6frac = tf / (720.0 * Nfpow);

            Nfpow *= Nf;   /* now equals Nf**7) */
            x7frac = 1.0 / (5040.0 * Nfpow * cf);

            Nfpow *= Nf;   /* now equals Nf**8) */
            x8frac = tf / (40320.0 * Nfpow);

            /* Precalculate polynomial coefficients for x**n.
               -- x**1 does not have a polynomial coefficient. */
            x2poly = -1.0 - nuf2;

            x3poly = -1.0 - 2 * tf2 - nuf2;

            x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2
                - 3.0 * (nuf2 * nuf2) - 9.0 * tf2 * (nuf2 * nuf2);

            x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;

            x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2
                + 162.0 * tf2 * nuf2;

            x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);

            x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);

            GeographicCoordinate gc = new GeographicCoordinate();

            /* Calculate latitude */
            gc.Latitude = Rad2Deg(phif + x2frac * x2poly * (easting * easting)
                + x4frac * x4poly * Math.Pow(easting, 4.0)
                + x6frac * x6poly * Math.Pow(easting, 6.0)
                + x8frac * x8poly * Math.Pow(easting, 8.0));

            /* Calculate longitude */
            gc.Longitude = Rad2Deg(utmc + x1frac * easting
                + x3frac * x3poly * Math.Pow(easting, 3.0)
                + x5frac * x5poly * Math.Pow(easting, 5.0)
                + x7frac * x7poly * Math.Pow(easting, 7.0));

            return gc;
        }
        private static double FootpointLatitude(double northing, Ellipsoid e)
        {
            double y_, alpha_, beta_, gamma_, delta_, epsilon_, n;

            /* Precalculate n (Eq. 10.18) */
            n = (e.AxisMajor - e.AxisMinor) / (e.AxisMajor + e.AxisMinor);

            /* Precalculate alpha_ (Eq. 10.22) */
            /* (Same as alpha in Eq. 10.17) */
            alpha_ = ((e.AxisMajor + e.AxisMinor) / 2.0)
                * (1 + (Math.Pow(n, 2.0) / 4) + (Math.Pow(n, 4.0) / 64));

            /* Precalculate y_ (Eq. 10.23) */
            y_ = northing / alpha_;

            /* Precalculate beta_ (Eq. 10.22) */
            beta_ = (3.0 * n / 2.0) + (-27.0 * Math.Pow(n, 3.0) / 32.0)
                + (269.0 * Math.Pow(n, 5.0) / 512.0);

            /* Precalculate gamma_ (Eq. 10.22) */
            gamma_ = (21.0 * Math.Pow(n, 2.0) / 16.0)
                + (-55.0 * Math.Pow(n, 4.0) / 32.0);

            /* Precalculate delta_ (Eq. 10.22) */
            delta_ = (151.0 * Math.Pow(n, 3.0) / 96.0)
                + (-417.0 * Math.Pow(n, 5.0) / 128.0);

            /* Precalculate epsilon_ (Eq. 10.22) */
            epsilon_ = (1097.0 * Math.Pow(n, 4.0) / 512.0);

            /* Now calculate the sum of the series (Eq. 10.21) */
            return y_ + (beta_ * Math.Sin(2.0 * y_))
                + (gamma_ * Math.Sin(4.0 * y_))
                + (delta_ * Math.Sin(6.0 * y_))
                + (epsilon_ * Math.Sin(8.0 * y_));
        }

        private static double UTMCentralMeridian(int zone)
        {
            return Deg2Rad(-183.0 + (zone * 6.0));
        }
        private static double Deg2Rad(double deg)
        {
            return (deg / 180.0 * PI);
        }
        private static double Rad2Deg(double rad)
        {
            return (rad / PI * 180.0);
        }


        //Matt's old converter - precision may be improved by implementing JavaScript converter below
        public static int LatLongToUTM(double dblLat, double dblLong, ref double Easting, ref double Northing, int? UTMZone = null)
        {

            double dblAA;
            double dblBB;
            double dblCC;
            double dblDD;
            double dblEE;
            double dblFF;
            double dblMM;
            int intzone;
            double dblLambDAO;
            double dblZZ;
            double dblVV;
            double dblNN;
            double dblTT;
            double pi;

            double dblScaleFactor;
            double dblEastF = 0;
            double dblNorthF = 0;
            double dblESQR;
            double dblSMA;
            double dblRadsLat;
            double dblRadsLong;

            //Set Constants
            pi = 3.14159265358989;
            dblScaleFactor = 0.9996;
            dblEastF = 500000;
            dblESQR = 0.00669437999013;
            dblSMA = 6378137;
            if (dblLat < 0) dblNorthF = 10000000;
            dblRadsLat = dblLat / (180 / pi);
            dblRadsLong = dblLong / (180 / pi);

            //Start Calculations
            dblAA = 1 - 2 * dblESQR / 8 - 12 * (dblESQR * dblESQR) / 256 - 60 * (dblESQR * dblESQR * dblESQR) / 3072 - 75 * (dblESQR * dblESQR * dblESQR * dblESQR) / 16384 - 441 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 65536;
            dblBB = 3 * dblESQR / 8 + 24 * (dblESQR * dblESQR) / 256 + 135 * (dblESQR * dblESQR * dblESQR) / 3072 + 105 * (dblESQR * dblESQR * dblESQR * dblESQR) / 2048 + 2205 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 65536;
            dblCC = 15 * (dblESQR * dblESQR) / 256 + 135 * (dblESQR * dblESQR * dblESQR) / 3072 + 525 * (dblESQR * dblESQR * dblESQR * dblESQR) / 4096 + 1575 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 16384;
            dblDD = 35 * (dblESQR * dblESQR * dblESQR) / 3072 + 175 * (dblESQR * dblESQR * dblESQR * dblESQR) / 2048 + 11025 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 131072;
            dblEE = 315 * (dblESQR * dblESQR * dblESQR * dblESQR) / 16384 + 2205 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 65536;
            dblFF = 693 * (dblESQR * dblESQR * dblESQR * dblESQR * dblESQR) / 131072;
            dblMM = dblScaleFactor * dblSMA * (dblAA * dblRadsLat - dblBB * Math.Sin(2 * dblRadsLat) + dblCC * Math.Sin(4 * dblRadsLat) - dblDD * Math.Sin(6 * dblRadsLat) + dblEE * Math.Sin(8 * dblRadsLat) - dblFF * Math.Sin(10 * dblRadsLat));
            intzone = UTMZone ?? Convert.ToInt32(Math.Floor((dblLong + 180) / 6 + 1));

            //NonStandardZone dblLat, dblLong, intzone
            dblLambDAO = intzone * 6 - 183;
            dblZZ = (dblLong - dblLambDAO) / (180 / pi) * Math.Cos(dblRadsLat);
            dblVV = dblScaleFactor * dblSMA / Math.Sqrt(1 - dblESQR * Math.Pow(Math.Sin(dblRadsLat), 2));
            dblNN = dblESQR * (Math.Cos(dblRadsLat) * Math.Cos(dblRadsLat)) / (1 - dblESQR);
            dblTT = Math.Tan(dblRadsLat) * Math.Tan(dblRadsLat);
            Easting = ((((2 * dblTT - 58) * dblTT + 14) * dblNN + (dblTT - 18) * dblTT + 5) * ((dblZZ * dblZZ) / 20) + dblNN - dblTT + 1);
            Easting = (Easting * ((dblZZ * dblZZ) / 6) + 1) * dblVV * dblZZ + dblEastF;
            Northing = ((dblTT - 58) * dblTT + 61) * ((dblZZ * dblZZ) / 30) + (9 + 4 * dblNN) * dblNN - dblTT + 5;
            Northing = (Northing * ((dblZZ * dblZZ) / 12) + 1) * dblVV * ((dblZZ * dblZZ) / 2) * Math.Tan(dblRadsLat) + dblMM + dblNorthF;

            if (Convert.ToInt32(Easting + 0.01) == (Convert.ToInt32(Easting) + 1)) Easting = Easting + 0.01;
            if (Convert.ToInt32(Northing + 0.01) == (Convert.ToInt32(Northing) + 1)) Northing = Northing + 0.01;
            Easting = Easting * 0.00001;
            Northing = Northing * 0.00001;
            if (Northing < 0) Northing = Northing + 100;
            Easting = Easting * 1000000000 / 1000000000;
            Northing = Northing * 1000000000 / 1000000000;
            Easting = Easting * 100000;
            Northing = Northing * 100000;

            return intzone;
        }
    }

    public class GeographicCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class ProjectedCoordinate
    {
        public double Easting { get; set; }
        public double Northing { get; set; }
        public byte Zone { get; set; }
    }
    public enum Hemisphere
    {
        Northern = 0,
        Southern = 1
    }

}



///*
// Matt's old converter - faulty results
//public static void UTMtoLL(double UTMNorthing, double UTMEasting, int UTMZone, ref double Longitude, ref double Latitude)
//{
//    double k0;
//    double a;
//    double eccSquared;
//    double eccPrimeSquared;
//    double e1;
//    double N1;
//    double t1;
//    double C1;
//    double R1;
//    double D;
//    double m;
//    double LongOrigin;
//    double mu;
//    double phi1;
//    double phi1Rad;
//    double x;
//    double y;
//    double dblLatitude = 0;
//    double dblLongitude = 0;
//    double pi;
//    pi = 3.14159265;
//    k0 = 0.9996;
//    //equatorial radius
//    a = NAD83.AxisMajor;
//    //eccentricitysquared
//    eccSquared = NAD83.EccSquared;

//    e1 = (1 - Math.Pow((1 - eccSquared), 0.5)) / (1 + Math.Pow((1 - eccSquared), 0.5));

//    x = UTMEasting - 500000;  //remove 500,000 meter offset for longitude
//    y = UTMNorthing;


//    eccPrimeSquared = (eccSquared) / (1 - eccSquared);

//    m = y / k0;
//    mu = m / (a * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

//    phi1Rad = mu + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu) + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * mu) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * mu);
//    phi1 = phi1Rad * (180 / pi);
//    N1 = a / ((1 - eccSquared * Math.Sin(phi1Rad) * Math.Pow(Math.Sin(phi1Rad), 0.5)));
//    t1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
//    C1 = eccPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
//    R1 = a * (1 - eccSquared) / (1 - eccSquared * Math.Sin(phi1Rad) * Math.Pow(Math.Sin(phi1Rad), 1.5));
//    D = x / (N1 * k0);

//    dblLatitude = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * t1 + 10 * C1 - 4 * C1 * C1 - 9 * eccPrimeSquared) * D * D * D * D / 24 + (61 + 90 * t1 + 298 * C1 + 45 * t1 * t1 - 252 * eccPrimeSquared - 3 * C1 * C1) * D * D * D * D * D * D / 720);
//    dblLatitude = dblLatitude * (180 / pi);
//    Latitude = dblLatitude;

//    LongOrigin = (UTMZone * 6) - (180 + 3); //+3 puts origin in middle of zone
//    dblLongitude = (D - (1 + 2 * t1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * t1 - 3 * C1 * C1 + 8 * eccPrimeSquared + 24 * t1 * t1) * D * D * D * D * D / 120) / Math.Cos(phi1Rad);
//    dblLongitude = LongOrigin + dblLongitude * (180 / pi);
//    Longitude = dblLongitude;

//}



///*
//var pi = 3.14159265358979;

// Ellipsoid model constants (actual values here are for WGS84) 
//var sm_a = 6378137.0;
//var sm_b = 6356752.314;
//var sm_EccSquared = 6.69437999013e-03;

//var UTMScaleFactor = 0.9996;


//* DegToRad
//*
//* Converts degrees to radians.
//*
//*/
//function DegToRad(deg)
//{
//    return (deg / 180.0 * pi)
//}




//* RadToDeg
//*
//* Converts radians to degrees.
//*
//*/
//function RadToDeg(rad)
//{
//    return (rad / pi * 180.0)
//}




//* ArcLengthOfMeridian
//*
//* Computes the ellipsoidal distance from the equator to a point at a
//* given latitude.
//*
//* Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
//* GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
//*
//* Inputs:
//*     phi - Latitude of the point, in radians.
//*
//* Globals:
//*     sm_a - Ellipsoid model major axis.
//*     sm_b - Ellipsoid model minor axis.
//*
//* Returns:
//*     The ellipsoidal distance of the point from the equator, in meters.
//*
//*/
//function ArcLengthOfMeridian(phi)
//{
//    var alpha, beta, gamma, delta, epsilon, n;
//    var result;

//    /* Precalculate n */
//    n = (sm_a - sm_b) / (sm_a + sm_b);

//    /* Precalculate alpha */
//    alpha = ((sm_a + sm_b) / 2.0)
//       * (1.0 + (Math.pow(n, 2.0) / 4.0) + (Math.pow(n, 4.0) / 64.0));

//    /* Precalculate beta */
//    beta = (-3.0 * n / 2.0) + (9.0 * Math.pow(n, 3.0) / 16.0)
//       + (-3.0 * Math.pow(n, 5.0) / 32.0);

//    /* Precalculate gamma */
//    gamma = (15.0 * Math.pow(n, 2.0) / 16.0)
//        + (-15.0 * Math.pow(n, 4.0) / 32.0);

//    /* Precalculate delta */
//    delta = (-35.0 * Math.pow(n, 3.0) / 48.0)
//        + (105.0 * Math.pow(n, 5.0) / 256.0);

//    /* Precalculate epsilon */
//    epsilon = (315.0 * Math.pow(n, 4.0) / 512.0);

//    /* Now calculate the sum of the series and return */
//    result = alpha
//        * (phi + (beta * Math.sin(2.0 * phi))
//            + (gamma * Math.sin(4.0 * phi))
//            + (delta * Math.sin(6.0 * phi))
//            + (epsilon * Math.sin(8.0 * phi)));

//    return result;
//}



//* UTMCentralMeridian
//*
//* Determines the central meridian for the given UTM zone.
//*
//* Inputs:
//*     zone - An integer value designating the UTM zone, range [1,60].
//*
//* Returns:
//*   The central meridian for the given UTM zone, in radians, or zero
//*   if the UTM zone parameter is outside the range [1,60].
//*   Range of the central meridian is the radian equivalent of [-177,+177].
//*
//*/
//function UTMCentralMeridian(zone)
//{
//    var cmeridian;

//    cmeridian = DegToRad(-183.0 + (zone * 6.0));

//    return cmeridian;
//}



//* FootpointLatitude
//*
//* Computes the footpoint latitude for use in converting transverse
//* Mercator coordinates to ellipsoidal coordinates.
//*
//* Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
//*   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
//*
//* Inputs:
//*   y - The UTM northing coordinate, in meters.
//*
//* Returns:
//*   The footpoint latitude, in radians.
//*
//*/
//function FootpointLatitude(y)
//{
//    var y_, alpha_, beta_, gamma_, delta_, epsilon_, n;
//    var result;

//    /* Precalculate n (Eq. 10.18) */
//    n = (sm_a - sm_b) / (sm_a + sm_b);

//    /* Precalculate alpha_ (Eq. 10.22) */
//    /* (Same as alpha in Eq. 10.17) */
//    alpha_ = ((sm_a + sm_b) / 2.0)
//        * (1 + (Math.pow(n, 2.0) / 4) + (Math.pow(n, 4.0) / 64));

//    /* Precalculate y_ (Eq. 10.23) */
//    y_ = y / alpha_;

//    /* Precalculate beta_ (Eq. 10.22) */
//    beta_ = (3.0 * n / 2.0) + (-27.0 * Math.pow(n, 3.0) / 32.0)
//        + (269.0 * Math.pow(n, 5.0) / 512.0);

//    /* Precalculate gamma_ (Eq. 10.22) */
//    gamma_ = (21.0 * Math.pow(n, 2.0) / 16.0)
//        + (-55.0 * Math.pow(n, 4.0) / 32.0);

//    /* Precalculate delta_ (Eq. 10.22) */
//    delta_ = (151.0 * Math.pow(n, 3.0) / 96.0)
//        + (-417.0 * Math.pow(n, 5.0) / 128.0);

//    /* Precalculate epsilon_ (Eq. 10.22) */
//    epsilon_ = (1097.0 * Math.pow(n, 4.0) / 512.0);

//    /* Now calculate the sum of the series (Eq. 10.21) */
//    result = y_ + (beta_ * Math.sin(2.0 * y_))
//        + (gamma_ * Math.sin(4.0 * y_))
//        + (delta_ * Math.sin(6.0 * y_))
//        + (epsilon_ * Math.sin(8.0 * y_));

//    return result;
//}



//* MapLatLonToXY
//*
//* Converts a latitude/longitude pair to x and y coordinates in the
//* Transverse Mercator projection.  Note that Transverse Mercator is not
//* the same as UTM; a scale factor is required to convert between them.
//*
//* Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
//* GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
//*
//* Inputs:
//*    phi - Latitude of the point, in radians.
//*    lambda - Longitude of the point, in radians.
//*    lambda0 - Longitude of the central meridian to be used, in radians.
//*
//* Outputs:
//*    xy - A 2-element array containing the x and y coordinates
//*         of the computed point.
//*
//* Returns:
//*    The function does not return a value.
//*
//*/
//function MapLatLonToXY(phi, lambda, lambda0, xy)
//{
//    var N, nu2, ep2, t, t2, l;
//    var l3coef, l4coef, l5coef, l6coef, l7coef, l8coef;
//    var tmp;

//    /* Precalculate ep2 */
//    ep2 = (Math.pow(sm_a, 2.0) - Math.pow(sm_b, 2.0)) / Math.pow(sm_b, 2.0);

//    /* Precalculate nu2 */
//    nu2 = ep2 * Math.pow(Math.cos(phi), 2.0);

//    /* Precalculate N */
//    N = Math.pow(sm_a, 2.0) / (sm_b * Math.sqrt(1 + nu2));

//    /* Precalculate t */
//    t = Math.tan(phi);
//    t2 = t * t;
//    tmp = (t2 * t2 * t2) - Math.pow(t, 6.0);

//    /* Precalculate l */
//    l = lambda - lambda0;

//    /* Precalculate coefficients for l**n in the equations below
//       so a normal human being can read the expressions for easting
//       and northing
//       -- l**1 and l**2 have coefficients of 1.0 */
//    l3coef = 1.0 - t2 + nu2;

//    l4coef = 5.0 - t2 + 9 * nu2 + 4.0 * (nu2 * nu2);

//    l5coef = 5.0 - 18.0 * t2 + (t2 * t2) + 14.0 * nu2
//        - 58.0 * t2 * nu2;

//    l6coef = 61.0 - 58.0 * t2 + (t2 * t2) + 270.0 * nu2
//        - 330.0 * t2 * nu2;

//    l7coef = 61.0 - 479.0 * t2 + 179.0 * (t2 * t2) - (t2 * t2 * t2);

//    l8coef = 1385.0 - 3111.0 * t2 + 543.0 * (t2 * t2) - (t2 * t2 * t2);

//    /* Calculate easting (x) */
//    xy[0] = N * Math.cos(phi) * l
//        + (N / 6.0 * Math.pow(Math.cos(phi), 3.0) * l3coef * Math.pow(l, 3.0))
//        + (N / 120.0 * Math.pow(Math.cos(phi), 5.0) * l5coef * Math.pow(l, 5.0))
//        + (N / 5040.0 * Math.pow(Math.cos(phi), 7.0) * l7coef * Math.pow(l, 7.0));

//    /* Calculate northing (y) */
//    xy[1] = ArcLengthOfMeridian(phi)
//        + (t / 2.0 * N * Math.pow(Math.cos(phi), 2.0) * Math.pow(l, 2.0))
//        + (t / 24.0 * N * Math.pow(Math.cos(phi), 4.0) * l4coef * Math.pow(l, 4.0))
//        + (t / 720.0 * N * Math.pow(Math.cos(phi), 6.0) * l6coef * Math.pow(l, 6.0))
//        + (t / 40320.0 * N * Math.pow(Math.cos(phi), 8.0) * l8coef * Math.pow(l, 8.0));

//    return;
//}



//* MapXYToLatLon
//*
//* Converts x and y coordinates in the Transverse Mercator projection to
//* a latitude/longitude pair.  Note that Transverse Mercator is not
//* the same as UTM; a scale factor is required to convert between them.
//*
//* Reference: Hoffmann-Wellenhof, B., Lichtenegger, H., and Collins, J.,
//*   GPS: Theory and Practice, 3rd ed.  New York: Springer-Verlag Wien, 1994.
//*
//* Inputs:
//*   x - The easting of the point, in meters.
//*   y - The northing of the point, in meters.
//*   lambda0 - Longitude of the central meridian to be used, in radians.
//*
//* Outputs:
//*   philambda - A 2-element containing the latitude and longitude
//*               in radians.
//*
//* Returns:
//*   The function does not return a value.
//*
//* Remarks:
//*   The local variables Nf, nuf2, tf, and tf2 serve the same purpose as
//*   N, nu2, t, and t2 in MapLatLonToXY, but they are computed with respect
//*   to the footpoint latitude phif.
//*
//*   x1frac, x2frac, x2poly, x3poly, etc. are to enhance readability and
//*   to optimize computations.
//*
//*/
//function MapXYToLatLon(x, y, lambda0, philambda)
//{
//    var phif, Nf, Nfpow, nuf2, ep2, tf, tf2, tf4, cf;
//    var x1frac, x2frac, x3frac, x4frac, x5frac, x6frac, x7frac, x8frac;
//    var x2poly, x3poly, x4poly, x5poly, x6poly, x7poly, x8poly;

//    /* Get the value of phif, the footpoint latitude. */
//    phif = FootpointLatitude(y);

//    /* Precalculate ep2 */
//    ep2 = (Math.pow(sm_a, 2.0) - Math.pow(sm_b, 2.0))
//          / Math.pow(sm_b, 2.0);

//    /* Precalculate cos (phif) */
//    cf = Math.cos(phif);

//    /* Precalculate nuf2 */
//    nuf2 = ep2 * Math.pow(cf, 2.0);

//    /* Precalculate Nf and initialize Nfpow */
//    Nf = Math.pow(sm_a, 2.0) / (sm_b * Math.sqrt(1 + nuf2));
//    Nfpow = Nf;

//    /* Precalculate tf */
//    tf = Math.tan(phif);
//    tf2 = tf * tf;
//    tf4 = tf2 * tf2;

//    /* Precalculate fractional coefficients for x**n in the equations
//       below to simplify the expressions for latitude and longitude. */
//    x1frac = 1.0 / (Nfpow * cf);

//    Nfpow *= Nf;   /* now equals Nf**2) */
//    x2frac = tf / (2.0 * Nfpow);

//    Nfpow *= Nf;   /* now equals Nf**3) */
//    x3frac = 1.0 / (6.0 * Nfpow * cf);

//    Nfpow *= Nf;   /* now equals Nf**4) */
//    x4frac = tf / (24.0 * Nfpow);

//    Nfpow *= Nf;   /* now equals Nf**5) */
//    x5frac = 1.0 / (120.0 * Nfpow * cf);

//    Nfpow *= Nf;   /* now equals Nf**6) */
//    x6frac = tf / (720.0 * Nfpow);

//    Nfpow *= Nf;   /* now equals Nf**7) */
//    x7frac = 1.0 / (5040.0 * Nfpow * cf);

//    Nfpow *= Nf;   /* now equals Nf**8) */
//    x8frac = tf / (40320.0 * Nfpow);

//    /* Precalculate polynomial coefficients for x**n.
//       -- x**1 does not have a polynomial coefficient. */
//    x2poly = -1.0 - nuf2;

//    x3poly = -1.0 - 2 * tf2 - nuf2;

//    x4poly = 5.0 + 3.0 * tf2 + 6.0 * nuf2 - 6.0 * tf2 * nuf2
//        - 3.0 * (nuf2 * nuf2) - 9.0 * tf2 * (nuf2 * nuf2);

//    x5poly = 5.0 + 28.0 * tf2 + 24.0 * tf4 + 6.0 * nuf2 + 8.0 * tf2 * nuf2;

//    x6poly = -61.0 - 90.0 * tf2 - 45.0 * tf4 - 107.0 * nuf2
//        + 162.0 * tf2 * nuf2;

//    x7poly = -61.0 - 662.0 * tf2 - 1320.0 * tf4 - 720.0 * (tf4 * tf2);

//    x8poly = 1385.0 + 3633.0 * tf2 + 4095.0 * tf4 + 1575 * (tf4 * tf2);

//    /* Calculate latitude */
//    philambda[0] = phif + x2frac * x2poly * (x * x)
//        + x4frac * x4poly * Math.pow(x, 4.0)
//        + x6frac * x6poly * Math.pow(x, 6.0)
//        + x8frac * x8poly * Math.pow(x, 8.0);

//    /* Calculate longitude */
//    philambda[1] = lambda0 + x1frac * x
//        + x3frac * x3poly * Math.pow(x, 3.0)
//        + x5frac * x5poly * Math.pow(x, 5.0)
//        + x7frac * x7poly * Math.pow(x, 7.0);

//    return;
//}




//* LatLonToUTMXY
//*
//* Converts a latitude/longitude pair to x and y coordinates in the
//* Universal Transverse Mercator projection.
//*
//* Inputs:
//*   lat - Latitude of the point, in radians.
//*   lon - Longitude of the point, in radians.
//*   zone - UTM zone to be used for calculating values for x and y.
//*          If zone is less than 1 or greater than 60, the routine
//*          will determine the appropriate zone from the value of lon.
//*
//* Outputs:
//*   xy - A 2-element array where the UTM x and y values will be stored.
//*
//* Returns:
//*   The UTM zone used for calculating the values of x and y.
//*
//*/
//function LatLonToUTMXY(lat, lon, zone, xy)
//{
//    MapLatLonToXY(lat, lon, UTMCentralMeridian(zone), xy);

//    /* Adjust easting and northing for UTM system. */
//    xy[0] = xy[0] * UTMScaleFactor + 500000.0;
//    xy[1] = xy[1] * UTMScaleFactor;
//    if (xy[1] < 0.0)
//        xy[1] = xy[1] + 10000000.0;

//    return zone;
//}



//* UTMXYToLatLon
//*
//* Converts x and y coordinates in the Universal Transverse Mercator
//* projection to a latitude/longitude pair.
//*
//* Inputs:
//*	x - The easting of the point, in meters.
//*	y - The northing of the point, in meters.
//*	zone - The UTM zone in which the point lies.
//*	southhemi - True if the point is in the southern hemisphere;
//*               false otherwise.
//*
//* Outputs:
//*	latlon - A 2-element array containing the latitude and
//*            longitude of the point, in radians.
//*
//* Returns:
//*	The function does not return a value.
//*
//*/
//function UTMXYToLatLon(x, y, zone, southhemi, latlon)
//{
//    var cmeridian;

//    x -= 500000.0;
//    x /= UTMScaleFactor;

//    /* If in southern hemisphere, adjust y accordingly. */
//    if (southhemi)
//        y -= 10000000.0;

//    y /= UTMScaleFactor;

//    cmeridian = UTMCentralMeridian(zone);
//    MapXYToLatLon(x, y, cmeridian, latlon);

//    return;
//}




//* btnToUTM_OnClick
//*
//* Called when the btnToUTM button is clicked.
//*
//*/
//function btnToUTM_OnClick()
//{
//    var xy = new Array(2);

//    if (isNaN(parseFloat(document.frmConverter.txtLongitude.value)))
//    {
//        alert("Please enter a valid longitude in the lon field.");
//        return false;
//    }

//    lon = parseFloat(document.frmConverter.txtLongitude.value);

//    if ((lon < -180.0) || (180.0 <= lon))
//    {
//        alert("The longitude you entered is out of range.  " +
//               "Please enter a number in the range [-180, 180).");
//        return false;
//    }

//    if (isNaN(parseFloat(document.frmConverter.txtLatitude.value)))
//    {
//        alert("Please enter a valid latitude in the lat field.");
//        return false;
//    }

//    lat = parseFloat(document.frmConverter.txtLatitude.value);

//    if ((lat < -90.0) || (90.0 < lat))
//    {
//        alert("The latitude you entered is out of range.  " +
//               "Please enter a number in the range [-90, 90].");
//        return false;
//    }

//    // Compute the UTM zone.
//    zone = Math.floor((lon + 180.0) / 6) + 1;

//    zone = LatLonToUTMXY(DegToRad(lat), DegToRad(lon), zone, xy);

//    /* Set the output controls.  */
//    document.frmConverter.txtX.value = xy[0];
//    document.frmConverter.txtY.value = xy[1];
//    document.frmConverter.txtZone.value = zone;
//    if (lat < 0)
//        // Set the S button.
//        document.frmConverter.rbtnHemisphere[1].checked = true;
//    else
//        // Set the N button.
//        document.frmConverter.rbtnHemisphere[0].checked = true;


//    return true;
//    }


//    /*
//    * btnToGeographic_OnClick
//    *
//    * Called when the btnToGeographic button is clicked.
//    *
//    */
//    function btnToGeographic_OnClick ()
//{
//        latlon = new Array(2);
//        var x, y, zone, southhemi;

//        if (isNaN(parseFloat(document.frmConverter.txtX.value)))
//        {
//            alert("Please enter a valid easting in the x field.");
//            return false;
//        }

//        x = parseFloat(document.frmConverter.txtX.value);

//        if (isNaN(parseFloat(document.frmConverter.txtY.value)))
//        {
//            alert("Please enter a valid northing in the y field.");
//            return false;
//        }

//        y = parseFloat(document.frmConverter.txtY.value);

//        if (isNaN(parseInt(document.frmConverter.txtZone.value)))
//        {
//            alert("Please enter a valid UTM zone in the zone field.");
//            return false;
//        }

//        zone = parseFloat(document.frmConverter.txtZone.value);

//        if ((zone < 1) || (60 < zone))
//        {
//            alert("The UTM zone you entered is out of range.  " +
//                   "Please enter a number in the range [1, 60].");
//            return false;
//        }

//        if (document.frmConverter.rbtnHemisphere[1].checked == true)
//        southhemi = true;
//    else
//        southhemi = false;

//            UTMXYToLatLon(x, y, zone, southhemi, latlon);

//            document.frmConverter.txtLongitude.value = RadToDeg(latlon[1]);
//            document.frmConverter.txtLatitude.value = RadToDeg(latlon[0]);

//            return true;
//            }



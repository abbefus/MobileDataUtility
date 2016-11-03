using System;

namespace ArchyManager
{
    public class LatLongCalc
    {

        public static void UTMtoLL(double UTMNorthing, double UTMEasting, int UTMZone, ref double Longitude, ref double Latitude)
        {
            double k0;
            double a;
            double eccSquared;
            double eccPrimeSquared;
            double e1;
            double N1;
            double t1;
            double C1;
            double R1;
            double D;
            double m;
            double LongOrigin;
            double mu;
            double phi1;
            double phi1Rad;
            double x;
            double y;
            double dblLatitude = 0;
            double dblLongitude = 0;
            double pi;
            pi = 3.14159265;
            k0 = 0.9996;
            //equatorial radius
            a = 6378137;
            //eccentricitysquared
            eccSquared = 0.00669438;
            
            e1 = (1 - Math.Pow((1 - eccSquared), 0.5)) / (1 + Math.Pow((1 - eccSquared), 0.5));
            
            x = UTMEasting - 500000;  //remove 500,000 meter offset for longitude
            y = UTMNorthing;

            LongOrigin = (UTMZone - 1) * 6 - 180 + 3; //+3 puts origin in middle of zone

            eccPrimeSquared = (eccSquared) / (1 - eccSquared);

            m = y / k0;
            mu = m / (a * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

            phi1Rad = mu + (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32) * Math.Sin(2 * mu) + (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32) * Math.Sin(4 * mu) + (151 * e1 * e1 * e1 / 96) * Math.Sin(6 * mu) + (1097 * e1 * e1 * e1 * e1 / 512) * Math.Sin(8 * mu);
            phi1 = phi1Rad * (180 / pi);
            N1 = a / ((1 - eccSquared * Math.Sin(phi1Rad) * Math.Pow(Math.Sin(phi1Rad),0.5)));
            t1 = Math.Tan(phi1Rad) * Math.Tan(phi1Rad);
            C1 = eccPrimeSquared * Math.Cos(phi1Rad) * Math.Cos(phi1Rad);
            R1 = a * (1 - eccSquared) / (1 - eccSquared * Math.Sin(phi1Rad) * Math.Pow(Math.Sin(phi1Rad),1.5));
            D = x / (N1 * k0);

            dblLatitude = phi1Rad - (N1 * Math.Tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * t1 + 10 * C1 - 4 * C1 * C1 - 9 * eccPrimeSquared) * D * D * D * D / 24 + (61 + 90 * t1 + 298 * C1 + 45 * t1 * t1 - 252 * eccPrimeSquared - 3 * C1 * C1) * D * D * D * D * D * D / 720);
            dblLatitude = dblLatitude * (180 / pi);
            Latitude = dblLatitude;

            dblLongitude = (D - (1 + 2 * t1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * t1 - 3 * C1 * C1 + 8 * eccPrimeSquared + 24 * t1 * t1) * D * D * D * D * D / 120) / Math.Cos(phi1Rad);
            dblLongitude = LongOrigin + dblLongitude * (180 / pi);
            Longitude = dblLongitude;

        }

        public static void LatLongToUTM(double dblLat, double dblLong, int UTMZone, ref double Easting, ref double Northing)
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
    double dblEastF=0;
    double dblNorthF = 0;
    //double dblx=0;
    double dblESQR;
    double dblSMA;
    double dblRadsLat;
    double dblRadsLong;

     //Set Constants
     pi = 3.14159265358989;
     dblScaleFactor = 0.9996;
     dblEastF = 500000;
     //dblx = 1;
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
     intzone = Convert.ToInt32(dblLong / 6 + 31);
     //iZone = CStr(intzone);
     //NonStandardZone dblLat, dblLong, intzone
     dblLambDAO = intzone * 6 - 183;
     dblZZ = (dblLong - dblLambDAO) / (180 / pi) * Math.Cos(dblRadsLat);
     dblVV = dblScaleFactor * dblSMA / Math.Sqrt(1 - dblESQR * Math.Pow(Math.Sin(dblRadsLat),2));
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

    }
    }

}

// Some helpers for converting GPS readings from the WGS84 geodetic system to a local North-East-Up cartesian axis.

// The implementation here is according to the paper:
// "Conversion of Geodetic coordinates to the Local Tangent Plane" Version 2.01.
// "The basic reference for this paper is J.Farrell & M.Barth 'The Global Positioning System & Inertial Navigation'"
// Also helpful is Wikipedia: http://en.wikipedia.org/wiki/Geodetic_datum
// Taken from https://gist.github.com/govert/1b373696c9a27ff4c72a

using System;
using UnityEngine;
using Assets.Resources;

namespace Assets.HelperClasses
{
    public class GPSUtils : CSSingleton<GPSUtils>
    {
        // WGS-84 geodetic constants
        const double a = 6378137;           // WGS-84 Earth semimajor axis (m)
        const double b = 6356752.3142;      // WGS-84 Earth semiminor axis (m)
        const double f = (a - b) / a;       // Ellipsoid Flatness
        const double e_sq = f * (2 - f);    // Square of Eccentricity

        // Converts WGS-84 Geodetic point (lat, lon, h) to the 
        // Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z).
        public void GeodeticToEcef(double lat, double lon, double h,
                                          out double x, out double y, out double z)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreeToRadian(lat);
            var phi = DegreeToRadian(lon);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            x = (h + N) * cos_lambda * cos_phi;
            y = (h + N) * cos_lambda * sin_phi;
            z = (h + (1 - e_sq) * N) * sin_lambda;
        }

        // Converts the Earth-Centered Earth-Fixed (ECEF) coordinates (x, y, z) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public void EcefToEnu(double x, double y, double z,
                                     double lat0, double lon0, double h0,
                                     out double xEast, out double yNorth, out double zUp)
        {
            // Convert to radians in notation consistent with the paper:
            var lambda = DegreeToRadian(lat0);
            var phi = DegreeToRadian(lon0);
            var s = Math.Sin(lambda);
            var N = a / Math.Sqrt(1 - e_sq * s * s);

            var sin_lambda = Math.Sin(lambda);
            var cos_lambda = Math.Cos(lambda);
            var cos_phi = Math.Cos(phi);
            var sin_phi = Math.Sin(phi);

            double x0 = (h0 + N) * cos_lambda * cos_phi;
            double y0 = (h0 + N) * cos_lambda * sin_phi;
            double z0 = (h0 + (1 - e_sq) * N) * sin_lambda;

            double xd, yd, zd;
            xd = x - x0;
            yd = y - y0;
            zd = z - z0;

            // This is the matrix multiplication
            xEast = -sin_phi * xd + cos_phi * yd;
            yNorth = -cos_phi * sin_lambda * xd - sin_lambda * sin_phi * yd + cos_lambda * zd;
            zUp = cos_lambda * cos_phi * xd + cos_lambda * sin_phi * yd + sin_lambda * zd;
        }

        // Converts the geodetic WGS-84 coordinated (lat, lon, h) to 
        // East-North-Up coordinates in a Local Tangent Plane that is centered at the 
        // (WGS-84) Geodetic point (lat0, lon0, h0).
        public void GeodeticToEnu(double lat, double lon, double h,
                                         double lat0, double lon0, double h0,
                                         out double xEast, out double yNorth, out double zUp)
        {
            double x, y, z;
            GeodeticToEcef(lat, lon, h, out x, out y, out z);
            EcefToEnu(x, y, z, lat0, lon0, h0, out xEast, out yNorth, out zUp);
        }

        public double DMSToDecimal(string dms)
        {
            // format: 6024.1234567 and 0519.12345
            double degrees = double.Parse(dms.Substring(0, 2));
            double minutes = double.Parse(dms.Remove(0, 2));

            return degrees + (minutes / 60);
        }

        public Tuple<Vector2, Vector2> GetCurrentLatLonArea(double lat, double lon)
        {
            //offsets in meters
            double dn = Config.Instance.conf.DataSettings["LatitudeArea"];
            double de = Config.Instance.conf.DataSettings["LongitudeArea"];

            return new Tuple<Vector2, Vector2>(
                OffsetLatLonByMeter(lat, lon, -dn, -de),
                OffsetLatLonByMeter(lat, lon, dn, de)
            );
        }

        private Vector2 OffsetLatLonByMeter(double lat, double lon, double dn, double de)
        {
            // Code retrieved from https://gis.stackexchange.com/a/2980

            // Earth’s radius, sphere
            double R = 6378137;

            // Coordinate offsets in radians
            double dLat = dn / R;
            double dLon = de / (R * Math.Cos(Math.PI * lat / 180));

            // OffsetPosition, decimal degrees
            double latO = lat + dLat * 180 / Math.PI;
            double lonO = lon + dLon * 180 / Math.PI;

            return new Vector2((float)latO, (float)lonO);
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}

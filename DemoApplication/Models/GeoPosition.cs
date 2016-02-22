using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mindream.Attributes;

namespace DemoApplication.Models
{
    /// <summary>
    ///     This enumeration is used to convert latitude and longiture to string.
    /// </summary>
    public enum GeoPositionDisplayMode
    {
        /// <summary>
        ///     Display the geoposition with format DD.ddddd
        /// </summary>
        DegreeDecimal,

        /// <summary>
        ///     Display the geoposition with format DD°MM.mmmm
        /// </summary>
        DegreeMinuteDecimal,

        /// <summary>
        ///     Display the geoposition with format DD°MM'SS''
        /// </summary>
        DegreeMinuteSecond
    }

    /// <summary>
    ///     This class defines a geo position.
    /// </summary>
    /// <example>
    ///     GeoPosition lFirstPosition = new OpenTK.Vector3d(12, 13, 5);
    ///     GeoPosition lSecondPosition = new GeoPosition(10, 1, 100);
    ///     GeoPosition lResult = (OpenTK.Vector3d)lFirstPosition + (OpenTK.Vector3d)lSecondPosition;
    ///     lResult = OpenTK.Vector3d.Subtract(lFirstPosition, lSecondPosition);
    ///     Console.WriteLine(lResult);
    /// </example>
    /// <!-- DPE -->
    public class GeoPosition : IEquatable<GeoPosition>, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        ///     Stores te
        /// </summary>
        private const double DOUBLE_PREC = 1.0e-08;

        #endregion // Fields.

        #region Events

#pragma warning disable 0067

        /// <summary>
        ///     Event raised when a property is modified.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore 0067

        #endregion // Events.

        #region Constants

        /// <summary>
        ///     Const Factor for easily passing from radians to degrees
        /// </summary>
        public const double TO_DEG_FACTOR = 180/Math.PI;

        /// <summary>
        ///     Const Factor for easily passing from degrees to radians
        /// </summary>
        public const double TO_RAD_FACTOR = Math.PI/180;

        /// <summary>
        ///     Const earth radius in kilometers.
        /// </summary>
        public const double EARTH_RADIUS_IN_KM = 6378.1;

        /// <summary>
        ///     Const earth radius in meters.
        /// </summary>
        public const double EARTH_RADIUS_IN_METER = 6378137.0;

        /// <summary>
        ///     Const double comparison tolerance.
        /// </summary>
        public const double TOLERANCE = 0.000001;

        #endregion Constants

        #region Properties

        /// <summary>
        ///     Gets or sets the latitude (in degrees).
        /// </summary>
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the longitude (in degrees).
        /// </summary>
        public double Longitude
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the altitude (in meters).
        /// </summary>
        public double Altitude
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeoPosition" /> class.
        /// </summary>
        public GeoPosition()
        {
            this.Latitude = double.NaN;
            this.Longitude = double.NaN;
            this.Altitude = double.NaN;
        }

        /// <summary>
        ///     Initialize a new instance of the <see cref="GeoPosition" /> class.
        /// </summary>
        /// <param name="pLatitude">The latitude.</param>
        /// <param name="pLongitude">The longitude.</param>
        /// <param name="pAltitude">The altitude.</param>
        public GeoPosition(double pLatitude, double pLongitude, double pAltitude)
        {
            this.Latitude = pLatitude;
            this.Longitude = pLongitude;
            this.Altitude = pAltitude;
        }

        #endregion // Constructors.

        #region Methods

        #region Methods Geodetic

        #region Methods Bearing

        /// <summary>
        ///     Compute the initial bearing in degrees (0/360) from north between "this" and an ending position in degrees along a
        ///     great circle
        /// </summary>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the initial bearing in degrees between 0/360</returns>
        public double InitialBearingTo(GeoPosition pEndPoint)
        {
            // Convert to radians.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;

            // Compute the delta in longitude
            var lDLon = (pEndPoint.Longitude - this.Longitude)*TO_RAD_FACTOR;

            // Compute the bearing
            var lY = Math.Sin(lDLon)*Math.Cos(lRadLat2);
            var lX = Math.Cos(lRadLat1)*Math.Sin(lRadLat2) -
                     Math.Sin(lRadLat1)*Math.Cos(lRadLat2)*Math.Cos(lDLon);

            // Finalize and back to degrees.
            var lBearing = Math.Atan2(lY, lX)*TO_DEG_FACTOR;

            // Convert from -180/180 to 0/360 degrees and return.
            return (lBearing + 360.0)%360;
        }

        /// <summary>
        ///     Compute the initial bearing in degrees (0/360) from north between two Geodetic positions in degrees along a great
        ///     circle
        /// </summary>
        /// <param name="pStartPoint">The starting Geodetic position in degrees</param>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the initial bearing in degrees between 0/360</returns>
        [StaticMethodComponent("Geographic")]
        public static double InitialBearing(GeoPosition pStartPoint, GeoPosition pEndPoint)
        {
            return pStartPoint.InitialBearingTo(pEndPoint);
        }

        /// <summary>
        ///     Compute the final bearing in degrees (0/360) from north between "this" and an ending position in degrees along a
        ///     great circle
        ///     The final bearing will differ from the initial bearing by varying degrees according to distance and latitude.
        /// </summary>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the final bearing in degree between 0/360</returns>
        public double FinalBearing(GeoPosition pEndPoint)
        {
            // Get initial bearing from destination point to this point & reverse it by adding 180°
            return (pEndPoint.InitialBearingTo(this) + 180)%360;
        }

        /// <summary>
        ///     Compute the final bearing in degrees (0/360) from north between two Geodetic positions in degrees along a great
        ///     circle
        ///     The final bearing will differ from the initial bearing by varying degrees according to distance and latitude.
        /// </summary>
        /// <param name="pStartPoint">The starting Geodetic position in degrees</param>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the final bearing in degree between 0/360</returns>
        public static double FinalBearing(GeoPosition pStartPoint, GeoPosition pEndPoint)
        {
            return pStartPoint.FinalBearing(pEndPoint);
        }

        /// <summary>
        ///     Compute the constant bearing in degrees (0/360) between "this" and an ending position in degrees along a Rhumb
        ///     line.
        /// </summary>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the constant bearing in degree between 0/360</returns>
        public double RhumbBearingTo(GeoPosition pEndPoint)
        {
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;

            // Compute the delta in longitude
            var lDLon = (pEndPoint.Longitude - this.Longitude)*TO_RAD_FACTOR;

            var lDPhi = Math.Log(Math.Tan(lRadLat2/2 + Math.PI/4)/Math.Tan(lRadLat1/2 + Math.PI/4));
            if
                (Math.Abs(lDLon) > Math.PI)
            {
                if
                    (lDLon > 0)
                {
                    lDLon = -(2*Math.PI - lDLon);
                }
                else
                {
                    lDLon = 2*Math.PI + lDLon;
                }
            }

            var lBearing = Math.Atan2(lDLon, lDPhi)*TO_DEG_FACTOR;

            return (lBearing + 360)%360;
        }

        /// <summary>
        ///     Compute the constant bearing in degrees (0/360) between two Geodetic positions in degrees along a Rhumb line.
        /// </summary>
        /// <param name="pStartPoint">The starting Geodetic position in degrees</param>
        /// <param name="pEndPoint">The ending Geodetic position in degrees</param>
        /// <returns>Return the constant bearing in degree between 0/360</returns>
        public static double RhumbBearing(GeoPosition pStartPoint, GeoPosition pEndPoint)
        {
            return pStartPoint.RhumbBearingTo(pEndPoint);
        }

        #endregion Methods Bearing

        #region Methods MidPoint

        /// <summary>
        ///     Compute the mid point in degrees along the great circle between "this" and an ending position in degrees
        ///     NOTE: this half-way point takes the highest altitude of the two endpoints
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>The mid point in degrees with the highest altitude of the two endpoints in meters</returns>
        public GeoPosition MidPointTo(GeoPosition pEndPoint)
        {
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            // Mid point computation
            var lDeltaLon = lRadLon2 - lRadLon1;
            var lBx = Math.Cos(lRadLat2)*Math.Cos(lDeltaLon);
            var lBy = Math.Cos(lRadLat2)*Math.Sin(lDeltaLon);
            var lLat3 = Math.Atan2(Math.Sin(lRadLat1) + Math.Sin(lRadLat2), Math.Sqrt((Math.Cos(lRadLat1) + lBx)*(Math.Cos(lRadLat1) + lBx) + lBy*lBy));
            var lLon3 = lRadLon1 + Math.Atan2(lBy, Math.Cos(lRadLat1) + lBx);

            // Normalize to -180/180 degrees.
            lLon3 = (lLon3 + 3*Math.PI)%(2*Math.PI) - Math.PI;

            // Back to degrees and return the geo position.
            return new GeoPosition(lLat3*TO_DEG_FACTOR, lLon3*TO_DEG_FACTOR, Math.Max(this.Altitude, pEndPoint.Altitude));
        }

        /// <summary>
        ///     Compute the mid point in degrees along the great circle between two geo positions in degrees
        ///     NOTE: this half-way point takes the highest altitude of the two endpoints
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>The mid point in degrees with the highest altitude of the two endpoints in meters</returns>
        public static GeoPosition ComputeMidPoint(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.MidPointTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the mid point between "this" and an ending position in 2D
        ///     preserving angles for a square plate projection.
        ///     NOTE: this half-way point takes the mid altitude of the two endpoints
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>The mid point in degrees with the mid altitude in meters</returns>
        public GeoPosition MidPoint2DTo(GeoPosition pEndPoint)
        {
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            var lLat3 = (lRadLat2 + lRadLat1)/2.0;
            var lLon3 = (lRadLon2 + lRadLon1)/2.0;
            var lAlt3 = (Math.Max(this.Altitude, pEndPoint.Altitude) + Math.Min(this.Altitude, pEndPoint.Altitude))/2;

            // Back to degrees and return the geo position.
            return new GeoPosition(lLat3*TO_DEG_FACTOR, lLon3*TO_DEG_FACTOR, lAlt3);
        }

        /// <summary>
        ///     Compute the mid point between two geo positions in 2D
        ///     preserving angles for a square plate projection.
        ///     NOTE: this half-way point takes the mid altitude of the two endpoints
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>The mid point in degrees with the mid altitude in meters</returns>
        public static GeoPosition ComputeMidPoint2D(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.MidPoint2DTo(pEndPoint2);
        }

        #endregion Methods MidPoint

        #region Methods Distance

        /// <summary>
        ///     Compute the distance between "this" and an endpoint using the 'Haversine' formula
        ///     with an inaccuracy of 3m in 1km
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between "this" and the endpoint in Meters</returns>
        public double DistanceTo(GeoPosition pEndPoint)
        {
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            // Haversine formula 
            var lDLatInRad = lRadLat2 - lRadLat1;
            var lDLonInRad = lRadLon2 - lRadLon1;

            var lSinHalfDLat = Math.Sin(lDLatInRad/2);
            var lSinHalfDLong = Math.Sin(lDLonInRad/2);
            var lA = lSinHalfDLat*lSinHalfDLat +
                     lSinHalfDLong*lSinHalfDLong*Math.Cos(lRadLat1)*Math.Cos(lRadLat2);
            var lC = 2.0*Math.Atan2(Math.Sqrt(lA), Math.Sqrt(1 - lA));

            return EARTH_RADIUS_IN_METER*lC; // Check Paris -> Marseil ~650000m.
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions in degrees using the 'Haversine' formula
        ///     with an inaccuracy of 3m in 1km
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public static double Distance(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.DistanceTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between "this" and an endpoint using the 'Haversine' formula
        ///     with an inaccuracy of 3m in 1km
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between "this" and the endpoint in KM</returns>
        public double DistanceInKmTo(GeoPosition pEndPoint)
        {
            return this.DistanceTo(pEndPoint)/1000.0; // Check Paris -> Marseille ~650km.
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions using the 'Haversine' formula
        ///     with an inaccuracy of 3m in 1km
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in KM</returns>
        public static double DistanceInKm(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.DistanceInKmTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions using the 'Vincenty' formula
        ///     giving results accurate to within 1mm. (Slower)
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public double DistanceAccurateTo(GeoPosition pEndPoint)
        {
            const double A = 6378137;
            const double B = 6356752.314245;
            const double F = 1/298.257223563; // WGS-84 pEllipsoid params

            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            var lL = lRadLon2 - lRadLon1;
            var lU1 = Math.Atan((1 - F)*Math.Tan(lRadLat1));
            var lU2 = Math.Atan((1 - F)*Math.Tan(lRadLat2));
            var lSinU1 = Math.Sin(lU1);
            var lCosU1 = Math.Cos(lU1);
            var lSinU2 = Math.Sin(lU2);
            var lCosU2 = Math.Cos(lU2);

            var lAmbda = lL;
            double lAmbdaP;
            double lIterLimit = 100;
            double lSinLambda = Math.Sin(lAmbda), lCosLambda = Math.Cos(lAmbda);
            var lSinSigma = Math.Sqrt(lCosU2*lSinLambda*(lCosU2*lSinLambda) + (lCosU1*lSinU2 - lSinU1*lCosU2*lCosLambda)*(lCosU1*lSinU2 - lSinU1*lCosU2*lCosLambda));

            if (Math.Abs(lSinSigma) < TOLERANCE)
            {
                return 0; // co-incident points
            }

            double lCosSigma;
            double lSigma;
            double lCosSqAlpha;
            double lCos2SigmaM;

            do
            {
                lSinLambda = Math.Sin(lAmbda);
                lCosLambda = Math.Cos(lAmbda);
                lSinSigma = Math.Sqrt(lCosU2*lSinLambda*(lCosU2*lSinLambda) + (lCosU1*lSinU2 - lSinU1*lCosU2*lCosLambda)*(lCosU1*lSinU2 - lSinU1*lCosU2*lCosLambda));

                if (Math.Abs(lSinSigma) < TOLERANCE)
                {
                    return 0; // co-incident points
                }

                lCosSigma = lSinU1*lSinU2 + lCosU1*lCosU2*lCosLambda;
                lSigma = Math.Atan2(lSinSigma, lCosSigma);
                var lSinAlpha = lCosU1*lCosU2*lSinLambda/lSinSigma;
                lCosSqAlpha = 1 - lSinAlpha*lSinAlpha;
                lCos2SigmaM = lCosSigma - 2*lSinU1*lSinU2/lCosSqAlpha;
                if (double.IsNaN(lCos2SigmaM))
                {
                    lCos2SigmaM = 0; // equatorial line: CosSqAlpha=0 (§6)
                }

                var lC = F/16*lCosSqAlpha*(4 + F*(4 - 3*lCosSqAlpha));
                lAmbdaP = lAmbda;
                lAmbda = lL + (1 - lC)*F*lSinAlpha*(lSigma + lC*lSinSigma*(lCos2SigmaM + lC*lCosSigma*(-1 + 2*lCos2SigmaM*lCos2SigmaM)));
            }
            while (Math.Abs(lAmbda - lAmbdaP) > 1e-12 && --lIterLimit > 0);

            if (Math.Abs(lIterLimit) < TOLERANCE)
            {
                return double.NaN; // formula failed to converge
            }

            var lUSq = lCosSqAlpha*(A*A - B*B)/(B*B);
            var lA = 1 + lUSq/16384*(4096 + lUSq*(-768 + lUSq*(320 - 175*lUSq)));
            var lB = lUSq/1024*(256 + lUSq*(-128 + lUSq*(74 - 47*lUSq)));
            var lDeltaSigma = lB*lSinSigma*(lCos2SigmaM + lB/4*(lCosSigma*(-1 + 2*lCos2SigmaM*lCos2SigmaM) - lB/6*lCos2SigmaM*(-3 + 4*lSinSigma*lSinSigma)*(-3 + 4*lCos2SigmaM*lCos2SigmaM)));
            var lS = B*lA*(lSigma - lDeltaSigma);

            lS = Math.Round(lS, 3); // Round to 1mm precision

            return lS;
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions using the 'Vincenty' formula
        ///     giving results accurate to within 1mm. (Slower)
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public static double DistanceAccurate(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.DistanceAccurateTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between the two endpoints along a rhumb line.
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public double RhumbDistanceTo(GeoPosition pEndPoint)
        {
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            var lDLatInRad = lRadLat2 - lRadLat1;
            var lDLonInRad = Math.Abs(lRadLon2 - lRadLon1);

            var lDPhi = Math.Log(Math.Tan(lRadLat2/2 + Math.PI/4)/Math.Tan(lRadLat1/2 + Math.PI/4));

            double lQ;
            var lTemp = lDLatInRad/lDPhi;
            if (!double.IsInfinity(lTemp) && !double.IsNaN(lTemp))
            {
                lQ = lTemp;
            }
            else
            {
                lQ = Math.Cos(lRadLat1); // E-W line gives dPhi=0
            }

            // If dLon over 180° take shorter rhumb across anti-meridian:
            if (Math.Abs(lDLonInRad) > Math.PI)
            {
                if (lDLonInRad > 0)
                {
                    lDLonInRad = -(2*Math.PI - lDLonInRad);
                }
                else
                {
                    lDLonInRad = 2*Math.PI + lDLonInRad;
                }
            }

            var lDist = Math.Sqrt(lDLatInRad*lDLatInRad + lQ*lQ*lDLonInRad*lDLonInRad)*EARTH_RADIUS_IN_METER;

            return lDist;
        }

        /// <summary>
        ///     Compute the distance between the two endpoints along a rhumb line.
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public static double RhumbDistance(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.RhumbDistanceTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between the two endpoints along a rhumb line.
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in KM</returns>
        public double RhumbDistanceInKmTo(GeoPosition pEndPoint)
        {
            return this.RhumbDistanceTo(pEndPoint)/1000.0;
        }

        /// <summary>
        ///     Compute the distance between the two endpoints along a rhumb line.
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in KM</returns>
        public static double RhumbDistanceInKm(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.RhumbDistanceInKmTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between "this" and an ending position in 2D
        ///     dealing with geopositions like vectors computing the length of the line
        ///     segment connecting "this" and the ending position
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between "this" and an ending position in Meters</returns>
        public double Distance2DTo(GeoPosition pEndPoint)
        {
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            var lRadLat2 = pEndPoint.Latitude*TO_RAD_FACTOR;
            var lRadLon2 = pEndPoint.Longitude*TO_RAD_FACTOR;

            var lDLatInRad = lRadLat2 - lRadLat1;
            var lDLonInRad = lRadLon2 - lRadLon1;

            return Math.Sqrt(lDLatInRad*lDLatInRad + lDLonInRad*lDLonInRad)*EARTH_RADIUS_IN_METER;
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions in 2D
        ///     dealing with geopositions like vectors computing the length of the line
        ///     segment connecting "this" and the ending position
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public static double Distance2D(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.Distance2DTo(pEndPoint2);
        }

        /// <summary>
        ///     Compute the distance between "this" and an ending position in 2D
        ///     dealing with geopositions like vectors computing the length of the line
        ///     segment connecting "this" and the ending position
        /// </summary>
        /// <param name="pEndPoint">The ending geo position in degrees</param>
        /// <returns>Return the distance between "this" and an ending position in KM</returns>
        public double Distance2DInKmTo(GeoPosition pEndPoint)
        {
            return this.Distance2DTo(pEndPoint)/1000.0;
        }

        /// <summary>
        ///     Compute the distance between two Geodetic positions in 2D
        ///     dealing with geopositions like vectors computing the length of the line
        ///     segment connecting "this" and the ending position
        /// </summary>
        /// <param name="pEndpoint1">The starting geo position in degrees</param>
        /// <param name="pEndPoint2">The ending geo position in degrees</param>
        /// <returns>Return the distance between endpoints in Meters</returns>
        public static double Distance2DInKm(GeoPosition pEndpoint1, GeoPosition pEndPoint2)
        {
            return pEndpoint1.Distance2DInKmTo(pEndPoint2);
        }

        #endregion Methods Distance

        #region Methods DestinationPoint

        /// <summary>
        ///     Compute the destination Geodetic position in degrees from "this" along the great circle
        /// </summary>
        /// <param name="pBearing">The bearing in degrees between 0/360</param>
        /// <param name="pDistance">The distance to travel in Meters</param>
        /// <returns>The destination geo position in degrees at the "this" altitude in Meters</returns>
        public GeoPosition DestinationPointTo(double pBearing, double pDistance)
        {
            var lAngularDist = pDistance/EARTH_RADIUS_IN_METER; // Angular distance covered on earth’s surface in Radians

            // Radians needed.
            pBearing = pBearing*TO_RAD_FACTOR;
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;

            var lRadLat2 = Math.Asin(Math.Sin(lRadLat1)*Math.Cos(lAngularDist) +
                                     Math.Cos(lRadLat1)*Math.Sin(lAngularDist)*Math.Cos(pBearing));
            var lRadLon2 = lRadLon1 + Math.Atan2(Math.Sin(pBearing)*Math.Sin(lAngularDist)*Math.Cos(lRadLat1),
                Math.Cos(lAngularDist) - Math.Sin(lRadLat1)*Math.Sin(lRadLat2));

            lRadLon2 = (lRadLon2 + 3*Math.PI)%(2*Math.PI) - Math.PI; // Normalise to -180..+180º

            return new GeoPosition(lRadLat2*TO_DEG_FACTOR, lRadLon2*TO_DEG_FACTOR, this.Altitude);
        }

        /// <summary>
        ///     Compute the destination Geodetic position in degrees from a starting Geodetic position in degrees along the great
        ///     circle
        /// </summary>
        /// <param name="pStartPoint">The starting geo position in degrees</param>
        /// <param name="pBearing">The bearing in degrees between 0/360</param>
        /// <param name="pDistance">The distance to travel in Meters</param>
        /// <returns>The destination Geodetic position in degrees at the starting point altitude in Meters</returns>
        public static GeoPosition ComputeDestinationPoint(GeoPosition pStartPoint, double pBearing, double pDistance)
        {
            return pStartPoint.DestinationPointTo(pBearing, pDistance);
        }

        /// <summary>
        ///     Calculate the destination Geodetic position in degrees from "this" along the great circle
        ///     using the accurate vicenty formula after traveling a specified distance in meters, using a specified starting
        ///     bearing in degrees
        /// </summary>
        /// <param name="pInitialBearing">Initial bearing in degrees between 0/360</param>
        /// <param name="pDistance">Distance to travel in Meters</param>
        /// <returns>The destination Geodetic position in degrees</returns>
        public GeoPosition DestinationPointAccurateTo(double pInitialBearing, double pDistance)
        {
            // Do not care about the final bearing
            var lFinalBearing = new double();
            return this.CalculateEndingGlobalCoordinatesAccurate(Ellipsoid.WGS84, pInitialBearing, pDistance, ref lFinalBearing);
        }

        /// <summary>
        ///     Calculate the destination Geodetic position in degrees from a starting position in degrees along the great circle
        ///     using the accurate vicenty formula after traveling a specified distance in meters, using a specified starting
        ///     bearing in degrees
        /// </summary>
        /// <param name="pStartPoint">Starting positon in degrees</param>
        /// <param name="pInitialBearing">Initial bearing in degrees between 0/360</param>
        /// <param name="pDistance">Distance to travel in Meters</param>
        /// <returns>The destination Geodetic position in degrees using the vicenty formula</returns>
        public static GeoPosition ComputeDestinationPointAccurate(GeoPosition pStartPoint, double pInitialBearing, double pDistance)
        {
            return pStartPoint.DestinationPointAccurateTo(pInitialBearing, pDistance);
        }

        /// <summary>
        ///     Compute the destination Geodetic position in degrees from "this" along a Rhumb line
        ///     after traveling a specified distance in meters, using a constant bearing in degrees
        /// </summary>
        /// <param name="pBearing">Constant bearing in degrees between 0/360</param>
        /// <param name="pDistance">Distance to travel in Meters</param>
        /// <returns>The destination geo position in degrees</returns>
        public GeoPosition RhumbDestinationPointTo(double pBearing, double pDistance)
        {
            var lAngularDist = pDistance/EARTH_RADIUS_IN_METER; // Angular distance covered on earth’s surface
            // Radians needed.
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;
            pBearing = pBearing*TO_RAD_FACTOR;

            var lDLat = lAngularDist*Math.Cos(pBearing);
            // Nasty kludge to overcome ill-conditioned results around parallels of latitude:
            if (Math.Abs(lDLat) < 1e-10)
            {
                lDLat = 0; // dLat < 1 mm
            }

            var lRadLat2 = lRadLat1 + lDLat;
            var lDPhi = Math.Log(Math.Tan(lRadLat2/2 + Math.PI/4)/Math.Tan(lRadLat1/2 + Math.PI/4));
            double lQ;
            var lTemp = lDLat/lDPhi;
            if (!double.IsInfinity(lTemp) && !double.IsNaN(lTemp))
            {
                lQ = lTemp;
            }
            else
            {
                lQ = Math.Cos(lRadLat1); // E-W line gives dPhi=0
            }

            var lDLon = lAngularDist*Math.Sin(pBearing)/lQ;

            // check for some daft bugger going past the pole, normalise latitude if so
            if (Math.Abs(lRadLat2) > Math.PI/2)
            {
                if (lRadLat2 > 0)
                {
                    lRadLat2 = Math.PI - lRadLat2;
                }
                else
                {
                    lRadLat2 = -Math.PI - lRadLat2;
                }
            }

            var lRadLon2 = (lRadLon1 + lDLon + 3*Math.PI)%(2*Math.PI) - Math.PI;

            return new GeoPosition(lRadLat2*TO_DEG_FACTOR, lRadLon2*TO_DEG_FACTOR, this.Altitude);
        }

        /// <summary>
        ///     Compute the destination Geodetic position in degrees from a starting position in degrees along a Rhumb line
        ///     after traveling a specified distance in meters, using a constant bearing in degrees
        /// </summary>
        /// <param name="pStartPoint">Starting positon in degrees between</param>
        /// <param name="pBearing">Constant bearing in degrees between 0/360</param>
        /// <param name="pDistance">Distance to travel in Meters</param>
        /// <returns>The destination geo position in degrees</returns>
        public static GeoPosition ComputeRhumbDestinationPoint(GeoPosition pStartPoint, double pBearing, double pDistance)
        {
            return pStartPoint.RhumbDestinationPointTo(pBearing, pDistance);
        }

        /// <summary>
        ///     Calculate the destination and final bearing using the vicenty formula
        ///     after traveling a specified distance, and a specified starting bearing, for an initial location.
        ///     This is the solution to the direct geodetic problem.
        /// </summary>
        /// <param name="pEllipsoid">reference ellipsoid to use</param>
        /// <param name="pInitialBearing">starting bearing (degrees)</param>
        /// <param name="pDistance">distance to travel (meters)</param>
        /// <param name="pFinalBearing">(ref)bearing at destination (degrees)</param>
        /// <returns>The destination position in degrees using the Vicenty formula</returns>
        // ReSharper disable once RedundantAssignment
        private GeoPosition CalculateEndingGlobalCoordinatesAccurate(Ellipsoid pEllipsoid, double pInitialBearing, double pDistance, ref double pFinalBearing)
        {
            // Radians needed.
            pInitialBearing = pInitialBearing*TO_RAD_FACTOR;
            var lRadLat1 = this.Latitude*TO_RAD_FACTOR;
            var lRadLon1 = this.Longitude*TO_RAD_FACTOR;

            var a = pEllipsoid.SemiMajorAxis;
            var b = pEllipsoid.SemiMinorAxis;
            var lASquared = a*a;
            var lBSquared = b*b;
            var lF = pEllipsoid.Flattening;
            var lPhi1 = lRadLat1;
            var lAlpha1 = pInitialBearing;
            var lCosAlpha1 = Math.Cos(lAlpha1);
            var lSinAlpha1 = Math.Sin(lAlpha1);
            var lS = pDistance;
            var lTanU1 = (1.0 - lF)*Math.Tan(lPhi1);
            var lCosU1 = 1.0/Math.Sqrt(1.0 + lTanU1*lTanU1);
            var lSinU1 = lTanU1*lCosU1;

            // eq. 1
            var lSigma1 = Math.Atan2(lTanU1, lCosAlpha1);

            // eq. 2
            var lSinAlpha = lCosU1*lSinAlpha1;

            var lSin2Alpha = lSinAlpha*lSinAlpha;
            var lCos2Alpha = 1 - lSin2Alpha;
            var lUSquared = lCos2Alpha*(lASquared - lBSquared)/lBSquared;

            // eq. 3
            var lA = 1 + lUSquared/16384*(4096 + lUSquared*(-768 + lUSquared*(320 - 175*lUSquared)));

            // eq. 4
            var lB = lUSquared/1024*(256 + lUSquared*(-128 + lUSquared*(74 - 47*lUSquared)));

            // iterate until there is a negligible change in sigma
            var lSOverbA = lS/(b*lA);
            var lSigma = lSOverbA;
            double lSinSigma;
            var lPrevSigma = lSOverbA;
            double lSigmaM2;
            double lCosSigmaM2;
            double lCos2SigmaM2;

            for (;;)
            {
                // eq. 5
                lSigmaM2 = 2.0*lSigma1 + lSigma;
                lCosSigmaM2 = Math.Cos(lSigmaM2);
                lCos2SigmaM2 = lCosSigmaM2*lCosSigmaM2;
                lSinSigma = Math.Sin(lSigma);
                var lCosSignma = Math.Cos(lSigma);

                // eq. 6
                var lDeltaSigma = lB*lSinSigma*(lCosSigmaM2 + lB/4.0*(lCosSignma*(-1 + 2*lCos2SigmaM2)
                                                                      - lB/6.0*lCosSigmaM2*(-3 + 4*lSinSigma*lSinSigma)*(-3 + 4*lCos2SigmaM2)));

                // eq. 7
                lSigma = lSOverbA + lDeltaSigma;

                // break after converging to tolerance
                if (Math.Abs(lSigma - lPrevSigma) < 0.0000000000001)
                    break;

                lPrevSigma = lSigma;
            }

            lSigmaM2 = 2.0*lSigma1 + lSigma;
            lCosSigmaM2 = Math.Cos(lSigmaM2);
            lCos2SigmaM2 = lCosSigmaM2*lCosSigmaM2;

            var lCosSigma = Math.Cos(lSigma);
            lSinSigma = Math.Sin(lSigma);

            // eq. 8
            var lPhi2 = Math.Atan2(lSinU1*lCosSigma + lCosU1*lSinSigma*lCosAlpha1,
                (1.0 - lF)*Math.Sqrt(lSin2Alpha + Math.Pow(lSinU1*lSinSigma - lCosU1*lCosSigma*lCosAlpha1, 2.0)));

            // eq. 9
            // This fixes the pole crossing defect spotted by Matt Feemster.  When a path
            // passes a pole and essentially crosses a line of latitude twice - once in
            // each direction - the longitude calculation got messed up.  Using Atan2
            // instead of Atan fixes the defect.  The change is in the next 3 lines.
            //double tanLambda = sinSigma * sinAlpha1 / (cosU1 * cosSigma - sinU1*sinSigma*cosAlpha1);
            //double lambda = Math.Atan(tanLambda);
            var lAmbda = Math.Atan2(lSinSigma*lSinAlpha1, lCosU1*lCosSigma - lSinU1*lSinSigma*lCosAlpha1);

            // eq. 10
            var lC = lF/16*lCos2Alpha*(4 + lF*(4 - 3*lCos2Alpha));

            // eq. 11
            var lL = lAmbda - (1 - lC)*lF*lSinAlpha*(lSigma + lC*lSinSigma*(lCosSigmaM2 + lC*lCosSigma*(-1 + 2*lCos2SigmaM2)));

            // eq. 12
            var lAlpha2 = Math.Atan2(lSinAlpha, -lSinU1*lSinSigma + lCosU1*lCosSigma*lCosAlpha1);

            // build result
            pFinalBearing = lAlpha2*TO_DEG_FACTOR;
            var lNewLat = lPhi2*TO_DEG_FACTOR;
            var lNewLon = (lRadLon1 + lL)*TO_DEG_FACTOR;
            Canonicalize(ref lNewLat, ref lNewLon);

            return new GeoPosition(lNewLat, lNewLon, this.Altitude);
        }

        #endregion Methods DestinationPoint

        #endregion Methods Geodetic

        #region Methods IEquatable

        /// <summary>
        ///     Verifies if the given geo position is equal to the current one.
        /// </summary>
        /// <param name="pOther">The geo position to compare.</param>
        /// <returns>True if the geo positions components are equal, false otherwise.</returns>
        public bool Equals(GeoPosition pOther)
        {
            return this.Latitude.Equals(pOther.Latitude) && this.Longitude.Equals(pOther.Longitude) && this.Altitude.Equals(pOther.Altitude);
        }

        /// <summary>
        ///     Verifies if the given object is equal to the current one.
        /// </summary>
        /// <param name="pOther">The object to compare.</param>
        /// <returns>True if the geo positions components are equal, false otherwise.</returns>
        public override bool Equals(object pOther)
        {
            if (pOther is GeoPosition == false)
            {
                return false;
            }

            return this.Equals((GeoPosition) pOther);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var lHashCode = this.Latitude.GetHashCode();
                lHashCode = (lHashCode*397) ^ this.Longitude.GetHashCode();
                lHashCode = (lHashCode*397) ^ this.Altitude.GetHashCode();
                return lHashCode;
            }
        }

        #endregion Methods IEquatable

        #region Methods Utilities

        /// <summary>
        ///     Compute the difference between "this" and an endpoint position in degrees
        /// </summary>
        /// <param name="pEndPoint">The ending position in degrees</param>
        /// <returns>The lat/long/alt difference in degrees/degrees/meters</returns>
        public GeoPosition ComputeDiff(GeoPosition pEndPoint)
        {
            var lDiffLat = pEndPoint.Latitude - this.Latitude;
            var lDiffLon = pEndPoint.Longitude - this.Longitude;
            var lDiffAlt = pEndPoint.Altitude - this.Altitude;

            return new GeoPosition(lDiffLat, lDiffLon, lDiffAlt);
        }

        /// <summary>
        ///     Compute the difference between two geo positions in degrees
        /// </summary>
        /// <param name="pEndPoint1">The first geo position in degrees</param>
        /// <param name="pEndPoint2">The second geo position in degrees</param>
        /// <returns>The lat/long difference in radians</returns>
        public static GeoPosition ComputeDiff(GeoPosition pEndPoint1, GeoPosition pEndPoint2)
        {
            return pEndPoint1.ComputeDiff(pEndPoint2);
        }

        /// <summary>
        ///     Compute the center geo position in degree of a set of geo positions in degree
        /// </summary>
        /// <param name="pPositions">The set of geo positions in degree</param>
        /// <returns>The computed center in degree</returns>
        public static GeoPosition ComputeCenterUsingMinMax(IEnumerable<GeoPosition> pPositions)
        {
            // Find the minimal and maximal geo position in degrees first.
            var lMinMax = ComputeMinMax(pPositions);
            if
                (lMinMax != null)
            {
                return lMinMax.Item1.MidPoint2DTo(lMinMax.Item2);
            }
            return null;
        }

        /// <summary>
        ///     Compute the minimal and maximal Geodetic positions in degrees giving a set of geo position in degrees.
        /// </summary>
        /// <param name="pPositions">The set of geo position in degrees</param>
        /// <returns>The minimal and maximal geo positions in degrees as Tuple[Min, Max]</returns>
        public static Tuple<GeoPosition, GeoPosition> ComputeMinMax(IEnumerable<GeoPosition> pPositions)
        {
            var lMinLat = double.MaxValue;
            var lMinLon = double.MaxValue;
            var lMinAlt = double.MaxValue;
            var lMaxLat = double.MinValue;
            var lMaxLon = double.MinValue;
            var lMaxAlt = double.MinValue;
            var lMinLatUpdated = false;
            var lMinLonUpdated = false;
            var lMaxLatUpdated = false;
            var lMaxLonUpdated = false;

            foreach
                (var lPosition in pPositions)
            {
                if (lPosition.Altitude > lMaxAlt)
                {
                    lMaxAlt = lPosition.Altitude;
                }

                if (lPosition.Altitude < lMinAlt)
                {
                    lMinAlt = lPosition.Altitude;
                }

                if (lPosition.Latitude < lMinLat)
                {
                    lMinLat = lPosition.Latitude;
                    lMinLatUpdated = true;
                }

                if (lPosition.Longitude < lMinLon)
                {
                    lMinLon = lPosition.Longitude;
                    lMinLonUpdated = true;
                }

                if (lPosition.Latitude > lMaxLat)
                {
                    lMaxLat = lPosition.Latitude;
                    lMaxLatUpdated = true;
                }

                if (lPosition.Longitude > lMaxLon)
                {
                    lMaxLon = lPosition.Longitude;
                    lMaxLonUpdated = true;
                }
            }

            if
                (lMinLatUpdated &&
                 lMinLonUpdated &&
                 lMaxLatUpdated &&
                 lMaxLonUpdated)
            {
                return new Tuple<GeoPosition, GeoPosition>(new GeoPosition(lMinLat, lMinLon, lMinAlt),
                    new GeoPosition(lMaxLat, lMaxLon, lMaxAlt));
            }

            return null;
        }

        /// <summary>
        ///     Method testing if "this" is in an area determined by its min and max position defining that area
        ///     2D Version only as Altitude will not be taken into account.
        /// </summary>
        /// <param name="pMin">The bbox min</param>
        /// <param name="pMax">The bbox max</param>
        /// <returns>True if "this" is inside the boudning box determined by Min and Max, false otherwise</returns>
        public bool IsInBound(GeoPosition pMin, GeoPosition pMax)
        {
            if
                (pMin == null ||
                 pMax == null)
            {
                return false;
            }

            return this.Latitude > pMin.Latitude && this.Longitude > pMin.Longitude &&
                   this.Latitude < pMax.Latitude && this.Longitude < pMax.Longitude;
        }

        /// <summary>
        ///     Method testing if "this" is between two Geodetic positions in degrees
        /// </summary>
        /// <param name="pEndpoint1">The starting position in degrees</param>
        /// <param name="pEndpoint2">The ending position in degrees</param>
        /// <returns>True if "this" is between the endpoints, false otherwise</returns>
        public bool IsBetween(GeoPosition pEndpoint1, GeoPosition pEndpoint2)
        {
            if
                (pEndpoint1 == null ||
                 pEndpoint2 == null)
            {
                return false;
            }

            var lCrossproduct = (pEndpoint2.Longitude - pEndpoint1.Longitude)*(this.Latitude - pEndpoint1.Latitude) - (pEndpoint2.Latitude - pEndpoint1.Latitude)*(this.Longitude - pEndpoint1.Longitude);
            if (Math.Abs(lCrossproduct) > double.Epsilon)
            {
                return false;
            }

            var lDotproduct = (pEndpoint2.Latitude - pEndpoint1.Latitude)*(this.Latitude - pEndpoint1.Latitude) + (pEndpoint2.Longitude - pEndpoint1.Longitude)*(this.Longitude - pEndpoint1.Longitude);
            if (lDotproduct < 0)
            {
                return false;
            }

            var lSquaredlengthba = (this.Latitude - pEndpoint1.Latitude)*(this.Latitude - pEndpoint1.Latitude) + (this.Longitude - pEndpoint1.Longitude)*(this.Longitude - pEndpoint1.Longitude);
            if (lDotproduct > lSquaredlengthba)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Indicates whether the geo position contains Nan values.
        /// </summary>
        /// <returns>True if at least one of its elements is Nan</returns>
        public bool IsNan()
        {
            return double.IsNaN(this.Latitude) || double.IsNaN(this.Longitude) || double.IsNaN(this.Altitude);
        }

        /// <summary>
        ///     Indicates whether the geo position is in the geodetic bounds or not.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if
                (this.IsNan() == false)
            {
                // If in the bounds
                return this.Latitude >= -90.0 &&
                       this.Latitude <= 90.0 &&
                       this.Longitude >= -180.0 &&
                       this.Longitude <= 180.0;
            }

            return false;
        }

        /// <summary>
        ///     This method converts from degree decimal to degree minute second
        /// </summary>
        /// <param name="pDegreeDecimal">The degree decimal</param>
        /// <param name="pDegreePart">The degrees as result.</param>
        /// <param name="pMinutePart">The minutes as result.</param>
        /// <param name="pSecondPart">The seconds as result.</param>
        public static void ToDegreeMinuteSecond(double pDegreeDecimal, out int pDegreePart, out int pMinutePart, out double pSecondPart)
        {
            if (double.IsInfinity(pDegreeDecimal) || double.IsNaN(pDegreeDecimal))
            {
                pDegreePart = 15;
                pMinutePart = 3;
                pSecondPart = 77;
            }
            else
            {
                var lDegreeDecimal = (decimal) pDegreeDecimal;
                var lDegreePart = Math.Truncate(lDegreeDecimal);
                var lRest = Math.Abs(lDegreeDecimal - lDegreePart);
                var lMinuteDecimal = lRest*60.0m;
                var lMinutePart = Math.Truncate(lMinuteDecimal);
                lRest = lMinuteDecimal - lMinutePart;
                var lSecondPart = lRest*60.0m;

                pDegreePart = (int) lDegreePart;

                // Compensating the double imprecision.
                if (Math.Abs(60.0m - lSecondPart) > (decimal) DOUBLE_PREC)
                {
                    pMinutePart = (int) lMinutePart;
                    pSecondPart = (double) lSecondPart;
                }
                else
                {
                    pMinutePart = (int) lMinutePart + 1;
                    pSecondPart = 0.0;
                }
            }
        }

        /// <summary>
        ///     This method converts from degree minute second to decimal angle.
        /// </summary>
        /// <param name="pDegree">The degree.</param>
        /// <param name="pMinute">The minute.</param>
        /// <param name="pSecond">The second.</param>
        /// <returns>The decimal angle.</returns>
        public static double FromDegreeMinuteSecond(int pDegree, int pMinute, double pSecond)
        {
            const decimal C_MINUTE_FACTOR = 1/60.0m;
            const decimal C_SECOND_FACTOR = 1/3600.0m;

            decimal lAngleSign = Math.Sign(pDegree);
            decimal lDegree = pDegree;
            decimal lMinute = pMinute;
            var lSecond = (decimal) pSecond;
            var lDecimalDegree = lDegree + lAngleSign*lMinute*C_MINUTE_FACTOR + lAngleSign*lSecond*C_SECOND_FACTOR;
            return (double) lDecimalDegree;
        }

        /// <summary>
        ///     This method converts from degree decimal to degree minute decimal
        /// </summary>
        /// <param name="pDegreeDecimal">The degree decimal</param>
        /// <param name="pDegreePart">The degrees as result.</param>
        /// <param name="pMinutePart">The minutes as result.</param>
        public static void ToDegreeMinuteDecimal(double pDegreeDecimal, out int pDegreePart, out double pMinutePart)
        {
            if (double.IsInfinity(pDegreeDecimal) || double.IsNaN(pDegreeDecimal))
            {
                pDegreePart = 15;
                pMinutePart = 3;
            }
            else
            {
                var lDegreeDecimal = (decimal) pDegreeDecimal;
                var lDegreePart = Math.Truncate(lDegreeDecimal);
                var lRest = Math.Abs(lDegreeDecimal - lDegreePart);
                var lMinutePart = lRest*60.0m;

                pDegreePart = (int) lDegreePart;
                pMinutePart = (double) lMinutePart;
            }
        }

        /// <summary>
        ///     This method converts from degree minute decimal to decimal angle.
        /// </summary>
        /// <param name="pDegree">The degree.</param>
        /// <param name="pMinute">The minute.</param>
        /// <returns>The decimal angle.</returns>
        public static double FromDegreeMinuteDecimal(int pDegree, double pMinute)
        {
            const decimal C_MINUTE_FACTOR = 1/60.0m;

            decimal lAngleSign = Math.Sign(pDegree);
            decimal lDegree = pDegree;
            var lMinute = (decimal) pMinute;
            var lDecimalDegree = lDegree + lAngleSign*lMinute*C_MINUTE_FACTOR;
            return (double) lDecimalDegree;
        }

        /// <summary>
        ///     Convert the geo position to string.
        /// </summary>
        /// <returns>The string describing the geo position.</returns>
        public override string ToString()
        {
            return string.Format(@"Lat {0:0.00} - Lon {1:0.00} - Alt {2:0.00}", this.Latitude, this.Longitude, this.Altitude);
        }

        #endregion Methods Utilities

        #region Methods Converters

        /// <summary>
        ///     Transform "this" from degrees to a geo position in radians.
        /// </summary>
        /// <returns>The geo position in radians</returns>
        public GeoPosition ConvertToRadians()
        {
            return new GeoPosition(this.Latitude*TO_RAD_FACTOR, this.Longitude*TO_RAD_FACTOR, this.Altitude);
        }

        /// <summary>
        ///     Transform "this" from radians to a geo position in degrees.
        /// </summary>
        /// <returns>The geo position in degrees</returns>
        public GeoPosition ConvertToDegrees()
        {
            // Degrees needed.
            return new GeoPosition(this.Latitude*TO_DEG_FACTOR, this.Longitude*TO_DEG_FACTOR, this.Altitude);
        }

        /// <summary>
        ///     Transform a geo position in degree into a geo position in radians.
        /// </summary>
        /// <param name="pPositionInDegree">The geo position in degree</param>
        /// <returns>The geo position in radians</returns>
        public static GeoPosition DegToRad(GeoPosition pPositionInDegree)
        {
            return pPositionInDegree.ConvertToRadians();
        }

        /// <summary>
        ///     Transform a geo position in degree into a geo position in radians.
        /// </summary>
        /// <param name="pLatInDegree">The latitude in degree</param>
        /// <param name="pLonInDegree">The longitude in degree</param>
        /// <param name="pAltitude">The altitude</param>
        /// <returns>The geo position in radians</returns>
        public static GeoPosition DegToRad(double pLatInDegree, double pLonInDegree, double pAltitude)
        {
            return new GeoPosition(pLatInDegree, pLonInDegree, pAltitude).ConvertToRadians();
        }

        /// <summary>
        ///     Transform a geo position in radians into a geo position in degrees.
        /// </summary>
        /// <param name="pPositionInRadian">The geo position in radians</param>
        /// <returns>The geo position in degrees</returns>
        public static GeoPosition RadToDeg(GeoPosition pPositionInRadian)
        {
            return pPositionInRadian.ConvertToDegrees();
        }

        /// <summary>
        ///     Transform a geo position in radians into a geo position in degrees.
        /// </summary>
        /// <param name="pLatInRadian">The latitude in radians</param>
        /// <param name="pLonInRadian">The longitude in radians</param>
        /// <param name="pAltitude">The altitude</param>
        /// <returns>The geo position in degrees</returns>
        public static GeoPosition RadToDeg(double pLatInRadian, double pLonInRadian, double pAltitude)
        {
            return new GeoPosition(pLatInRadian, pLonInRadian, pAltitude).ConvertToDegrees();
        }

        /// <summary>
        ///     Convert a value between -180/180 degrees to a value between 0/360 degrees
        /// </summary>
        /// <param name="pValueBetweenMinus180180">The value between -180/180 degrees</param>
        /// <returns>The value between 0/360 degrees</returns>
        public static double ConvertFrom_Minus180_180_To_0_360(double pValueBetweenMinus180180)
        {
            // Convert from -180/180 to 0/360 degrees and return.
            return (pValueBetweenMinus180180 + 360.0)%360;
        }

        /// <summary>
        ///     Canonicalize the current latitude and longitude values such that:
        ///     -90 [ latitude ] +90
        ///     -180 [ longitude ] +180
        /// </summary>
        /// <param name="pLatitude">The latitude to clamp in degrees</param>
        /// <param name="pLongitude">The longitude to clamp in degrees</param>
        public static void Canonicalize(ref double pLatitude, ref double pLongitude)
        {
            var lAtitude = pLatitude;
            var lOngitude = pLongitude;

            lAtitude = (lAtitude + 180)%360;
            if
                (lAtitude < 0)
            {
                lAtitude += 360;
            }

            lAtitude -= 180;

            if
                (lAtitude > 90)
            {
                lAtitude = 180 - lAtitude;
                lOngitude += 180;
            }
            else if
                (lAtitude < -90)
            {
                lAtitude = -180 - lAtitude;
                lOngitude += 180;
            }

            lOngitude = (lOngitude + 180)%360;
            if
                (lOngitude <= 0)
            {
                lOngitude += 360;
            }

            lOngitude -= 180;

            // Refresh
            pLatitude = lAtitude;
            pLongitude = lOngitude;
        }

        #endregion Methods Converters

        #endregion Methods
    }
}
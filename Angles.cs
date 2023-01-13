using System;

namespace Calculator
{
    class Angles
    {
        public enum units
        {
            RADIANS,  
            GRADIANS
        }

        public class Converter
        {
            public static double degrees(double angle, units angleUnit)
            {
                if (angleUnit == units.RADIANS)
                    return angle * 180 / Math.PI;
                else if (angleUnit == units.GRADIANS)
                    return angle * 9 / 10;
                else
                {
                    Exception error = new Exception("Invalid parameters");
                    throw error;
                }
            }

            public static double radians(double angle, units angleUnit)
            {
               
                if (angleUnit == units.RADIANS)
                    return angle;

                return degrees(angle, angleUnit) * Math.PI / 180;
            }

            public static double gradians(double angle, units angleUnit)
            {
             
                if (angleUnit == units.GRADIANS)
                    return angle;

                return degrees(angle, angleUnit) * 10 / 9;
            }
        }
    }
}

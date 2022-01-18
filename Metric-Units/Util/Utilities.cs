using System;

namespace MetricUnits.Util
{
    internal struct Utilities
    {
        public static bool isdigit(char c)
        {
            switch (c)
            {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return true;
            default:
                return false;
            }
        }
        public static bool isnumchar(char c)
        {
            return isdigit(c) || c == '.' || c == '-';
        }
    }

    internal class Unit
    {
        public enum ID
        {
            NONE,
            METERS,
            FEET,
        }

        private const double ONE_FOOT_IN_METERS = 0.3048;

        public static double? Convert(double value, ID in_unit, ID out_unit)
        {
            if (out_unit == ID.NONE)
                return null;

            if (in_unit == out_unit || in_unit == ID.NONE)
                return null;

            if (in_unit == ID.FEET && out_unit == ID.METERS)
            {
                value *= ONE_FOOT_IN_METERS;
            }
            if (in_unit == ID.METERS && out_unit == ID.FEET)
            {
                value /= ONE_FOOT_IN_METERS;
            }
            
            return Math.Round(value, 1);
        }

        public static ID FromString(string s)
        {
            if (s.Equals("feet", StringComparison.OrdinalIgnoreCase))
                return ID.FEET;
            if (s.Equals("meters", StringComparison.OrdinalIgnoreCase))
                return ID.METERS;
            return ID.NONE;
        }
    }

}

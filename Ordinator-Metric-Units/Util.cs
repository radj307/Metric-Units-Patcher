using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricUnits
{
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

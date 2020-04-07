using System;
using System.Collections.Generic;
using DataAccess.Model;
using OxyPlot;

namespace LoadDevelopmentUI.Helper
{
    public static class VelocityNodeCalc
    {
        private struct Point
        {
            public float x;
            public float y;
        };

        static private float computeSlope(LoadString string1, LoadString string2)
        {
            return (string2.AvgVelocity - string1.AvgVelocity) / (string2.PowderCharge - string1.PowderCharge);
        }

        static public List<DataPoint> CalculateVelocityNodes(List<DataAccess.Model.LoadString> shotStrings)
        {
            var velocityNodes = new List<DataPoint>();
            var slopes = new Dictionary<DataPoint, float>();
            for (int i = 0; i < shotStrings.Count; ++i)
            {
                var string1 = shotStrings[i];
                ++i;
                if (i >= shotStrings.Count)
                    break;

                var string2 = shotStrings[i];

                if (string1.AvgVelocity <= 0 || string2.AvgVelocity <= 0)
                    continue;

                Point midPoint = findMidPoint(string1.PowderCharge, string1.AvgVelocity,
                    string2.PowderCharge, string2.AvgVelocity);

                slopes.Add(new DataPoint(midPoint.x, midPoint.y), computeSlope(string1, string2));
            }

            foreach (var key in slopes)
            {
                if (Math.Abs(key.Value) <= 10)
                    velocityNodes.Add(key.Key);
            }

            return velocityNodes;
        }

        private static Point findMidPoint(float x1, float y1, float x2, float y2)
        {
            var p = new Point();

            p.x = (x1 + x2) / 2;
            p.y = (y1 + y2) / 2;

            return p;
        }
    }
}

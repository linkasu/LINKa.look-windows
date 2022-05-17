using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LinkaWPF
{
    class MousePointCalcs
    {
        public static double CalcMiddleLength (List<Vector> points)
        {
            double sum = 0;
            for (int i = 0; i < points.Count-2; i++)
            {
                sum += (points[i] - points[i + 1]).Length;
            }
            return sum / (points.Count - 1);
        }
        public static Vector MiddleValue(List<Vector> points)
        {
            var sum = new Vector(0, 0);
            foreach (var point in points)
            {
                sum += point;
            }
            return sum / points.Count;
        }
    }
}

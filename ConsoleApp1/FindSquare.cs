using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FindSquare()
    {
        public double[] solve(double a, double b, double c, double e = 0.00001)
        {
            if (double.IsNaN(a))
                throw new ArgumentException("a is not a number");
            if (double.IsNaN(b))
                throw new ArgumentException("b is not a number");
            if (double.IsNaN(c))
                throw new ArgumentException("c is not a number");

            if (Math.Abs(a) <= e)
                throw new ArgumentException("a должно быть не равно 0");

            var D = (b * b) - (4 * (a * c));
            if (D < -e)
                return [];

            if (Math.Abs(D) <= e)
                return [-b/(2*a), -b/(2*a)];

            if (D > e)
                return [(-b + Math.Sqrt(D)) / (2 * a), (-b - Math.Sqrt(D)) / (2 * a)];

            return [];
        }
    }
}

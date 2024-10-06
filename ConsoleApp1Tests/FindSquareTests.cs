using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace ConsoleApp1.Tests
{
    [TestClass()]
    public class FindSquareTests
    {

        [TestMethod()]
        public void solveTest()
        {
            var tests = new[]
            {
               new { name = "x^2+1 = 0", a = 1.0, b = 0.0, c = 1.0, want = (object)Array.Empty<double>() },
               new { name = "x^2-1 = 0", a = 1.0, b = 0.0, c = -1.0, want = (object)new double[] { 1, -1 } },
               new { name = "x^2+2x+1 = 0", a = 1.0, b = 2.0, c = 1.0, want = (object)new double[] { -1, -1 } },
               new { name = "x^2-3x+2 = 0", a = 1.0, b = -3.0, c = 2.0, want = (object)new double[] { 2, 1 } },
               new { name = "c check is NaN", a = 1.0, b = 0.0, c = Math.Sqrt(-4), want = (object)new ArgumentException("c is not a number") },
               new { name = "b check is NaN", a = 1.0, b = Math.Sqrt(-4), c = 0.0, want = (object)new ArgumentException("b is not a number") },
               new { name = "a check is NaN", a = Math.Sqrt(-4), b = 0.0, c = 0.0, want = (object)new ArgumentException("a is not a number") }
            };

            //Arrange
            var square = new FindSquare();

            foreach (var test in tests)
            {
                try
                {
                    //Act
                    var result = square.solve(test.a, test.b, test.c, double.Parse("1E-5"));

                    //Assert
                    CollectionAssert.AreEqual(test.want as double[], result);
                }
                catch (Exception ex)
                {
                    //Assert
                    Assert.AreEqual((test.want as Exception)?.Message, ex.Message);
                }
            }
        }
    }
}
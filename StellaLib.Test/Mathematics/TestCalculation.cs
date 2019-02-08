using NUnit.Framework;
using StellaLib.Mathematics;

namespace StellaLib.Test.Mathematics
{
    [TestFixture]
    public class TestCalculation
    {
        [TestCase(new long[]{1,100,200,300,400 },ExpectedResult=200)]
        [TestCase(new long[]{1,200,300,400 }    ,ExpectedResult=250)]
        [TestCase(new long[]{-500,-100,-10,50,68},ExpectedResult=-10)]
        [TestCase(new long[]{-1000,-500,-100,-10,50,68},ExpectedResult=-55)]
        public long Median(long[] values)
        {
            return Calculation.Median(values);
        }

        [TestCase(new long[]{-30002724785, -39002725445,-597026527710}, -34502725115,-39002725445,-318014626577.5)]
        [TestCase(new long[]{464,2424,64842}, 1444,2424, 33633)]
        [TestCase(new long[]{111,222,333333333}, 166.5, 222, 166666777.5)]
        [TestCase(new long[]{-235,-100,-30,100,300,400,500}, -65,100, 350)]
        public void Percentile(long[] values, double expected_q1, double expected_q2, double expected_q3)
        {
            double q1 = Calculation.Percentile(values,25);
            double q2 = Calculation.Percentile(values,50);
            double q3 = Calculation.Percentile(values,75);

            Assert.AreEqual(expected_q1, q1, "Q1 is incorrect");
            Assert.AreEqual(expected_q2, q2, "Q2 is incorrect");
            Assert.AreEqual(expected_q3, q3, "Q3 is incorrect");
        }


        
    }
}
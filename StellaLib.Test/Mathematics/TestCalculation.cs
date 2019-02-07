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
        
    }
}
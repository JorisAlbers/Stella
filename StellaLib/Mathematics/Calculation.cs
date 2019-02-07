using System.Linq;

namespace StellaLib.Mathematics
{
    public static class Calculation
    {
        public static long Median(long[] array)
        {
            int numberCount = array.Length; 
            int halfIndex = numberCount/2;
            long[] sortedNumbers = array.OrderBy(n=>n).ToArray(); 

            if ((numberCount % 2) == 0) 
            { 
                return (sortedNumbers[halfIndex]+ sortedNumbers[halfIndex-1]) / 2; 
            } 
            else { 
                return sortedNumbers[halfIndex]; 
            } 
        }

    }
}
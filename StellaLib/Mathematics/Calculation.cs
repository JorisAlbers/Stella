using System;
using System.Linq;

namespace StellaLib.Mathematics
{
    public static class Calculation
    {
        public static double Median(long[] array)
        {
            int numberCount = array.Length; 
            int halfIndex = numberCount/2;
            long[] sortedNumbers = array.OrderBy(n=>n).ToArray(); 

            if ((numberCount % 2) == 0) 
            { 
                return (sortedNumbers[halfIndex]+ sortedNumbers[halfIndex-1]) / 2; //TODO Indexoutofrangeexcpetion
            } 
            else { 
                return sortedNumbers[halfIndex]; 
            } 
        }

        /// <summary>
        /// Calulates the percentile of an array of long. The array must be sorted!
        /// </summary>
        /// <param name="sortedData"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double Percentile(long[] sortedData, double p)
        {
            int N = sortedData.Length;
            double n = (N - 1) * p + 1;

            if (n == 1d) return sortedData[0];
            else if (n == N) return sortedData[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sortedData[k - 1] + d * (sortedData[k] - sortedData[k - 1]);
            }
        }

    }
}
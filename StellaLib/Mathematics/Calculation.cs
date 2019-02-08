using System;
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

        /// <summary>
        /// Calulates the percentile of an array of long. The array must be sorted!
        /// </summary>
        /// <param name="sortedData"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double Percentile(long[] sortedData, double p)
        {
            // algo derived from Aczel pg 15 bottom
            if (p >= 100.0d) return sortedData[sortedData.Length - 1];

            double position = (double)(sortedData.Length + 1) * p / 100.0;
            double leftNumber = 0.0d, rightNumber = 0.0d;

            double n = p / 100.0d * (sortedData.Length - 1) + 1.0d;

            if (position >= 1)
            {
                leftNumber = sortedData[(int)System.Math.Floor(n) - 1];
                rightNumber = sortedData[(int)System.Math.Floor(n)];
            }
            else
            {
                leftNumber = sortedData[0]; // first data
                rightNumber = sortedData[1]; // first data
            }

            if (leftNumber == rightNumber)
                return leftNumber;
            else
            {
                double part = n - System.Math.Floor(n);
                return leftNumber + part * (rightNumber - leftNumber);
            }
        }

    }
}
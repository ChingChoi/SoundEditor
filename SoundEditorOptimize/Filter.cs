//
// Author: Choi, Ching
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Define filter static methods
    /// </summary>
    class Filter
    {
        /// <summary>
        /// Takes in original data, and a filter
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="filter">filter</param>
        /// <returns>filtered data</returns>
        public static double[] filter(double[] data, int[] filter)
        {
            double[] filteredData = new double[data.Length];
            AmplitudeF[] filterF = new AmplitudeF[filter.Length];
            for (int i = 0; i < filterF.Length; i++)
            {
                filterF[i].re = filter[i];
                filterF[i].im = filter[i];
            }
            double[] filterT = FourierTransform.inverseFourier(filterF, filterF.Length);

            if (data.Length < ThreadSetting.THREAD_THRESHOLD && filter.Length < ThreadSetting.THREAD_THRESHOLD)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    for (int j = 0; j < filterT.Length && j + i < data.Length; j++)
                    {
                        filteredData[i] += data[i + j] * filterT[j];
                    }
                }
            }
            else
            {
                int segment = (int)Math.Ceiling((double)data.Length / (double)ThreadSetting.threadNum);
                Thread[] threads = new Thread[ThreadSetting.threadNum];
                for (int i = 0; i < ThreadSetting.threadNum; i++)
                {
                    int z = i;
                    if (i < ThreadSetting.threadNum - 1)
                    {
                        threads[z] = new Thread(() => filterThread(filteredData, data, filterT, segment * z, segment * (z + 1)));
                    }
                    else
                    {
                        threads[z] = new Thread(() => filterThread(filteredData, data, filterT, segment * z, data.Length));
                    }
                }
                foreach (Thread t in threads)
                {
                    t.Start();
                }
                foreach (Thread t in threads)
                {
                    t.Join();
                }
            }
            return filteredData;
        }

        // 
        /// <summary>
        /// Get the filter using specified values
        /// Takes in size of filter, start, and end (cutoff)
        /// </summary>
        /// <param name="size">Size of filter</param>
        /// <param name="start">Start position of filter</param>
        /// <param name="end">End position of filter</param>
        /// <returns>filter</returns>
        public static int[] getFilter(int size, int start, int end)
        {
            int[] filter = new int[size];
            for (int i = start; i < size + start; i++)
            {
                if (i < end || i > size + start - end)
                {
                    filter[i] = 1;
                }
                else
                {
                    filter[i] = 0;
                }
            }
            return filter;
        }

        /// <summary>
        /// Thread for filter method
        /// </summary>
        /// <param name="filteredData">Filtered data array</param>
        /// <param name="data">Input data array</param>
        /// <param name="filterT">Filter in time domain</param>
        /// <param name="startPos">Start position of thread work</param>
        /// <param name="endPos">End position of thread work</param>
        private static void filterThread(double[] filteredData, double[] data, double[] filterT, int startPos, int endPos)
        {
            for (int i = startPos; i < endPos; i++)
            {
                for (int j = 0; j < filterT.Length && j + i < data.Length; j++)
                {
                    filteredData[i] += data[i + j] * filterT[j];
                }
            }
        }
    }
}

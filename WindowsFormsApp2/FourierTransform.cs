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
    /// Holds real and imaginary number of a frequency bin
    /// </summary>
    public struct AmplitudeF
    {
        public double re;
        public double im;

        public AmplitudeF(double re, double im)
        {
            this.re = re;
            this.im = im;
        }
    }
    /// <summary>
    /// Defines fourier transform methods
    /// </summary>
    class FourierTransform
    {
        /// <summary>
        /// Half fourier formula that take in a set of data point with N samples per second
        /// </summary>
        /// <param name="data">input data</param>
        /// <param name="N">Size to apply half fourier</param>
        /// <returns>an array of Double Afn</returns>
        public static double[] halfFourier(double[] data, int N)
        {
            double[] Afn = new double[N];
            for (int f = 0; f < data.Length; f++)
            {
                for (int t = 0; t < data.Length; t++)
                {
                    Afn[f] += data[t] * Math.Cos(2 * Math.PI * t * f / (double)N);
                }
            }
            return Afn;
        }


        /// <summary>
        /// Full fourier formula that takes in a set of data point with N samples per second
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="N">Size to apply fourier</param>
        /// <returns>an array of AmplitudeF Af</returns>
        public static AmplitudeF[] fourier(double[] data, int N)
        {
            AmplitudeF[] Af = new AmplitudeF[N];
            if (N < ThreadSetting.THREAD_THRESHOLD)
            {
                for (int f = 0; f < data.Length; f++)
                {
                    double re = 0;
                    double im = 0;
                    for (int t = 0; t < N; t++)
                    {
                        re += data[t] * Math.Cos(2 * Math.PI * t * f / (double)N);
                        im += data[t] * (-1) * Math.Sin(2 * Math.PI * t * f / (double)N);
                    }
                    Af[f] = new AmplitudeF(re / (double)data.Length, im / (double)data.Length);
                }
            }
            else
            {
                int segment = (int)Math.Ceiling((double)N / (double)ThreadSetting.threadNum);
                Thread[] threads = new Thread[ThreadSetting.threadNum];
                for (int i = 0; i < ThreadSetting.threadNum; i++)
                {
                    int z = i;
                    if (i < ThreadSetting.threadNum - 1)
                    {
                        threads[z] = new Thread(() => fourierThread(Af, data, N, segment * z, segment * (z + 1)));
                    }
                    else
                    {
                        threads[z] = new Thread(() => fourierThread(Af, data, N, segment * z, data.Length));
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

            return Af;
        }

        /// <summary>
        /// Reverse fourier formula that takes in a set of data point with N samples per second
        /// </summary>
        /// <param name="Af">Frequency bins</param>
        /// <param name="N">Sizes to be processed</param>
        /// <returns>an array of sample at time t</returns>
        public static double[] inverseFourier(AmplitudeF[] Af, int N)
        {
            double[] St = new Double[Af.Length];
            if (N < ThreadSetting.THREAD_THRESHOLD)
            {
                for (int t = 0; t < Af.Length; t++)
                {
                    for (int f = 0; f < Af.Length; f++)
                    {
                        St[t] += Af[f].re * Math.Cos(2 * Math.PI * t * f / (double)N) +
                            Af[f].im * Math.Sin(2 * Math.PI * f * t / (double)N);
                    }
                    St[t] = St[t] / (double)N;
                }
            }
            else
            {
                int segment = (int)Math.Ceiling((double)N / (double)ThreadSetting.threadNum);
                Thread[] threads = new Thread[ThreadSetting.threadNum];
                for (int i = 0; i < ThreadSetting.threadNum; i++)
                {
                    int z = i;
                    if (i < ThreadSetting.threadNum - 1)
                    {
                        threads[z] = new Thread(() => inverseFourierThread(St, Af, N, segment * z, segment * (z + 1)));
                    }
                    else
                    {
                        threads[z] = new Thread(() => inverseFourierThread(St, Af, N, segment * z, Af.Length));
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
            return St;
        }

        /// <summary>
        /// Get actual amplitude using provided frequency bins
        /// </summary>
        /// <param name="Af">Frequency bins</param>
        /// <returns>Actual amplitude</returns>
        public static double[] getAmplitudeFromAf(AmplitudeF[] Af)
        {
            double[] A = new Double[Af.Length];
            for (int f = 0; f < Af.Length; f++)
            {
                A[f] = Math.Sqrt(Af[f].re * Af[f].re + Af[f].im * Af[f].im);
            }
            return A;
        }

        /// <summary>
        /// Thread method for fourier
        /// Takes destination Af, and set the result using original data, size of data 
        /// starting and ending position of where the thread will do the job
        /// </summary>
        /// <param name="Af">Frequency bins</param>
        /// <param name="data">Data to be processed</param>
        /// <param name="N">Size of data</param>
        /// <param name="startPos">Starting position to be processed</param>
        /// <param name="endPos">Ending position to be processed</param>
        private static void fourierThread(AmplitudeF[] Af, double[] data, int N, int startPos, int endPos)
        {
            for (int f = startPos; f < endPos; f++)
            {
                double re = 0;
                double im = 0;
                for (int t = 0; t < N; t++)
                {
                    re += data[t] * Math.Cos(2 * Math.PI * t * f / (double)N);
                    im += data[t] * (-1) * Math.Sin(2 * Math.PI * t * f / (double)N);
                }
                Af[f] = new AmplitudeF(re / (double)data.Length, im / (double)data.Length);
            }
        }

        /// <summary>
        /// Thread method for inverse fourier
        /// Takes destination St, and set the result using original Af, size of Af 
        /// starting and ending position of where the thread will do the job
        /// </summary>
        /// <param name="St">Sample in time domain</param>
        /// <param name="Af">Frequency bins</param>
        /// <param name="N">Size of sample</param>
        /// <param name="startPos">Starting position to be processed</param>
        /// <param name="endPos">Ending position to be processed</param>
        private static void inverseFourierThread(double[] St, AmplitudeF[] Af, int N, int startPos, int endPos)
        {
            for (int t = startPos; t < endPos; t++)
            {
                for (int f = 0; f < Af.Length; f++)
                {
                    St[t] += Af[f].re * Math.Cos(2 * Math.PI * t * f / (double)N) +
                        Af[f].im * Math.Sin(2 * Math.PI * f * t / (double)N);
                }
                St[t] = St[t] / (double)N;
            }
        }
    }
}

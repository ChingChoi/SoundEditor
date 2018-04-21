using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Transform array to a different type and handle internal value adjustment when needed
    /// </summary>
    class ArrayTransform
    {
        /// <summary>
        /// Convert List<DataPoint> to double
        /// </summary>
        /// <param name="listData">Input data</param>
        /// <returns>converted double array of data</returns>
        public static double[] listDataToDouble(List<DataPoint> listData)
        {
            double[] doubleArray = new double[listData.Count];
            for (int i = 0; i < listData.Count; i++)
            {
                doubleArray[i] = listData.ElementAt(i).YValues[0];
            }
            return doubleArray;
        }

        /// <summary>
        /// Convert DataPointCollection to double
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>converted double array of data</returns>
        public static double[] DataCollectionToDouble(DataPointCollection data)
        {
            double[] doubleArray = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                doubleArray[i] = data[i].YValues[0];
            }
            return doubleArray;
        }

        /// <summary>
        /// Convert byte record to double and apply 128 adjustment
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>converted double array of data</returns>
        public static double[] byteRecordToDouble(byte[] data)
        {
            double[] doubleArray = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                doubleArray[i] = data[i] - 128;
            }
            return doubleArray;
        }

        /// <summary>
        /// Convert double record data back to byte record and apply 128 adjustment
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns>converted double array of data</returns>
        public static byte[] doubleRecordToByte(double[] data)
        {
            byte[] byteArray = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                byteArray[i] = (byte)(data[i] + 128);
            }
            return byteArray;
        }

        public static float[] byteRecordToFloat(byte[] data)
        {
            float[] floatArray = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                floatArray[i] = (float)data[i];
            }
            return floatArray;
        }

        /// <summary>
        /// Convert float array to double array
        /// </summary>
        /// <param name="inputArray">input data</param>
        /// <returns>converted double array of data</returns>
        public static double[] convertToDouble(float[] inputArray)
        {
            if (inputArray == null)
                return null;

            double[] output = new double[inputArray.Length];
            for (int i = 0; i < inputArray.Length; i++)
                output[i] = inputArray[i];
            return output;
        }

        /// <summary>
        /// Convert double array to float array
        /// </summary>
        /// <param name="inputArray">input data</param>
        /// <returns>converted float array of data</returns>
        public static float[] convertToFloat(double[] inputArray)
        {
            if (inputArray == null)
                return null;

            float[] output = new float[inputArray.Length];
            for (int i = 0; i < inputArray.Length; i++)
                output[i] = (float)inputArray[i];
            return output;
        }
    }
}

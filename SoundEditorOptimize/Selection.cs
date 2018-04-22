//
// Author: Choi, Ching
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SoundEditorOptimize;
using System.Threading;

/// <summary>
/// Holds pixel compression information
/// </summary>
struct PixelCompression
{
    public int compressionFactor;
    public int level;
    public List<DataPoint>[] dataArray;
    public int[] viewSize;
    public int[] compressionLevel;
}

namespace SoundEditorSelector
{
    /// <summary>
    /// Handle select related events on chartAreas
    /// </summary>
    class Selection
    {
        SoundEditorOptimize.SoundEditorOptimize soundEditorOptimizeObject;
        int screenWidth;
        int[] xMin;
        int[] xMax;
        double[] yMax;
        int cXMax;
        int cXMin;
        int[] scrollPos;
        int[] zoomPos;
        int[] blockSize;
        int[] viewSize;
        int[] currentCompressLevel;
        int incrementorSpeed;
        int decrementorSpeed;
        int chartIndex;
        int selectedIndex;
        int maxChartSize;
        int fourierBelong;
        double scrollSpeedScaler;
        double maxScrollScaler;
        bool isInChart = false;
        double[][] datas;
        List<DataPoint>[] dataPoints;
        double[] data;
        double[] copiedData;
        int[] chartType;    // 0 = time domain, 1 = frequency domain
        int[] chartBelong;  // index of the parent chart, same as parent chart if not a child
        int[] chartChannel; // indicate the chart is part of a stereo channel
        int[] chartPair;    // finds its channel pair if stereo
        int[] currentScrollPos;
        int[] prevScrollPos;
        System.Windows.Forms.Timer incrementor;
        System.Windows.Forms.Timer decrementor;
        Point mdown;
        Point mUp;
        Chart chart;
        Pen pen;
        Color selectedColor;
        Color themeColor;
        Color[] selectedColors;
        Color[] themeColors;
        SolidBrush brush;
        SolidBrush[] brushes;
        SolidBrush lineBrush;
        SolidBrush[] lineBrushes;
        ArchiveManager archiveManager;
        PixelCompression[] pixelc;

        public int[] BlockSize { get => blockSize; set => blockSize = value; }
        public int[] ViewSize { get => viewSize; set => viewSize = value; }
        public double[] Data { get => data; set => data = value; }
        public int[] XMax { get => xMax; set => xMax = value; }
        public int[] XMin { get => xMin; set => xMin = value; }
        public int CXMax { get => cXMax; set => cXMax = value; }
        public int CXMin { get => cXMin; set => cXMin = value; }
        public double[][] Datas { get => datas; set => datas = value; }
        public int SelectedIndex { get => selectedIndex; set => selectedIndex = value; }
        public Color[] SelectedColors { get => selectedColors; set => selectedColors = value; }
        public Color[] ThemeColors { get => themeColors; set => themeColors = value; }
        public Color SelectedColor { get => selectedColor; set => selectedColor = value; }
        public SolidBrush[] Brushes { get => brushes; set => brushes = value; }
        public SolidBrush[] LineBrushes { get => lineBrushes; set => lineBrushes = value; }
        internal ArchiveManager ArchiveManager { get => archiveManager; set => archiveManager = value; }
        public int[] ChartType { get => chartType; set => chartType = value; }
        public int[] ChartBelong { get => chartBelong; set => chartBelong = value; }
        public int ChartIndex { get => chartIndex; set => chartIndex = value; }
        public List<DataPoint>[] DataPoints { get => dataPoints; set => dataPoints = value; }
        internal PixelCompression[] Pixelc { get => pixelc; set => pixelc = value; }
        public int[] CurrentCompressLevel { get => currentCompressLevel; set => currentCompressLevel = value; }
        public SoundEditorOptimize.SoundEditorOptimize SoundEditorOptimizeObject { get => soundEditorOptimizeObject; set => soundEditorOptimizeObject = value; }
        public int[] ZoomPos { get => zoomPos; set => zoomPos = value; }
        public int[] CurrentScrollPos { get => currentScrollPos; set => currentScrollPos = value; }
        public int[] ChartChannel { get => chartChannel; set => chartChannel = value; }
        public int[] ChartPair { get => chartPair; set => chartPair = value; }
        public int MaxChartSize { get => maxChartSize; set => maxChartSize = value; }
        public int[] ScrollPos { get => scrollPos; set => scrollPos = value; }
        public int FourierBelong { get => fourierBelong; set => fourierBelong = value; }

        /// <summary>
        /// Constructor for selection object
        /// </summary>
        /// <param name="soundEditorOptimimzeObject">SoundEditorOptimize object</param>
        /// <param name="chartInput">Chart object</param>
        /// <param name="data">Initial data to be displayed</param>
        /// <param name="scrollPos">Initial scroll position</param>
        /// <param name="blockSize">Initial block size</param>
        /// <param name="themeColor">Theme color of GUI</param>
        /// <param name="chartIndex">Chart index of chartArea</param>
        public Selection(SoundEditorOptimize.SoundEditorOptimize soundEditorOptimimzeObject,
            Chart chartInput, double[] data, int scrollPos, int blockSize, Color themeColor, int chartIndex)
        {
            this.SoundEditorOptimizeObject = soundEditorOptimimzeObject;
            this.MaxChartSize = 6;
            this.screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            this.CurrentScrollPos = new int[MaxChartSize];
            this.prevScrollPos = new int[MaxChartSize];
            this.pen = new Pen(new SolidBrush(Color.FromArgb(100, 0, 255, 255)));
            this.brush = new SolidBrush(Color.FromArgb(50, 0, 255, 255));
            this.brushes = new SolidBrush[MaxChartSize];
            this.brushes[0] = brush;
            this.lineBrush = new SolidBrush(Color.FromArgb(255, 0, 255, 255));
            this.lineBrushes = new SolidBrush[MaxChartSize];
            this.lineBrushes[0] = lineBrush;
            this.themeColor = themeColor;
            this.themeColors = new Color[MaxChartSize];
            this.themeColors[0] = themeColor;
            this.selectedColor = Color.Cyan;
            this.selectedColors = new Color[MaxChartSize];
            this.selectedColors[0] = selectedColor;
            this.mdown = Point.Empty;
            this.chartIndex = chartIndex;
            this.selectedIndex = -1;
            this.datas = new double[MaxChartSize][];
            this.data = data;
            this.datas[0] = data;
            this.dataPoints = new List<DataPoint>[MaxChartSize];
            this.chartType = new int[MaxChartSize];
            this.chartType[0] = 0;
            this.chartBelong = new int[MaxChartSize];
            this.chartBelong[0] = 0;
            this.chartChannel = new int[MaxChartSize];
            this.chartChannel[0] = 1;
            this.chartPair = new int[MaxChartSize];
            for (int i = 0; i < chartPair.Length; i++)
            {
                chartPair[i] = -1;
            }
            this.currentCompressLevel = new int[MaxChartSize];
            this.currentCompressLevel[0] = 1;
            this.scrollSpeedScaler = 3000;
            this.maxScrollScaler = 10;
            this.incrementor = new System.Windows.Forms.Timer();
            this.incrementor.Interval = 1;
            this.incrementor.Enabled = false;
            this.incrementor.Tick += new EventHandler(incrementorEvent);
            this.incrementorSpeed = 1;
            this.decrementor = new System.Windows.Forms.Timer();
            this.decrementor.Interval = 1;
            this.decrementor.Enabled = false;
            this.decrementor.Tick += new EventHandler(decrementorEvent);
            this.decrementorSpeed = -1;
            this.xMax = new int[MaxChartSize];
            this.xMax[chartIndex] = -1;
            this.xMin = new int[MaxChartSize];
            this.xMin[chartIndex] = -1;
            this.yMax = new double[MaxChartSize];
            this.yMax[chartIndex] = 0;
            this.cXMax = 0;
            this.cXMin = 0;
            this.blockSize = new int[MaxChartSize];
            this.blockSize[chartIndex] = blockSize;
            this.viewSize = new int[MaxChartSize];
            this.viewSize[chartIndex] = blockSize + 1;
            this.ScrollPos = new int[MaxChartSize];
            this.ScrollPos[chartIndex] = scrollPos;
            this.ZoomPos = new int[MaxChartSize];
            this.ZoomPos[chartIndex] = 0;
            this.CurrentScrollPos[chartIndex] = (int)chartInput.ChartAreas[this.ChartIndex].AxisX.ScaleView.Position;
            this.prevScrollPos[chartIndex] = this.CurrentScrollPos[chartIndex];
            this.chart = chartInput;
            this.FourierBelong = -1;
            this.ArchiveManager = new ArchiveManager(this.datas);
            this.Pixelc = new PixelCompression[MaxChartSize];
            this.chart.MouseDown += new MouseEventHandler(this.chart_MouseDown);
            this.chart.MouseMove += new MouseEventHandler(this.chart_MouseMove);
            this.chart.MouseUp += new MouseEventHandler(this.chart_MouseUp);
            this.chart.MouseWheel += new MouseEventHandler(this.chart_MouseWheel);
        }

        /// <summary>
        /// Thread method for handling pixel compression when level is equal to one
        /// </summary>
        /// <param name="cData">Stores compressed data</param>
        /// <param name="inputdata">Input data</param>
        /// <param name="startPos">Start position of thread</param>
        /// <param name="endPos">End position of thread</param>
        private void pixelCompressThreadCaseOne(List<DataPoint> cData, double[] inputData, PixelCompression compressed, int level, int startPos, int endPos)
        {
            int j = startPos;
            while (j < endPos)
            {
                double[] minMax = new double[2];
                //MaxAvg
                minMax[0] = inputData[j];
                //MinAvg
                minMax[1] = inputData[j];
                for (int k = 0; k < compressed.compressionLevel[level] &&
                    j + k < inputData.Length; k++)
                {
                    if (inputData[j + k] > 0)
                    {
                        minMax[0] += inputData[j + k];
                    }
                    else if (inputData[j + k] < 0)
                    {
                        minMax[1] += inputData[j + k];
                    }
                }
                minMax[0] /= compressed.compressionLevel[level];
                minMax[1] /= compressed.compressionLevel[level];
                if (minMax[0] < 0)
                {
                    minMax[0] = 0;
                }
                else if (minMax[1] > 0)
                {
                    minMax[1] = 0;
                }
                cData.Add(new DataPoint(j, minMax));
                j += compressed.compressionLevel[level];
            }
        }

        /// <summary>
        /// Thread method for handling pixel compression when level is equal to zero
        /// </summary>
        /// <param name="cData">Stores compressed data</param>
        /// <param name="inputdata">Input data</param>
        /// <param name="startPos">Start position of thread</param>
        /// <param name="endPos">End position of thread</param>
        private void pixelCompressThreadCaseZero(List<DataPoint>cData, double[] data, int startPos, int endPos)
        {
            for (int i = startPos; i < endPos; i++)
            {
                cData.Add(new DataPoint(i, data[i]));
            }
        }

        // Take input data and compress for pixel display, and store result into
        // corresponding index of selection instances
        public PixelCompression pixelCompress(int index, double[] inputData)
        {
            //
            // latest implementation
            //
            Datas[index] = inputData;
            //
            // latest implementation
            //
            PixelCompression compressed;
            compressed.compressionFactor = 10;
            int length = inputData.Length;
            int level = 1;
            while (length / compressed.compressionFactor > screenWidth * 2)
            {
                level++;
                length /= compressed.compressionFactor;
            }
            if (inputData.Length > screenWidth * 2 && level != 1)
            {
                level++;
            }
            compressed.level = level;
            compressed.dataArray = new List<DataPoint>[compressed.level];
            compressed.viewSize = new int[compressed.level];
            compressed.compressionLevel = new int[compressed.level];
            for (int i = 0; i < compressed.level; i++)
            {
                compressed.compressionLevel[i] = 1;
            }
            for (int i = 0; i < level; i++)
            {
                List<DataPoint> cdata = new List<DataPoint>();
                if (i == level - 1 && level > 1)
                {
                    compressed.compressionLevel[i] = inputData.Length / screenWidth;
                }
                else
                {
                    compressed.compressionLevel[i] = (int)(Math.Pow(compressed.compressionFactor, i));
                    compressed.viewSize[i] = inputData.Length / compressed.compressionLevel[i];
                }
                if (i == 0)
                {
                    //if (inputData.Length < ThreadSetting.THREAD_THRESHOLD)
                    //{
                        for (int j = 0; j < inputData.Length; j++)
                        {
                            cdata.Add(new DataPoint(j, inputData[j]));
                        }
                    //}
                    //else
                    //{
                    //    int segment = (int)Math.Ceiling((double)inputData.Length / (double)ThreadSetting.threadNum);
                    //    Thread[] threads = new Thread[ThreadSetting.threadNum];
                    //    for (int j = 0; j < ThreadSetting.threadNum; j++)
                    //    {
                    //        int z = j;
                    //        if (j < ThreadSetting.threadNum - 1)
                    //        {
                    //            threads[z] = new Thread(() => pixelCompressThreadCaseZero(cdata, inputData, segment * z, segment * (z + 1)));
                    //        }
                    //        else
                    //        {
                    //            threads[z] = new Thread(() => pixelCompressThreadCaseZero(cdata, inputData, segment * z, inputData.Length));
                    //        }
                    //    }
                    //    foreach (Thread t in threads)
                    //    {
                    //        t.Start();
                    //    }
                    //    foreach (Thread t in threads)
                    //    {
                    //        t.Join();
                    //    }
                    //}
                }
                else if (i == level - 1 && level > 1)
                {
                    int j = 0;
                    int range = inputData.Length / screenWidth;
                    while (j < inputData.Length)
                    {
                        double[] minMax = new double[2];
                        //MaxAvg
                        minMax[0] = inputData[j];
                        //MinAvg
                        minMax[1] = inputData[j];
                        for (int k = 0; k < range &&
                            j + k < inputData.Length; k++)
                        {
                            if (inputData[j + k] > 0)
                            {
                                minMax[0] += inputData[j + k];
                            }
                            else if (inputData[j + k] < 0)
                            {
                                minMax[1] += inputData[j + k];
                            }
                        }
                        minMax[0] /= range;
                        minMax[1] /= range;
                        if (minMax[0] < 0)
                        {
                            minMax[0] = 0;
                        }
                        else if (minMax[1] > 0)
                        {
                            minMax[1] = 0;
                        }
                        cdata.Add(new DataPoint(j, minMax));
                        j += range;
                    }
                    compressed.viewSize[i] = cdata.Count;
                }
                else if (i == 1)
                {
                    //if (inputData.Length < ThreadSetting.THREAD_THRESHOLD)
                    //{
                        int j = 0;
                        while (j < inputData.Length)
                        {
                            double[] minMax = new double[2];
                            //MaxAvg
                            minMax[0] = inputData[j];
                            //MinAvg
                            minMax[1] = inputData[j];
                            for (int k = 0; k < compressed.compressionLevel[i] &&
                                j + k < inputData.Length; k++)
                            {
                                if (inputData[j + k] > 0)
                                {
                                    minMax[0] += inputData[j + k];
                                }
                                else if (inputData[j + k] < 0)
                                {
                                    minMax[1] += inputData[j + k];
                                }
                            }
                            minMax[0] /= compressed.compressionLevel[i];
                            minMax[1] /= compressed.compressionLevel[i];
                            if (minMax[0] < 0)
                            {
                                minMax[0] = 0;
                            }
                            else if (minMax[1] > 0)
                            {
                                minMax[1] = 0;
                            }
                            cdata.Add(new DataPoint(j, minMax));
                            j += compressed.compressionLevel[i];
                        }
                    //}
                    //else
                    //{
                    //    int segment = (int)Math.Ceiling((double)inputData.Length / (double)ThreadSetting.threadNum);
                    //    Thread[] threads = new Thread[ThreadSetting.threadNum];
                    //    for (int j = 0; j < ThreadSetting.threadNum; j++)
                    //    {
                    //        int z = j;
                    //        if (j < ThreadSetting.threadNum - 1)
                    //        {
                    //            threads[z] = new Thread(() => pixelCompressThreadCaseZero(cdata, inputData, segment * z, segment * (z + 1)));
                    //        }
                    //        else
                    //        {
                    //            threads[z] = new Thread(() => pixelCompressThreadCaseZero(cdata, inputData, segment * z, inputData.Length));
                    //        }
                    //    }
                    //    foreach (Thread t in threads)
                    //    {
                    //        t.Start();
                    //    }
                    //    foreach (Thread t in threads)
                    //    {
                    //        t.Join();
                    //    }
                    //}
                }
                else
                {
                    int j = 0;
                    int x = 0;
                    while (j < compressed.dataArray[i - 1].Count)
                    {
                        double[] minMax = new double[2];
                        minMax[0] = compressed.dataArray[i - 1][j].XValue;
                        minMax[1] = compressed.dataArray[i - 1][j].XValue;
                        for (int k = 0; k < compressed.compressionLevel[i] && j + k < compressed.dataArray[i - 1].Count; k++)
                        {
                            if (compressed.dataArray[i - 1][j + k].YValues[0] > 0)
                            {
                                minMax[0] += compressed.dataArray[i - 1][j + k].YValues[0];
                            }
                            else if (compressed.dataArray[i - 1][j + k].YValues.Length > 1 &&
                                compressed.dataArray[i - 1][j + k].YValues[1] < 0)
                            {
                                minMax[1] += compressed.dataArray[i - 1][j + k].YValues[1];
                            }
                        }
                        minMax[0] /= compressed.compressionLevel[i];
                        minMax[1] /= compressed.compressionLevel[i];
                        if (minMax[0] < 0)
                        {
                            minMax[0] = 0;
                        }
                        else if (minMax[1] > 0)
                        {
                            minMax[1] = 0;
                        }
                        cdata.Add(new DataPoint(x, minMax));
                        x += compressed.compressionLevel[i];
                        j += compressed.compressionFactor;
                    }
                }
                compressed.dataArray[i] = cdata;
            }

            if (compressed.level - 1 < 1)
            {
                DataPoints[index] = compressed.dataArray[0];
                CurrentCompressLevel[index] = compressed.compressionLevel[0];
                viewSize[index] = inputData.Length;
                //chart.ChartAreas[chartIndex].AxisX.ScaleView.Zoom(0, viewSize[chartIndex] - currentCompressLevel[chartIndex]);
                ////chart.ChartAreas[chartIndex].AxisX.ScrollBar.Size = 15;
                //chart.ChartAreas[chartIndex].AxisX.ScaleView.SmallScrollSize = viewSize[chartIndex] - currentCompressLevel[chartIndex];
            }
            else
            {
                DataPoints[index] = compressed.dataArray[compressed.level - 1];
                CurrentCompressLevel[index] = compressed.compressionLevel[compressed.level - 1];
                viewSize[index] = inputData.Length / compressed.compressionLevel[compressed.level - 1];
                zoomPos[index] = 0;
                //                chart.ChartAreas[chartIndex].AxisX.ScaleView.Zoom(0, DataPoints[chartIndex].Count - 1);
                //chart.ChartAreas[chartIndex].AxisX.ScaleView.SmallScrollSize = viewSize[chartIndex] - currentCompressLevel[chartIndex];
            }
            pixelc[index] = compressed;
            return compressed;
        }

        /// <summary>
        /// Handles scrolling data towards positive end 
        /// when user selected outside of viewable chartArea
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void incrementorEvent(object sender, EventArgs e)
        {
            if (ScrollPos[ChartIndex] < datas[chartIndex].Length - (viewSize[ChartIndex] - 1) * currentCompressLevel[chartIndex])
            {
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                DataPointCollection csp = chart.Series[ChartIndex].Points;

                int increased = 0;
                do
                {
                    ScrollPos[ChartIndex]++;
                    increased++;
                } while (increased < incrementorSpeed && ScrollPos[ChartIndex] < datas[chartIndex].Length - (viewSize[ChartIndex] - 1) * currentCompressLevel[chartIndex]);
                chart.ChartAreas[ChartIndex].AxisX.ScaleView.Zoom(ScrollPos[ChartIndex], BlockSize[chartIndex] - 1 + ScrollPos[ChartIndex] - 1);
                CurrentScrollPos[ChartIndex] = (int)chart.ChartAreas[ChartIndex].AxisX.ScaleView.Position;
                int currentScrollPosAdjust = (CurrentScrollPos[chartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
                prevScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];
                int curMaxView = currentScrollPosAdjust + viewSize[chartIndex] - 1;
                if (curMaxView > csp.Count - 1)
                {
                    curMaxView = csp.Count - 1;
                }
                int xMINP = 0;
                // | . |
                if (xMin[ChartIndex] >= currentScrollPosAdjust && xMin[ChartIndex] <= curMaxView)
                {
                    xMINP = (int)xAxis.ValueToPixelPosition(csp[xMin[ChartIndex]].XValue);
                }
                else
                {
                    xMINP = (int)xAxis.ValueToPixelPosition(csp[currentScrollPosAdjust].XValue);
                }
                int xMAXP = (int)xAxis.ValueToPixelPosition(csp[curMaxView].XValue);
                chart.Refresh();
                using (Graphics g = chart.CreateGraphics())
                    g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartIndex));
            }
        }

        /// <summary>
        /// Handles scrolling data towards negative end 
        /// when user selected outside of viewable chartArea
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void decrementorEvent(object sender, EventArgs e)
        {
            if (ScrollPos[ChartIndex] > 1)
            {
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                DataPointCollection csp = chart.Series[ChartIndex].Points;
                int decreased = 0;
                do
                {
                    ScrollPos[ChartIndex]--;
                    decreased--;
                } while (decreased > decrementorSpeed && ScrollPos[ChartIndex] > 1);
                chart.ChartAreas[ChartIndex].AxisX.ScaleView.Zoom(ScrollPos[ChartIndex], BlockSize[chartIndex] - 1 + ScrollPos[ChartIndex] - 1);
                CurrentScrollPos[ChartIndex] = (int)chart.ChartAreas[ChartIndex].AxisX.ScaleView.Position;
                prevScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];

                int currentScrollPosAdjust = (CurrentScrollPos[chartIndex] - CurrentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
                int curMaxView = currentScrollPosAdjust + viewSize[chartIndex] - 1;
                int xMINP = (int)xAxis.ValueToPixelPosition(csp[currentScrollPosAdjust].XValue);
                int xMAXP = 0;
                // swap max min selected in case
                if (xMin[ChartIndex] > xMax[ChartIndex])
                {
                    int temp = xMin[ChartIndex];
                    xMin[ChartIndex] = xMax[ChartIndex];
                    xMax[ChartIndex] = temp;
                }

                // | . |
                if (xMax[ChartIndex] >= currentScrollPosAdjust && xMax[ChartIndex] <= curMaxView)
                {
                    xMAXP = (int)xAxis.ValueToPixelPosition(csp[xMax[ChartIndex]].XValue);
                }
                else
                {
                    xMAXP = (int)xAxis.ValueToPixelPosition(csp[curMaxView].XValue);
                }

                chart.Refresh();
                using (Graphics g = chart.CreateGraphics())
                    g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartIndex));
            }
        }

        /// <summary>
        /// Attempt to set current chart index if chartArea is selected
        /// </summary>
        private void trySetCurrentChartIndex()
        {
            for (int i = 0; i < chart.ChartAreas.Count; i++)
            {
                Axis yAxis = chart.ChartAreas[i].AxisY;
                Axis xAxis = chart.ChartAreas[i].AxisX;
                int yMinP = (int)yAxis.ValueToPixelPosition(yAxis.Minimum);
                int yMaxP = (int)yAxis.ValueToPixelPosition(yAxis.Maximum);
                int currentScrollPosAdjust = (CurrentScrollPos[i] - CurrentCompressLevel[i]) / CurrentCompressLevel[i];
                int xMINP = (int)xAxis.ValueToPixelPosition(chart.Series[i].Points[currentScrollPosAdjust].XValue);
                int xMAXP = 0;
                if (currentScrollPosAdjust + viewSize[i] - 1 < chart.Series[i].Points.Count - 1)
                {
                    xMAXP = (int)xAxis.ValueToPixelPosition(chart.Series[i].Points[currentScrollPosAdjust + viewSize[i] - 1].XValue);
                }
                else
                {
                    xMAXP = (int)xAxis.ValueToPixelPosition(chart.Series[i].Points[chart.Series[i].Points.Count - 1].XValue);
                }

                if (mdown.X >= xMINP && mdown.X <= xMAXP && mdown.Y >= yMaxP && mdown.Y <= yMinP)
                {
                    isInChart = true;
                    ChartIndex = i;
                    //instant draw version
                    //viewSize[ChartIndex] = (int)chart.Series[i].Points[chart.Series[i].Points.Count - 1].XValue - 
                    //    (int)chart.Series[i].Points[0].XValue + 1;
                    data = Datas[i];

                    CurrentScrollPos[ChartIndex] = (int)chart.ChartAreas[ChartIndex].AxisX.ScaleView.Position;
                    prevScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];
                    i = chart.ChartAreas.Count;
                }
                else
                {
                    isInChart = false;
                }
            }
        }

        /// <summary>
        /// Handle the event when mouse is down
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void chart_MouseDown(object sender, MouseEventArgs e)
        {
            mdown = e.Location;
            trySetCurrentChartIndex();
            if (isInChart && chartType[ChartIndex] == 0)
            {
                Axis yAxis = chart.ChartAreas[ChartIndex].AxisY;
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                xMin[chartIndex] = (int)(Math.Round(xAxis.PixelPositionToValue(mdown.X)) / CurrentCompressLevel[chartIndex]);
                xMax[chartIndex] = 0;
                if (xMin[ChartIndex] == -1)
                {
                    xMin[ChartIndex] = 0;
                    xMax[ChartIndex] = 0;
                }
                isInChart = true;
            }
            else if (isInChart && chartType[ChartIndex] == 1)
            {
                xMin[ChartIndex] = 0;

                Axis yAxis = chart.ChartAreas[ChartIndex].AxisY;
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                int i = 0;
                while (mdown.X > xAxis.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[i].XValue))
                {
                    xMax[ChartIndex] = (int)chart.Series[ChartIndex].Points[i].XValue;
                    i++;
                }
                isInChart = true;
            }
        }

        /// <summary>
        /// Handle the event when mouse is moving
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void chart_MouseMove(object sender, MouseEventArgs e)
        {
            int currentScrollPosAdjust = (CurrentScrollPos[ChartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
            if (isInChart && e.Button == MouseButtons.Left)
            {
                Axis ax = chart.ChartAreas[ChartIndex].AxisX;
                int curMaxView = currentScrollPosAdjust + viewSize[chartIndex] - 1;
                //instant draw version
                //int xMAXP = (int)ax.ValueToPixelPosition(
                //    chart.Series[ChartIndex].Points[chart.Series[ChartIndex].Points.Count - 1].XValue);
                //int xMINP = (int)ax.ValueToPixelPosition(chart.Series[ChartIndex].Points[0].XValue);
                if (curMaxView > chart.Series[chartIndex].Points.Count - 1)
                {
                    curMaxView = chart.Series[chartIndex].Points.Count - 1;
                }
                int xMAXP = (int)ax.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[curMaxView].XValue);
                int xMINP = (int)ax.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[currentScrollPosAdjust].XValue);
                if (e.Location.X > xMAXP)
                {
                    double diff = e.Location.X - xMAXP;
                    incrementorSpeed = (int)((diff / scrollSpeedScaler) *
                        ((double)data.Length / maxScrollScaler));
                    ScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];
                    incrementor.Enabled = true;
                }
                else if (e.Location.X < xMINP)
                {
                    double diff = -(xMINP - e.Location.X);
                    decrementorSpeed = (int)((diff / scrollSpeedScaler) *
                        ((double)data.Length / maxScrollScaler));
                    ScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];
                    decrementor.Enabled = true;
                }
                else
                {
                    decrementor.Enabled = false;
                    incrementor.Enabled = false;
                }
            }
            CurrentScrollPos[ChartIndex] = (int)chart.ChartAreas[ChartIndex].AxisX.ScaleView.Position;
            prevScrollPos[ChartIndex] = CurrentScrollPos[ChartIndex];
            if (e.Button == MouseButtons.Left && isInChart)
            {
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                Axis yAxis = chart.ChartAreas[ChartIndex].AxisY;
                int curMaxView = currentScrollPosAdjust + viewSize[chartIndex] - 1;
                if (e.Location.Y < yAxis.ValueToPixelPosition(yAxis.Minimum)
                    && e.Location.Y > yAxis.ValueToPixelPosition(yAxis.Maximum))
                {
                    yMax[ChartIndex] = yAxis.PixelPositionToValue(e.Location.Y);
                }
                else if (e.Location.Y < yAxis.ValueToPixelPosition(yAxis.Maximum))
                {
                    yMax[ChartIndex] = yAxis.Maximum;
                }
                int xMINP = 0;
                {
                    // | .  |
                    if (xMin[ChartIndex] >= currentScrollPosAdjust &&
                        xMin[ChartIndex] < curMaxView)
                    {
                        xMINP = (int)xAxis.ValueToPixelPosition(xMin[ChartIndex] *
                            CurrentCompressLevel[chartIndex]);
                    }
                    // |  | .
                    else if (xMin[ChartIndex] > curMaxView)
                    {
                        xMINP = (int)xAxis.ValueToPixelPosition(curMaxView *
                            CurrentCompressLevel[chartIndex]);
                    }
                    // . | |
                    else
                    {
                        xMINP = (int)xAxis.ValueToPixelPosition(currentScrollPosAdjust *
                            CurrentCompressLevel[chartIndex]);
                    }
                    chart.Refresh();
                    if (chartType[ChartIndex] == 0)
                    {
                        using (Graphics g = chart.CreateGraphics())
                            g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(
                                new Point(xMINP, 0), e.Location, chart, chartIndex));

                        if (chartPair[chartIndex] != -1)
                        {
                            using (Graphics g = chart.CreateGraphics())
                                g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(
                                    new Point(xMINP, 0), e.Location, chart, chartPair[chartIndex]));
                        }
                    }
                    else if (chartType[ChartIndex] == 1)
                    {
                        int cPos = (int)xAxis.Minimum;
                        int xMAXP;
                        if (e.Location.X > xAxis.ValueToPixelPosition(xAxis.Minimum) &&
                            e.Location.X < xAxis.ValueToPixelPosition(xAxis.Maximum))
                        {
                            cPos = (int)xAxis.PixelPositionToValue(e.Location.X);
                        }
                        if (cPos >= (int)xAxis.Maximum / 2)
                        {
                            xMAXP = (int)xAxis.ValueToPixelPosition(
                                        (int)Math.Round(xAxis.Maximum / 2.0));
                        }
                        else if (cPos <= (int)xAxis.Minimum)
                        {
                            xMAXP = (int)xAxis.ValueToPixelPosition(xAxis.Minimum);
                        }
                        else
                        {
                            xMAXP = (int)xAxis.ValueToPixelPosition((int)(
                                        (int)xAxis.PixelPositionToValue(e.Location.X)));
                        }
                        using (Graphics g = chart.CreateGraphics())
                            g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(
                                new Point(xMINP, 0), new Point(xMAXP, e.Location.Y), chart, chartIndex));
                        drawOppositeRectangle(xAxis, xMINP, e);
                    }
                }
            }
        }

        /// <summary>
        /// Handle the event when mouse up occurs
        /// Specifically deals with selecting data points
        /// Painting selected points
        /// Drawing rectangle on selected points
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event e</param>
        private void chart_MouseUp(object sender, MouseEventArgs e)
        {
            mUp = e.Location;
            int currentScrollPosAdjust = (CurrentScrollPos[ChartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];

            incrementor.Enabled = false;
            decrementor.Enabled = false;
            incrementorSpeed = 1;
            decrementorSpeed = 1;
            if (isInChart)
            {
                SelectedIndex = ChartIndex;
                Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
                Axis yAxis = chart.ChartAreas[ChartIndex].AxisY;
                Rectangle rect = getRectangle(mdown, e.Location, chart, chartIndex);
                if (e.Location.Y < yAxis.ValueToPixelPosition(yAxis.Minimum)
                    && e.Location.Y > yAxis.ValueToPixelPosition(yAxis.Maximum))
                {
                    yMax[ChartIndex] = yAxis.PixelPositionToValue(e.Location.Y);
                }
                else if (e.Location.Y < yAxis.ValueToPixelPosition(yAxis.Maximum))
                {
                    yMax[ChartIndex] = yAxis.Maximum;
                }
                if (e.Location.X < (int)xAxis.ValueToPixelPosition(chart.Series[ChartIndex].Points[currentScrollPosAdjust].XValue))
                {
                    if (xMax[chartIndex] > xMin[chartIndex])
                    {
                        xMin[ChartIndex] = (int)chart.Series[ChartIndex].Points[currentScrollPosAdjust].XValue / CurrentCompressLevel[chartIndex];
                    }
                    else
                    {
                        xMin[ChartIndex] = (int)chart.Series[ChartIndex].Points[currentScrollPosAdjust].XValue / CurrentCompressLevel[chartIndex];
                        xMax[chartIndex] = (int)(Math.Round(xAxis.PixelPositionToValue(mdown.X)) / CurrentCompressLevel[chartIndex]);
                    }
                }
                else
                {
                    //instant draw version
                    //int curMax = chart.Series[chartIndex].Points.Count - 1 + currentScrollPos[chartIndex];
                    int curMax = currentScrollPosAdjust + viewSize[chartIndex] - 1;
                    int curMin = currentScrollPosAdjust;
                    if (e.Location.X > (int)xAxis.ValueToPixelPosition(curMax * CurrentCompressLevel[chartIndex]))
                    {
                        xMax[ChartIndex] = curMax;
                    }
                    else if (e.Location.X < (int)xAxis.ValueToPixelPosition(curMin * CurrentCompressLevel[chartIndex]))
                    {
                        xMax[ChartIndex] = curMin;
                    }
                    else
                    {
                        xMax[ChartIndex] = (int)(Math.Round(xAxis.PixelPositionToValue(e.Location.X)) / CurrentCompressLevel[chartIndex]);
                    }
                    if (chartType[chartIndex] == 1 && xMax[ChartIndex] > (int)(xAxis.Maximum / 2.0) / CurrentCompressLevel[chartIndex])
                    {
                        xMax[ChartIndex] = (int)(xAxis.Maximum / 2.0) / CurrentCompressLevel[chartIndex];
                    }

                }

                // Check if selection is in reverse
                if (xMin[ChartIndex] > xMax[ChartIndex])
                {
                    int temp = xMin[ChartIndex];
                    xMin[ChartIndex] = xMax[ChartIndex];
                    xMax[ChartIndex] = temp;
                }

                if (e.Location.X == mdown.X && chartType[ChartIndex] == 0)
                {
                    Axis x = chart.ChartAreas[ChartIndex].AxisX;
                    int xMINP = (int)x.ValueToPixelPosition(xMin[ChartIndex] * CurrentCompressLevel[chartIndex]);
                    int xDraw = xMINP;
                    paintSelected(ChartIndex);
                    chart.Refresh();
                    using (Graphics g = chart.CreateGraphics())
                        g.FillRectangle(lineBrushes[ChartIndex], getRectangle(new Point(xDraw - 1, 0), new Point(xDraw + 1, 0), chart, chartIndex));
                    if (chartPair[chartIndex] != -1)
                    {
                        using (Graphics g = chart.CreateGraphics())
                            g.FillRectangle(lineBrushes[ChartIndex], getRectangle(new Point(xDraw - 1, 0), new Point(xDraw + 1, 0), chart, chartPair[chartIndex]));
                    }
                }
                else
                {
                    paintSelected(chartIndex);
                    updateSelectedRectFill(chartIndex);
                }
                isInChart = false;
            }
            else
            {
                updateSelectedRectFill(chartIndex);
            }
        }

        /// <summary>
        /// Handle the event when mouse wheel occurs for zooming
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void chart_MouseWheel(object sender, MouseEventArgs e)
        {
            mdown = e.Location;
            trySetCurrentChartIndex();
            if (isInChart)
            {
                int[] uncompressedZoomRange = getUncompressedZoomRange(datas[chartIndex].Length);
                if (ZoomPos[chartIndex] < pixelc[chartIndex].compressionLevel.Length -
                    1 + uncompressedZoomRange.Length && ZoomPos[chartIndex] >= 0)
                {
                    if (e.Delta / 120 > 0 || (e.Delta / 120 < 0 && ZoomPos[chartIndex] + e.Delta / 120 >= 0))
                    {
                        ZoomPos[chartIndex] += e.Delta / 120;

                        //datas[chartIndex] = ArrayTransform.listDataToDouble(pixelc[chartIndex].dataArray[zoomPos[chartIndex]]);
                        int retrieveIndex = pixelc[chartIndex].compressionLevel.Length - 1 - ZoomPos[chartIndex];
                        int uncompressedRetrieveIndex = uncompressedZoomRange.Length - 1 + retrieveIndex;
                        if (retrieveIndex >= 0)
                        {
                            DataPoints[chartIndex] = pixelc[chartIndex].dataArray[retrieveIndex];
                            CurrentCompressLevel[chartIndex] = pixelc[chartIndex].compressionLevel[retrieveIndex];
                        }

                        // max zoomed out
                        if (ZoomPos[chartIndex] == 0)
                        {
                            viewSize[chartIndex] = DataPoints[chartIndex].Count;
                            SoundEditorOptimizeObject.BlockSize = datas[chartIndex].Length;
                            blockSize[chartIndex] = datas[chartIndex].Length;
                            SoundEditorOptimizeObject.fillChart(chartIndex, chart.Series[chartIndex].Name);
                        }
                        // zoomed in with compression
                        else if (retrieveIndex > 0)
                        {
                            viewSize[chartIndex] = DataPoints[chartIndex].Count / CurrentCompressLevel[chartIndex];
                            SoundEditorOptimizeObject.BlockSize = (viewSize[chartIndex] - 1)/* * CurrentCompressLevel[chartIndex]*/;
                            blockSize[chartIndex] = SoundEditorOptimizeObject.BlockSize;
                            SoundEditorOptimizeObject.fillChart(chartIndex, chart.Series[chartIndex].Name);
                        }
                        else if (retrieveIndex == 0)
                        {
                            viewSize[chartIndex] = uncompressedZoomRange[uncompressedRetrieveIndex] + 1;
                            SoundEditorOptimizeObject.BlockSize = viewSize[chartIndex] - 1;
                            blockSize[chartIndex] = SoundEditorOptimizeObject.BlockSize;
                            SoundEditorOptimizeObject.fillChart(chartIndex, chart.Series[chartIndex].Name);
                        }
                        else
                        {
                            viewSize[chartIndex] = uncompressedZoomRange[uncompressedRetrieveIndex] + 1;
                            SoundEditorOptimizeObject.BlockSize = viewSize[chartIndex] - 1;
                            blockSize[chartIndex] = SoundEditorOptimizeObject.BlockSize;
                            chart.ChartAreas[ChartIndex].AxisX.ScaleView.Zoom(0/*scrollPos[ChartIndex]*/, BlockSize[chartIndex] - 1 /*+ scrollPos[ChartIndex] - 1*/);
                            chart.ChartAreas[ChartIndex].AxisX.ScaleView.SmallScrollSize = BlockSize[chartIndex] - 2;
                            chart.ChartAreas[ChartIndex].AxisX.ScaleView.Size = BlockSize[chartIndex];
                            //SoundEditorOptimizeObject.fillChart(chartIndex, chart.ChartAreas[chartIndex].Name);
                        }
                    }
                }
                updateSelectedRectFill(chartIndex);
            }
        }

        /// <summary>
        /// Gets uncompressed zoom range for scroll to zoom
        /// </summary>
        /// <param name="dataLength">Data length</param>
        /// <returns>An array of zoom range</returns>
        private int[] getUncompressedZoomRange(int dataLength)
        {
            int[] maxUncompressedZoomRange = new int[19];
            int[] uncompressedZoomRange = null;
            int levels = 0;
            int zoomSize = 0;
            int index = 0;
            while (dataLength > zoomSize && zoomSize < 1000)
            {
                if (zoomSize < 10)
                {
                    levels++;
                    zoomSize++;
                }
                else if (zoomSize == 10)
                {
                    levels++;
                    zoomSize = 25;
                }
                else if (zoomSize < 100)
                {
                    levels++;
                    zoomSize += 25;
                }
                else if (zoomSize == 100)
                {
                    levels++; ;
                    zoomSize = 250;
                }
                else if (zoomSize < 1000)
                {
                    levels++;
                    zoomSize += 250;
                }
                maxUncompressedZoomRange[index] = zoomSize;
                index++;
            }
            uncompressedZoomRange = new int[levels];
            for (int i = 0; i < levels; i++)
            {
                uncompressedZoomRange[i] = maxUncompressedZoomRange[i];
            }
            return uncompressedZoomRange;
        }

        /// <summary>
        /// Return a rectangle that suits the current chartArea and situation
        /// </summary>
        /// <param name="p1">Point position one</param>
        /// <param name="p2">Point position two</param>
        /// <param name="chart">Chart object</param>
        /// <returns>Rectangle object</returns>
        private Rectangle getRectangle(Point p1, Point p2, Chart chart, int chartIndex)
        {
            Axis yAxis = chart.ChartAreas[chartIndex].AxisY;
            Axis xAxis = chart.ChartAreas[chartIndex].AxisX;
            int yMinP = (int)yAxis.ValueToPixelPosition(yAxis.Minimum);
            int yMaxP = (int)yAxis.ValueToPixelPosition(yAxis.Maximum);
            int currentScrollPosAdjust = (CurrentScrollPos[chartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
            if (chartType[chartIndex] == 1)
            {
                yMaxP = (int)yAxis.ValueToPixelPosition(yMax[chartIndex]);
            }
            int xMINP = (int)xAxis.ValueToPixelPosition(
                chart.Series[chartIndex].Points[currentScrollPosAdjust].XValue);
            int xMAXP = 0;
            if (currentScrollPosAdjust + viewSize[chartIndex] - 1 < chart.Series[chartIndex].Points.Count - 1)
            {
                xMAXP = (int)xAxis.ValueToPixelPosition(
                    chart.Series[chartIndex].Points[currentScrollPosAdjust + viewSize[chartIndex] - 1].XValue);
            }
            else
            {
                xMAXP = (int)xAxis.ValueToPixelPosition(
                    chart.Series[chartIndex].Points[chart.Series[chartIndex].Points.Count - 1].XValue);
            }

            //  . | . |
            if (p2.X < xMINP)
            {

                return new Rectangle(Math.Min(p1.X, xMINP), yMaxP,
                    Math.Abs(p1.X - xMINP), Math.Abs(yMaxP - yMinP - 1));
            }
            // | . | .
            else if (p2.X > xMAXP)
            {
                return new Rectangle(Math.Min(p1.X, xMAXP), yMaxP,
                    Math.Abs(p1.X - xMAXP), Math.Abs(yMaxP - yMinP - 1));
            }
            // | . . |
            else
            {
                return new Rectangle(Math.Min(p1.X, p2.X), yMaxP,
                    Math.Abs(p1.X - p2.X), Math.Abs(yMaxP - yMinP - 1));
            }
        }

        /// <summary>
        /// Call copying method based based on stereo or mono
        /// </summary>
        public void copy()
        {
            if (chartPair[chartIndex] != -1)
            {
                copying(chartPair[chartIndex]);
            }
            copying(chartIndex);
        }

        /// <summary>
        /// Copy the selected points and store them
        /// </summary>
        /// <param name="chartIndex"></param>
        public void copying(int chartIndex)
        {
            cXMax = xMax[chartIndex];
            cXMin = xMin[chartIndex];
            int xMaxA = xMax[chartIndex] * currentCompressLevel[chartIndex];
            int xMinA = xMin[chartIndex] * currentCompressLevel[chartIndex];
            copiedData = new double[xMaxA - xMinA + 1];
            for (int i = xMinA, j = 0; i <= xMaxA; i++, j++)
            {
                copiedData[j] = datas[chartIndex][i];
            }
            selectedIndex = chartIndex;
        }
        
        /// <summary>
        /// Cut the selected points by calling copy and delete
        /// </summary>
        public void cut()
        {
            copy();
            delete();
        }

        /// <summary>
        /// Delete selected points baesd on stereo or mono
        /// </summary>
        public void delete()
        {
            if (chartPair[chartIndex] != -1)
            {
                deleting(chartPair[chartIndex]);
            }
            deleting(chartIndex);
        }

        /// <summary>
        /// Delete selected points
        /// </summary>
        /// <param name="chartIndex">current chart index</param>
        public void deleting(int chartIndex)
        {
            if (xMax[ChartIndex] != -1 && xMin[ChartIndex] != -1 &&
                xMax[ChartIndex] - xMin[ChartIndex] > 0)
            {
                int xMaxA = xMax[ChartIndex] * currentCompressLevel[chartIndex];
                int xMinA = xMin[ChartIndex] * currentCompressLevel[chartIndex];
                double[] newData = new double[datas[ChartIndex].Length -
                    xMaxA + xMinA];
                int index = 0;
                for (int i = 0; i < xMinA; i++)
                {
                    newData[index] = datas[ChartIndex][i];
                    index++;
                }
                for (int i = xMaxA + currentCompressLevel[chartIndex]; i < newData.Length; i++)
                {
                    newData[index] = datas[ChartIndex][i];
                    index++;
                }

                soundEditorOptimizeObject.BlockSize = newData.Length - 1;
                ViewSize[chartIndex] = newData.Length;
                BlockSize[chartIndex] = newData.Length - 1;
                pixelCompress(chartIndex, newData);
                soundEditorOptimizeObject.fillChart(chartIndex, chart.Series[chartIndex].Name);
            }
            archiveManager.saveAction(ChartIndex, datas[ChartIndex]);
            soundEditorOptimizeObject.Modified = true;
        }

        /// <summary>
        /// Delete selected data from selected chart
        /// Paste copied data to selected chart
        /// Based on stereo or mono
        /// </summary>
        public void paste()
        {
            if (chartPair[chartIndex] != -1)
            {
                pasting(chartPair[chartIndex]);
            }
            pasting(chartIndex);
        }

        /// <summary>
        /// Delete selected data from selected chart and paste copied data to the selected chart
        /// </summary>
        /// <param name="chartIndex"></param>
        public void pasting(int chartIndex)
        {
            deleting(chartIndex);
            if (cXMax - cXMin > 0)
            {
                int xMinA = xMin[chartIndex] * currentCompressLevel[chartIndex];
                double[] newData = new double[datas[chartIndex].Length + copiedData.Length];
                int index = 0;
                for (int i = 0; i < xMinA; i++)
                {
                    newData[i] = datas[chartIndex][i];
                    index++;
                }
                for (int i = xMinA, j = 0; i < copiedData.Length + xMinA; i++, j++)
                {
                    newData[i] = copiedData[j];
                }
                for (int i = xMinA + copiedData.Length + 1; i < newData.Length; i++)
                {
                    newData[i] = datas[chartIndex][index];
                    index++;
                }
                soundEditorOptimizeObject.BlockSize = newData.Length - 1;
                ViewSize[chartIndex] = newData.Length;
                BlockSize[chartIndex] = newData.Length - 1;
                pixelCompress(chartIndex, newData);
                soundEditorOptimizeObject.fillChart(chartIndex, chart.Series[chartIndex].Name);
            }
            archiveManager.saveAction(ChartIndex, datas[ChartIndex]);
        }

        /// <summary>
        /// Select all data of selected chart based on chart type
        /// </summary>
        public void selectAll()
        {
            if (chartType[chartIndex] == 0)
            {
                xMin[chartIndex] = 0;
                xMax[chartIndex] = chart.Series[chartIndex].Points.Count - 1;
                paintSelected(chartIndex);
                updateSelectedRectFill(chartIndex);
            }
            else if (chartType[chartIndex] == 1)
            {
                xMin[chartIndex] = 0;
                xMax[chartIndex] = (int)(chart.Series[chartIndex].Points.Count - 1) / 2;
                paintSelected(chartIndex);
                updateSelectedRectFill(chartIndex);
            }
        }

        /// <summary>
        /// Paint selected point based on stereo or mono
        /// </summary>
        /// <param name="index">Index to the chartArea</param>
        public void paintSelected(int index)
        {
            paintingSelected(index);
            if (chartPair[index] != -1)
            {
                xMax[chartPair[index]] = xMax[chartIndex];
                xMin[chartPair[index]] = xMin[chartIndex];
                paintingSelected(chartPair[index]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void paintingSelected(int index)
        {
            for (int i = 0; i < chart.Series[index].Points.Count; i++)
            {
                if (i >= xMin[index] && i <= xMax[index])
                {
                    chart.Series[index].Points[i].Color = selectedColors[index];
                }
                else
                {
                    chart.Series[index].Points[i].Color = themeColors[index];
                }
            }
            if (chartType[index] == 1)
            {
                int xMAXP = chart.Series[index].Points.Count - 1;
                int xMINP = xMAXP - xMax[index] + 1;
                for (int i = xMINP; i <= xMAXP; i++)
                {
                    chart.Series[index].Points[i].Color = selectedColors[index];
                }
            }
        }

        /// <summary>
        /// Draw opposite rectangle for frequency domain type of chartArea
        /// </summary>
        /// <param name="xAxis">The xAxis of chartArea</param>
        /// <param name="xMINP">The minimum X position of chartArea</param>
        /// <param name="e">Event</param>
        private void drawOppositeRectangle(Axis xAxis, int xMINP, MouseEventArgs e)
        {
            // _||_
            int currentScrollPosAdjust = (CurrentScrollPos[chartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
            int xMAXP = 0;
            if (currentScrollPosAdjust + viewSize[chartIndex] - 1 < chart.Series[ChartIndex].Points.Count - 1)
            {
                xMAXP = (int)xAxis.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[currentScrollPosAdjust + viewSize[chartIndex] - 1].XValue);
            }
            else
            {
                xMAXP = (int)xAxis.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[chart.Series[ChartIndex].Points.Count - 1].XValue);
            }
            // | __ |
            int cPos = (int)xAxis.Minimum;
            if (e.Location.X > xAxis.ValueToPixelPosition(xAxis.Minimum) &&
                e.Location.X < xAxis.ValueToPixelPosition(xAxis.Maximum))
            {
                cPos = (int)xAxis.PixelPositionToValue(e.Location.X);
            }
            if (cPos <= (int)xAxis.Minimum)
            {
                xMINP = (int)xAxis.ValueToPixelPosition((int)xAxis.Maximum);
            }
            else if (cPos >= (int)xAxis.Maximum / 2)
            {
                xMINP = (int)xAxis.ValueToPixelPosition(
                            (int)Math.Round(xAxis.Maximum / 2.0) + 1);
            }
            else
            {
                xMINP = (int)xAxis.ValueToPixelPosition((int)(xAxis.Maximum -
                        (int)xAxis.PixelPositionToValue(e.Location.X) + 1));
            }
            using (Graphics g = chart.CreateGraphics())
                g.DrawRectangle(new Pen(brushes[ChartIndex]), getRectangle(
                    new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartIndex));
        }

        /// <summary>
        /// Draws a rectangle on top of selected data points
        /// </summary>
        public void updateSelectedRectFill(int chartIndex)
        {
            int currentScrollPosAdjust = (
                CurrentScrollPos[ChartIndex] - currentCompressLevel[chartIndex]) / CurrentCompressLevel[chartIndex];
            if (CurrentScrollPos[ChartIndex] < 1)
            {
                currentScrollPosAdjust++;
            }
            Axis xAxis = chart.ChartAreas[ChartIndex].AxisX;
            DataPointCollection csp = chart.Series[ChartIndex].Points;

            int curMaxView = currentScrollPosAdjust + viewSize[chartIndex] - 1;
            if (curMaxView > csp.Count - 1)
            {
                curMaxView = csp.Count - 1;
            }
            int xMINP = (int)xAxis.ValueToPixelPosition(
                csp[currentScrollPosAdjust].XValue);
            int xMAXP = xMINP;
            if (xMin[chartIndex] < 0 && xMax[chartIndex] < 0)
            {
            }
            else if (xMax[chartIndex] < currentScrollPosAdjust) // . . |  | selected point scenario vs view window
            {
            }
            else if (xMin[chartIndex] <= currentScrollPosAdjust && xMax[chartIndex] >
                currentScrollPosAdjust && xMax[chartIndex] < curMaxView)  // . |  . |
            {
                xMINP = (int)xAxis.ValueToPixelPosition(csp[currentScrollPosAdjust].XValue);
                xMAXP = (int)xAxis.ValueToPixelPosition(csp[xMax[chartIndex]].XValue);
            }
            else if (xMin[chartIndex] <= currentScrollPosAdjust && xMax[chartIndex] >=
                curMaxView)                 // . |   | .
            {
                xMINP = (int)xAxis.ValueToPixelPosition(csp[currentScrollPosAdjust].XValue);
                xMAXP = (int)xAxis.ValueToPixelPosition(csp[curMaxView].XValue);
            }
            else if (xMin[chartIndex] >= currentScrollPosAdjust && xMax[chartIndex] >
                currentScrollPosAdjust && xMax[chartIndex] < curMaxView)  // | . . |
            {
                xMINP = (int)xAxis.ValueToPixelPosition(csp[xMin[chartIndex]].XValue);
                xMAXP = (int)xAxis.ValueToPixelPosition(csp[xMax[chartIndex]].XValue);
            }
            else if (xMin[chartIndex] >= currentScrollPosAdjust && xMin[chartIndex] <
                curMaxView && xMax[chartIndex] >= curMaxView) // | . | .
            {
                xMINP = (int)xAxis.ValueToPixelPosition(csp[xMin[chartIndex]].XValue);
                xMAXP = (int)xAxis.ValueToPixelPosition(csp[curMaxView].XValue);
            }
            else if (xMin[chartIndex] > currentScrollPosAdjust &&
                xMax[chartIndex] > currentScrollPosAdjust) // | | . .
            {
            }
            chart.Refresh();
            using (Graphics g = chart.CreateGraphics())
                g.FillRectangle(brushes[ChartIndex],
                    getRectangle(new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartIndex));

            if (chartPair[chartIndex] != -1)
            {
                using (Graphics g = chart.CreateGraphics())
                    g.FillRectangle(brushes[ChartIndex],
                        getRectangle(new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartPair[chartIndex]));
            }
            if (chartType[ChartIndex] == 1 && xMax[ChartIndex] != 0)
            {
                xMAXP = (int)xAxis.ValueToPixelPosition(
                    chart.Series[ChartIndex].Points[chart.Series[ChartIndex].Points.Count - 1].XValue);
                xMINP = (int)xAxis.ValueToPixelPosition((int)(
                    chart.Series[ChartIndex].Points[chart.Series[ChartIndex].Points.Count - 1].XValue -
                    (xMax[ChartIndex] * currentCompressLevel[chartIndex]) + 1));
                using (Graphics g = chart.CreateGraphics())
                    g.FillRectangle(brushes[ChartIndex], getRectangle(new Point(xMINP, 0), new Point(xMAXP, 0), chart, chartIndex));
            }
        }

        /// <summary>
        /// Refreshes all chartAreas
        /// </summary>
        public void refreshCharts()
        {
            for (int i = 0; i < chart.ChartAreas.Count; i++)
            {
                if (prevScrollPos[i] == 0)
                {
                    prevScrollPos[i]++;
                }
                CurrentScrollPos[i] = (int)chart.ChartAreas[i].AxisX.ScaleView.Position;
                prevScrollPos[i] = CurrentScrollPos[i];
                if (xMin[i] != -1 && xMax[i] != -1)
                {
                    paintSelected(i);
                }
            }
        }
    }
}
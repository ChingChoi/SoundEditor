//
// Author: Choi, Ching
//
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;
using SoundEditorSelector;
using System.Threading;
using System.IO;

/// <summary>
/// Hold info of minimum number of data before calling a thread
/// and number of threads to be used
/// </summary>
public struct ThreadSetting
{
    public static int THREAD_THRESHOLD = 5000;
    public static int threadNum = 4;
}

/// <summary>
/// Hold toggle flag of menuStripItem
/// </summary>
public struct FlagSetting
{
    public static bool runLengthEncoding = false;
}

/// <summary>
/// Header info for audio file for playback
/// </summary>
public struct AudioHeaderInfo
{
    public uint wFormatTag;
    public uint nChannels;
    public uint nSamplesPerSec;
    public uint nAvgBytesPerSec;
    public uint nBlockAlign;
    public uint wBitsPerSample;
    public uint cbSize;
}

/// <summary>
/// Main namespace
/// </summary>
namespace SoundEditorOptimize
{
    /// <summary>
    /// Main partial class of window form that deals with the GUI
    /// </summary>
    public partial class SoundEditorOptimize : Form
    {
        /// <summary>
        /// For overriding
        /// </summary>
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;
        /// <summary>
        /// position offset
        /// </summary>
        int closeFormHorzOffset = 30;
        int panel1VertOffest = 25;
        int chartVertOffset = 25;
        int recorderOffset = 0;
        int maxformHorzOffset = 60;
        int minformHorzOffset = 90;
        /// <summary>
        /// recorder
        /// </summary>
        uint dllDataSize;
        IntPtr dllData;
        byte[] originalRecord;
        byte[] modifiedRecord;
        int setVolume;
        int curVolume;
        int toBeProcessedPos;
        int processedPos;
        bool isRecordedData;
        bool modified;
        System.Windows.Forms.Timer timer;
        DateTime startTime;
        DateTime endTime;
        Panel recorderPanel;
        CustomButton record;
        CustomButton endRecord;
        CustomButton playBegin;
        CustomButton playEnd;
        CustomButton playPause;
        CustomButton playSpeed;
        CustomButton playReverse;
        CustomButton playRepeat;
        CustomSlider customSlider;
        AudioHeaderInfo audioHeaderInfo;
        /// <summary>
        /// threading
        /// </summary>
        Panel threadingPanel;
        CustomButton beginThreadTest;
        CustomButton addThreadCount;
        CustomButton subtractThreadCount;
        CustomButton performanceTime;
        Label currentThreadCount;
        /// <summary>
        /// Windowing
        /// </summary>
        bool rectangleWindow;
        bool triangleWindow;
        bool polynomialWindow;
        /// <summary>
        /// main UI and control
        /// </summary>
        int N;
        int frequencyOne;
        int frequencyTwo;
        int scrollPos;
        int blockSize;
        int viewSize;
        int chartIndex;
        int numOfChartArea;
        int chartPixelWidth;
        double[] data;
        int index;
        bool runRecorder;
        bool runThreading;
        bool newChart;
        Color themeColor;
        Color themeColorR;
        Color themeColor2;
        Color themeColor3;
        Color themeColor4;
        Color selectedColor;
        Color selectedColorR;
        Color selectedColor2;
        Color selectedColor3;
        Color selectedColor4;
        Color themeBackgroundColor;
        SolidBrush brushR;
        SolidBrush brush2;
        SolidBrush brush3;
        SolidBrush brush4;
        SolidBrush lineBrushR;
        SolidBrush lineBrush2;
        SolidBrush lineBrush3;
        SolidBrush lineBrush4;
        Selection selectionObject;
        AudioFileManager am;
        /// <summary>
        /// Constructor
        /// </summary>
        public SoundEditorOptimize()
        {
            // Color LightGreen, PaleTorquoise
            this.N = 100;
            this.index = 0;
            this.frequencyOne = 3;
            this.frequencyTwo = 1;
            this.scrollPos = 1;
            this.BlockSize = 100;
            this.viewSize = BlockSize + 1;
            this.runRecorder = false;
            this.runThreading = false;
            this.isRecordedData = false;
            this.modified = false;
            this.ThemeColor = Color.FromArgb(200, 144, 238, 144);
            this.ThemeColorR = Color.FromArgb(200, 144, 144, 238);
            this.ThemeColor2 = Color.FromArgb(200, 154, 28, 23);
            this.ThemeColor3 = Color.FromArgb(200, 194, 87, 26);
            this.ThemeColor4 = Color.FromArgb(200, 130, 147, 86);
            this.SelectedColor = Color.Cyan;
            this.SelectedColorR = Color.Cyan;
            this.SelectedColor2 = Color.FromArgb(200, 245, 139, 76);
            this.SelectedColor3 = Color.FromArgb(200, 239, 212, 105);
            this.SelectedColor4 = Color.FromArgb(200, 67, 171, 201);
            this.ThemeBackgroundColor = Color.FromArgb(175, 0, 0, 0);
            this.BrushR = new SolidBrush(Color.FromArgb(50, 0, 255, 255));
            this.brush2 = new SolidBrush(Color.FromArgb(100, 245, 139, 76));
            this.brush3 = new SolidBrush(Color.FromArgb(100, 239, 212, 105));
            this.brush4 = new SolidBrush(Color.FromArgb(100, 67, 171, 201));
            this.LineBrushR = new SolidBrush(Color.Cyan);
            this.lineBrush2 = new SolidBrush(Color.FromArgb(255, 245, 139, 76));
            this.lineBrush3 = new SolidBrush(Color.FromArgb(255, 239, 212, 105));
            this.lineBrush4 = new SolidBrush(Color.FromArgb(255, 67, 171, 201));
            this.data = new double[this.BlockSize * 30];
            this.chartIndex = 0;
            this.numOfChartArea = 0;
            this.rectangleWindow = true;
            this.newChart = false;

            for (int i = 0; i < BlockSize * 30; i++)
            {
                data[i] = 3 * Math.Cos(2 * Math.PI * frequencyOne * i / N) + 2 * Math.Cos(2 * Math.PI * i * frequencyTwo / N);
            }
            InitializeComponent();
            chart.BackColor = Color.FromArgb(0, 0, 0, 0);
            selectionObject = new Selection(this, chart, data, scrollPos, BlockSize, ThemeColor, chartIndex);

            this.am = new AudioFileManager(selectionObject.MaxChartSize);

            selectionObject.pixelCompress(index, data);
            selectionObject.ViewSize[index] = blockSize + 1;
            selectionObject.ZoomPos[chartIndex] = 4;
            chart.ChartAreas[0].AxisX.Title = "Time";
            chart.ChartAreas[0].AxisY.Title = "Amplitude";
            chart.ChartAreas[0].AxisX.TitleForeColor = themeColor;
            chart.ChartAreas[0].AxisY.TitleForeColor = themeColor;
            fillChart(chartIndex, "Sound Wave");
            chartAreaThemeColor(chart.ChartAreas[numOfChartArea], ThemeColor, SelectedColor,
                selectionObject.Brushes[chartIndex], selectionObject.LineBrushes[chartIndex], numOfChartArea);
            numOfChartArea++;
            Title.ReadOnly = true;
            Title.Enabled = false;

            customizeMenuStrip(menuStrip1);

            closeForm.TabStop = false;
            closeForm.FlatStyle = FlatStyle.Flat;
            closeForm.FlatAppearance.BorderSize = 0;
            closeForm.ForeColor = ThemeColor;
            minForm.ForeColor = ThemeColor;
            maxForm.ForeColor = ThemeColor;
            Title.ForeColor = ThemeColor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        /// <summary>
        /// Override WndProc to allow for resizing
        /// </summary>
        /// <param name="m">Message for WndProc</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)(HT_CAPTION);
            }
        }

        /// <summary>
        /// Add a chartArea to chart
        /// </summary>
        /// <param name="seriesName">Series name</param>
        /// <param name="type">Series chart type</param>
        public void addChart(string seriesName, SeriesChartType type)
        {
            try
            {
                int tempSize = 100;
                // chart
                ChartArea chartArea = new ChartArea();
                chartArea.Name = "ChartArea" + (numOfChartArea + 1);
                chart.ChartAreas.Add(chartArea);
                // legend
                Legend legend = new Legend();
                legend.Name = "Legend" + numOfChartArea + 1;
                chart.Legends.Add(legend);
                legend.BackColor = ThemeBackgroundColor;
                legend.DockedToChartArea = "ChartArea" + (numOfChartArea + 1);
                legend.IsDockedInsideChartArea = true;
                // series
                Series series = new Series();
                series.Legend = legend.Name;
                series.Name = seriesName;
                series = chart.Series.Add(seriesName);
                chart.Series[series.Name].Legend = legend.Name;
                series.ChartType = type;
                series.ChartArea = "ChartArea" + (numOfChartArea + 1);
                series.XValueType = ChartValueType.Int32;
                for (int i = 0; i < tempSize; i++)
                {
                    chart.Series[numOfChartArea].Points.AddXY(i, 0);
                }
                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Maximum = tempSize;
                chartArea.AxisX.ScaleView.Zoomable = true;
                chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
                chartArea.AxisX.ScaleView.Zoom(0, tempSize);
                chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
                chartArea.AxisX.ScaleView.SmallScrollSize = tempSize;
                chartArea.AxisY.LineWidth = 1;
                chartArea.AxisX.LineWidth = 1;
                chartArea.AxisX.Title = "Time";
                chartArea.AxisY.Title = "Amplitude";
                chartArea.BackColor = ThemeBackgroundColor;
                numOfChartArea++;
            }
            catch (System.ArgumentException e)
            {
            }
        }

        public void removeChart(string seriesName)
        {
            int index = chart.Series.IndexOf(seriesName);
            if (index != -1)
            {
                chart.ChartAreas.Remove(chart.ChartAreas[index]);
                chart.Series.Remove(chart.Series[index]);
                chart.Legends.Remove(chart.Legends[index]);
                numOfChartArea--;
            }
        }

        /// <summary>
        /// Fill a specific chartArea with transformed data
        /// </summary>
        /// <param name="index">Index of the chartArea</param>
        /// <param name="seriesName">Series name of the chart</param>
        /// <param name="input">Input data</param>
        /// <param name="chartType">Define if chart is time doamin type or frequency domain type</param>
        /// <param name="chartBelong">Specify which chartArea index this chartArea belongs to</param>
        public void fillChartTransform(int index, string seriesName, double[] input, int chartType, int chartBelong)
        {
            var chartArea = chart.ChartAreas[index];
            chart.Series[index].ChartType = SeriesChartType.RangeColumn;
            selectionObject.ArchiveManager.Datas[index] = selectionObject.Datas[index];
            selectionObject.ChartType[index] = chartType;
            selectionObject.ChartBelong[index] = chartBelong;
            selectionObject.XMax[index] = 0;
            selectionObject.XMin[index] = 0;

            chart.Series[index].Points.Clear();
            for (int i = 0; i < selectionObject.DataPoints[index].Count; i++)
            {
                chart.Series[index].Points.Add(selectionObject.DataPoints[index][i]);
            }

            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = input.Length - 1;

            double yMax = 0;
            double yMin = 0;

            foreach (double dp in input)
            {
                if (dp > yMax)
                {
                    yMax = dp;
                }
                else if (dp < yMin)
                {
                    yMin = dp;
                }
            }

            double yAxisMax = 0;
            double yAxisMin = 0;
            double yAxisMaxCur = 0.01;
            double yAxisMinCur = 0;
            if (yMax > yAxisMaxCur && yMax > 0)
            {
                if (yMax > 1)
                {
                    yAxisMax = Math.Truncate(yMax + yMax / 2);
                    while (yAxisMax < yMax)
                    {
                        yAxisMax++;
                    }
                }
                else
                {
                    yAxisMaxCur = Math.Truncate(yMax * 100) / 100;
                    yAxisMax = yAxisMaxCur + yAxisMaxCur / 2;
                    if (yAxisMax > 1 && yAxisMax > yMax)
                    {
                        yAxisMax = Math.Truncate(yAxisMax);
                    }
                }
            }
            if (yAxisMax == 0)
            {
                yAxisMax = 0.01;
            }
            if (yMin < yAxisMinCur && yMin < 0)
            {
                if (yMin < -1)
                {
                    yAxisMin = Math.Truncate(yMin + yMin / 2);
                }
                else
                {
                    yAxisMinCur = Math.Truncate(yMin * 100) / 100;
                    yAxisMin = yAxisMinCur + yAxisMinCur / 2;
                    if (yAxisMin > 1 && yAxisMin > yMin)
                    {
                        yAxisMin = Math.Truncate(yAxisMin);
                    }
                }
            }

            if (yAxisMax > yAxisMin * -1 && yAxisMin != 0)
            {
                yAxisMin = -1 * yAxisMax;
            }
            else if (yAxisMin == 0)
            {
            }
            else
            {
                yAxisMax = -1 * yAxisMin;
            }

            chartArea.AxisY.Maximum = yAxisMax;
            chartArea.AxisY.Minimum = yAxisMin;

            double interval = yAxisMax;

            chartArea.AxisY.MajorGrid.Interval = interval;
            chartArea.AxisY.MajorGrid.LineWidth = 2;
            chartArea.AxisX.MajorGrid.LineWidth = 2;
            chartArea.AxisY.Interval = interval;
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chartArea.AxisX.ScaleView.Zoom(1, input.Length - 1);

            int pos = (int)chartArea.AxisX.ScaleView.Position;

            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chartArea.AxisX.ScrollBar.Size = 15;
            chartArea.AxisX.ScaleView.SmallScrollSize = input.Length - 2;

            chart.Series[index].Name = seriesName;
            BackColor = Color.FromArgb(35, 35, 35);
            if (index != selectionObject.SelectedIndex)
            {
                selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
            }

            this.selectionObject.refreshCharts();
            this.selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
        }

        /// <summary>
        /// Fill a specific chartArea with data
        /// </summary>
        /// <param name="index">Index of the chartArea</param>
        /// <param name="seriesName">Series name</param>
        public void fillChart(int index, string seriesName)
        {
            var chartArea = chart.ChartAreas[index];

            chart.Series[index].Points.Clear();
            chart.Series[index].ChartType = SeriesChartType.RangeColumn;

            if (selectionObject.DataPoints[index] != null)
            {
                for (int i = 0; i < selectionObject.DataPoints[index].Count; i++)
                {
                    chart.Series[index].Points.Insert(i, selectionObject.DataPoints[index][i]);
                }
            }
            else
            {
                for (int i = 0; i < selectionObject.Datas[index].Length; i++)
                {
                    chart.Series[index].Points.AddXY(i, selectionObject.Datas[index][i]);
                }
            }

            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = selectionObject.Datas[index].Length - 1;

            int yAxisMax = 0;
            int yAxisMin = 0;

            foreach (DataPoint dp in chart.Series[index].Points)
            {
                if (dp.YValues[0] > yAxisMax)
                {
                    yAxisMax = (int)Math.Ceiling(dp.YValues[0]);
                }
                else if (dp.YValues.Length > 1 && dp.YValues[1] < yAxisMin)
                {
                    yAxisMin = (int)Math.Floor(dp.YValues[1]);
                }
                else if (dp.YValues[0] < yAxisMin)
                {
                    yAxisMin = (int)Math.Floor(dp.YValues[0]);
                }
            }

            chartArea.AxisY.Maximum = yAxisMax + yAxisMax / 2;
            if (yAxisMin != 0)
            {
                chartArea.AxisY.Minimum = -1 * (yAxisMax + yAxisMax / 2);
            }
            else
            {
                chartArea.AxisY.Minimum = 0;
            }

            double interval = (yAxisMax + yAxisMax / 2);

            chartArea.AxisY.LineWidth = 1;
            chartArea.AxisX.LineWidth = 1;
            chartArea.AxisY.MajorGrid.Interval = interval;
            chartArea.AxisY.MajorGrid.LineWidth = 2;
            chartArea.AxisX.MajorGrid.LineWidth = 2;
            chartArea.AxisY.Interval = interval;

            //chart.ChartAreas["ChartArea1"].CursorX.IsUserEnabled = true;
            //chart.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true;
            //chart.ChartAreas["ChartArea1"].CursorX.AutoScroll = true;

            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            chartArea.AxisX.ScaleView.Zoom(selectionObject.CurrentCompressLevel[index], BlockSize - 1 + selectionObject.CurrentCompressLevel[index]);

            int pos = (int)chartArea.AxisX.ScaleView.Position;

            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chartArea.AxisX.ScrollBar.Size = 15;
            chartArea.AxisX.ScaleView.SmallScrollSize = BlockSize;

            bool ifContain = false;
            for (int i = 0; i < chart.Series.Count; i++)
            {
                if (chart.Series[i].Name == seriesName)
                {
                    ifContain = true;
                }
            }
            if (!ifContain)
            {
                chart.Series[index].Name = seriesName;
            }

            BackColor = Color.FromArgb(35, 35, 35);
            index = BlockSize;
        }

        /// <summary>
        /// Closes the form when closeForm button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void closeForm_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Minimizes the form when minForm button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void minForm_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.selectionObject.paintSelected(chartIndex);
            this.selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
        }

        /// <summary>
        /// Maximize the form when maxForm button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void maxForm_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                //            this.selectionObject.paintSelected(chartIndex);
            }
            this.selectionObject.refreshCharts();
            this.selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
        }

        /// <summary>
        /// Open interface for file selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newChart)
            {
                if (selectionObject.ChartIndex == 0)
                {
                    chartIndex = 0;
                }
                else
                {
                    chartIndex = chart.Series.IndexOf("Sound Wave 2");
                }
            }
            string file = "NULL";
            //int size = -1;
            openFileDialog.Filter = "*wav|* wav";
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                AudioFile audioFile = new AudioFile(file);

                if (String.ReferenceEquals(file, "NULL"))
                {
                    Console.WriteLine("...Failed to load note");
                }
                else if (audioFile.Read)
                {
                    data = new double[audioFile.L1.Length];
                    data = ArrayTransform.convertToDouble(audioFile.L1);
                    BlockSize = data.Length/* / 10*/;
                    viewSize = BlockSize + 1;
                    selectionObject.BlockSize[chartIndex] = BlockSize;
                    selectionObject.ViewSize[chartIndex] = viewSize;
                    selectionObject.Datas[chartIndex] = data;
                    selectionObject.ArchiveManager.Datas[chartIndex] = data;
                    selectionObject.pixelCompress(chartIndex, data);

                    if (audioFile.Channels == 1)
                    {
                        selectionObject.ChartChannel[chartIndex] = 1;
                        selectionObject.ChartPair[chartIndex] = -1;
                        if (!newChart || chartIndex == 0)
                        {
                            removeChart("Sound Wave R");
                            fillChart(chartIndex, "Sound Wave");
                        }
                        else
                        {
                            removeChart("Sound Wave R2");
                            fillChart(chartIndex, "Sound Wave 2");
                        }
                    }
                    else if (audioFile.Channels == 2)
                    {
                        selectionObject.ChartChannel[chartIndex] = 2;
                        selectionObject.ChartPair[chartIndex] = chartIndex + 1;
                        if (!newChart || chartIndex == 0)
                        {
                            fillChart(chartIndex, "Sound Wave");
                            if (chart.Series.IndexOf("Sound Wave R") == -1)
                            {
                                addChart("Sound Wave R", SeriesChartType.Column);
                            }
                        }
                        else
                        {
                            fillChart(chartIndex, "Sound Wave 2");
                            if (chart.Series.IndexOf("Sound Wave R2") == -1)
                            {
                                addChart("Sound Wave R2", SeriesChartType.Column);
                            }
                        }
                        data = new double[audioFile.R1.Length];
                        data = ArrayTransform.convertToDouble(audioFile.R1);
                        BlockSize = data.Length/* / 10*/;
                        viewSize = BlockSize + 1;
                        selectionObject.BlockSize[chartIndex + 1] = BlockSize;
                        selectionObject.ViewSize[chartIndex + 1] = viewSize;
                        selectionObject.Datas[chartIndex + 1] = data;
                        selectionObject.ArchiveManager.Datas[chartIndex + 1] = data;
                        chartAreaThemeColor(chart.ChartAreas[chartIndex + 1],
                            ThemeColorR, SelectedColorR, BrushR, LineBrushR, chartIndex + 1);
                        selectionObject.pixelCompress(chartIndex + 1, data);
                        selectionObject.CurrentScrollPos[chartIndex + 1] = selectionObject.CurrentCompressLevel[chartIndex + 1];
                        selectionObject.ChartChannel[chartIndex + 1] = 2;
                        selectionObject.ChartPair[chartIndex + 1] = chartIndex;
                        if (!newChart || chartIndex == 0)
                        {
                            fillChart(chartIndex + 1, "Sound Wave R");
                        }
                        else
                        {
                            fillChart(chartIndex + 1, "Sound Wave R2");
                        }
                    }
                    am.AudioFiles[chartIndex] = audioFile;
                    fillHeaderInfo(0,
                        (uint)(audioFile.SampleRate / (audioFile.BitDepth / 8)),
                        (uint)(audioFile.FmtBlockAlign), (uint)(audioFile.Channels),
                        (uint)(audioFile.SampleRate), (uint)(audioFile.BitDepth),
                        (uint)(audioFile.FmtCode));
                    originalRecord = audioFile.ByteArray;
                    Modified = true;
                    isRecordedData = false;
                }
            }
        }

        /// <summary>
        /// Fills up the headerInfo
        /// </summary>
        /// <param name="cbSize">chunk size</param>
        /// <param name="nAvgbps">average byte per seconds</param>
        /// <param name="nBlockA">number of block align</param>
        /// <param name="nChannels">number of channels</param>
        /// <param name="nSampleps">number of samples</param>
        /// <param name="wbitps">bite per sample</param>
        /// <param name="wFomrat">format tag</param>
        private void fillHeaderInfo(uint cbSize, uint nAvgbps, uint nBlockA,
            uint nChannels, uint nSampleps, uint wbitps, uint wFomrat)
        {
            audioHeaderInfo.cbSize = cbSize;
            audioHeaderInfo.nAvgBytesPerSec = nAvgbps;
            audioHeaderInfo.nBlockAlign = nBlockA;
            audioHeaderInfo.nChannels = nChannels;
            audioHeaderInfo.nSamplesPerSec = nSampleps;
            audioHeaderInfo.wBitsPerSample = wbitps;
            audioHeaderInfo.wFormatTag = wFomrat;
        }

        /// <summary>
        /// Saves the audio file into host device
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AudioFile audioFile = (AudioFile)am.AudioFiles[chartIndex];
            if (FlagSetting.runLengthEncoding)
            {
                // testing compression
                byte[] compressed = FileCompression.ModifiedRunLengthCompression(audioFile.ByteArray);
                // testing decompression
                byte[] uncompressed = FileCompression.ModifiedRunLengthDecompress(compressed, audioFile.ByteArray);

                for (int i = 0; i < audioFile.ByteArray.Length; i++)
                {
                    int count = 0;
                    if (uncompressed[i] == audioFile.ByteArray[i])
                    {
                        count++;
                    }
                    if (count == audioFile.ByteArray.Length)
                    {
                        count = 0;
                    }
                    else
                    {
                        count = -1;
                    }
                }
            }

            if (originalRecord != null)
            {
                if (Modified)
                {
                    if (isRecordedData)
                    {
                        originalRecord = ArrayTransform.doubleRecordToByte(
                            selectionObject.Datas[selectionObject.ChartIndex]);
                        Recorder.SetHeaderInfo(audioHeaderInfo);
                        updateAudio(originalRecord);
                        prepareSaveAudioFile(originalRecord);
                    }
                    else
                    {
                        updateAudioFileChannelDataAndSize();
                    }
                    Modified = false;
                }
            }

            saveFileDialog.Filter = "*.wav|* .wav";
            DialogResult dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                Stream f;
                if ((f = saveFileDialog.OpenFile()) != null)
                {
                    WavIO.writeWav(audioFile, f);
                }
            }
        }

        /// <summary>
        /// Calls cut method in Selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionObject.cut();
        }

        /// <summary>
        /// Call copy method in Selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionObject.copy();
        }

        /// <summary>
        /// Call delete method in Selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionObject.delete();
        }

        /// <summary>
        /// Call paste method in Selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionObject.paste();
        }

        /// <summary>
        /// Call selectAll method in Selection
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectionObject.selectAll();
        }

        // redraw rectangle to be implemented
        /// <summary>
        /// Redraw form when resized
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void SoundEditorOptimize_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                Control control = (Control)sender;
                int w = control.Size.Width;
                int h = control.Size.Height;
                //            chart.Size = new System.Drawing.Size(w, h -25);
                chartPixelWidth = w;
                closeForm.Location = new Point(w - closeFormHorzOffset, 0);
                panel1.Size = new Size(w, h - panel1VertOffest);
                chart.Size = new Size(w, h - chartVertOffset - 20);
                maxForm.Location = new Point(w - maxformHorzOffset, 0);
                minForm.Location = new Point(w - minformHorzOffset, 0);
                if (runRecorder)
                {
                    recorderPanel.Size = new Size(w, 60);
                    //record.Location = new Point(50, 10);
                    //record.Size = new Size(30, 30);
                }
                if (runThreading)
                {
                    threadingPanel.Size = new Size(w, 60 + recorderOffset);
                }
                //fillChart(chartIndex, "Sound Wave");
                if (selectionObject.XMax[selectionObject.ChartIndex] > 0 && selectionObject.XMin[selectionObject.ChartIndex] > 0)
                {
                    selectionObject.refreshCharts();
                    selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
                }
            }
        }

        /// <summary>
        /// Changes theme color of a chartArea
        /// </summary>
        /// <param name="area">chartArea</param>
        /// <param name="chartTheme">theme color</param>
        /// <param name="selectTheme">selected color</param>
        /// <param name="brush">brush with specified color</param>
        /// <param name="lineBrush">lineBrush with specified color</param>
        /// <param name="target">index of target chart</param>
        private void chartAreaThemeColor(ChartArea area, Color chartTheme,
                Color selectTheme, SolidBrush brush, SolidBrush lineBrush, int target)
        {
            chart.Series[target].Color = chartTheme;
            chart.Legends[target].ForeColor = chartTheme;
            area.AxisX.ScrollBar.ButtonColor = ThemeBackgroundColor;
            area.AxisX.ScrollBar.LineColor = ThemeBackgroundColor;
            area.BackColor = ThemeBackgroundColor;
            area.AxisY.LineColor = ThemeBackgroundColor;
            area.AxisX.LineColor = ThemeBackgroundColor;
            area.AxisX.MajorGrid.LineColor = ThemeBackgroundColor;
            area.AxisY.MajorGrid.LineColor = ThemeBackgroundColor;
            area.AxisY.LabelStyle.ForeColor = chartTheme;
            area.AxisX.LabelStyle.ForeColor = chartTheme;
            area.AxisX.ScrollBar.BackColor = chartTheme;
            area.AxisY.TitleForeColor = chartTheme;
            area.AxisX.TitleForeColor = chartTheme;
            chart.Legends[index].BackColor = ThemeBackgroundColor;
            selectionObject.ThemeColors[target] = chartTheme;
            selectionObject.SelectedColors[target] = selectTheme;
            selectionObject.Brushes[target] = brush;
            selectionObject.LineBrushes[target] = lineBrush;
            //            chart.Refresh();
        }

        /// <summary>
        /// Handle discrete fourier transform on current chartArea
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void discreteFourierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (chart.Series.IndexOf("Fourier Transform") == -1)
            {
                addChart("Fourier Transform", SeriesChartType.FastLine);
            }
            int index = chart.Series.IndexOf("Fourier Transform");
            chartAreaThemeColor(chart.ChartAreas[index],
                ThemeColor2, SelectedColor2, brush2, lineBrush2, index);
            int cIndex = selectionObject.ChartIndex;
            int xMax = (selectionObject.XMax[cIndex] + 1) * selectionObject.CurrentCompressLevel[cIndex];
            int xMin = selectionObject.XMin[cIndex] * selectionObject.CurrentCompressLevel[cIndex];
            selectionObject.FourierBelong = cIndex;
            if (xMax >= selectionObject.Data.Length)
            {
                xMax = selectionObject.Data.Length - 1;
            }
            if (xMin != -1 && xMax != -1)
            {
                int N = xMax - xMin + 1;
                double[] dataDouble = new double[N];
                for (int i = 0; i < N; i++)
                {
                    dataDouble[i] = selectionObject.Data[xMin + i];
                }
                double[] weightedPoints = applyWeights(dataDouble, xMax, xMin, N);
                double[] transformed = FourierTransform.getAmplitudeFromAf(
                    FourierTransform.fourier(weightedPoints, N));
                selectionObject.pixelCompress(index, transformed);
                fillChartTransform(index, "Fourier Transform", transformed, 1, selectionObject.ChartIndex);
            }
        }

        /// <summary>
        /// Handle inverse discrete fourier transform on current chartArea
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inverseDiscreteFourierToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private double[] applyWeights(double[] dataDouble, int xMax, int xMin, int N)
        {
            double[] weights = null;
            if (rectangleWindow)
            {
                weights = getRectangularWindow(xMax, xMin, N);
            }
            else if (triangleWindow)
            {
                weights = getTriangularWindow(xMax, xMin, N);
            }
            else if (polynomialWindow)
            {
                weights = getPolynomialWindow(xMax, xMin, N);
            }
            double[] weightedPoints = new double[N];
            for (int i = 0; i < N; i++)
            {
                weightedPoints[i] = dataDouble[i] * weights[i];
            }
            return weightedPoints;
        }

        /// <summary>
        /// Returns rectangular window weights baesd on selected ranges
        /// </summary>
        /// <param name="xMax">selected max x position</param>
        /// <param name="xMin">selected min x position</param>
        /// <param name="N">selected number of data</param>
        /// <returns>Rectangular window weights</returns>
        private double[] getRectangularWindow(int xMax, int xMin, int N)
        {
            double[] weights = new double[N];
            for (int i = 0; i < N; i++)
            {
                weights[i] = 1;
            }
            return weights;
        }

        /// <summary>
        /// Returns triangular window weights baesd on selected ranges
        /// </summary>
        /// <param name="xMax">selected max x position</param>
        /// <param name="xMin">selected min x position</param>
        /// <param name="N">selected number of data</param>
        /// <returns>Triangular window weights</returns>
        private double[] getTriangularWindow(int xMax, int xMin, int N)
        {
            double[] weights = new double[N];
            for (int i = 0; i < N; i++)
            {
                weights[i] = 1 - Math.Abs(((double)i - ((double)N - 1) / 2) / (((double)N - 1) / 2));
            }
            return weights;
        }

        private double[] getPolynomialWindow(int xMax, int xMin, int N)
        {
            double[] weights = new double[N];
            for (int i = 0; i < N; i++)
            {
                weights[i] = 1 - Math.Pow(((double)i - ((double)N - 1) / 2) / (((double)N - 1) / 2), 2);
            }
            return weights;
        }

        /// <summary>
        /// Handle triangle filter on current chartArea and plot on a new chartArea if not yet existed
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rectangleWindow = false;
            rectangleToolStripMenuItem.CheckState = CheckState.Unchecked;
            triangleWindow = true;
            triangleToolStripMenuItem.CheckState = CheckState.Checked;
            polynomialWindow = false;
            polynomialToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        /// <summary>
        /// Rectangle window toggle button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rectangleWindow = true;
            rectangleToolStripMenuItem.CheckState = CheckState.Checked;
            triangleWindow = false;
            triangleToolStripMenuItem.CheckState = CheckState.Unchecked;
            polynomialWindow = false;
            polynomialToolStripMenuItem.CheckState = CheckState.Unchecked;
        }

        /// <summary>
        /// Polynomial window toggle button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void polynomialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rectangleWindow = false;
            rectangleToolStripMenuItem.CheckState = CheckState.Unchecked;
            triangleWindow = false;
            triangleToolStripMenuItem.CheckState = CheckState.Unchecked;
            polynomialWindow = true;
            polynomialToolStripMenuItem.CheckState = CheckState.Checked;
        }

        /// <summary>
        /// Handle filter on current chartArea and plot on a new chartArea if not yet existed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectionObject.ChartType[selectionObject.ChartIndex] == 1)
            {
                int cIndex = selectionObject.ChartIndex;
                int xMax = (selectionObject.XMax[cIndex] + 1) * selectionObject.CurrentCompressLevel[cIndex];
                int xMin = selectionObject.XMin[cIndex] * selectionObject.CurrentCompressLevel[cIndex];
                if (xMin != -1 && xMax != -1)
                {
                    int N = xMax - xMin + 1;
                    //string seriesName = "Sound Wave";
                    //int belong = selectionObject.ChartBelong[selectionObject.ChartIndex];
                    int index = selectionObject.FourierBelong;
                    double[] data = selectionObject.Datas[index];
                    int[] filter = Filter.getFilter(selectionObject.Datas[cIndex].Length, 0, N);
                    double[] filteredData = Filter.filter(data, filter);
                    //int index = chart.Series.IndexOf(seriesName);
                    selectionObject.pixelCompress(index, filteredData);
                    fillChartTransform(index, chart.Series[index].Name, filteredData, 0, 1);
                }
            }
        }

        /// <summary>
        /// Refreshes panel when clicked to ensure resize properly redraws selected data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Click(object sender, EventArgs e)
        {
            selectionObject.refreshCharts();
            selectionObject.updateSelectedRectFill(selectionObject.ChartIndex);
        }

        /// <summary>
        /// Color renderer to override specific toolStrip color
        /// </summary>
        private class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer(Color themeBackgroundColor) : base(new MyColors(themeBackgroundColor)) { }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                var toolStripMenuItem = e.Item as ToolStripMenuItem;
                if (toolStripMenuItem != null)
                {
                    e.ArrowColor = Color.FromArgb(200, 144, 238, 144);
                }
                base.OnRenderArrow(e);
            }
        }

        /// <summary>
        /// Class that override specific form item color
        /// </summary>
        private class MyColors : ProfessionalColorTable
        {
            private Color themeBackgroundColor;

            public MyColors(Color themeBackgroundColor)
            {
                this.themeBackgroundColor = themeBackgroundColor;
            }

            public override Color MenuItemSelected
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonSelectedGradientMiddle
            {
                get { return Color.Transparent; }
            }

            public override Color ButtonSelectedHighlight
            {
                get { return Color.Transparent; }
            }

            public override Color ButtonCheckedGradientBegin
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonSelectedBorder
            {
                get { return Color.FromArgb(200, 144, 238, 144); }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color CheckSelectedBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemBorder
            {
                get { return Color.Black; }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.Transparent; }
            }
            public override Color CheckBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color CheckPressedBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return Color.Transparent; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return Color.Transparent; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return Color.Transparent; }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.Transparent; }
            }
        }

        /// <summary>
        /// Custom button that override Button
        /// </summary>
        public class CustomButton : Button
        {
            protected override bool ShowFocusCues
            {
                get
                {
                    return false;
                }
            }

        }

        /// <summary>
        /// Customize menuStrip's color
        /// </summary>
        /// <param name="menuStrip">Menustrip object</param>
        private void customizeMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.Renderer = new MyRenderer(ThemeBackgroundColor);
            menuStrip.BackColor = Color.Transparent;
            menuStrip.ForeColor = ThemeColor;
            newToolStripMenuItem.ForeColor = ThemeColor;
            openToolStripMenuItem.ForeColor = ThemeColor;
            saveToolStripMenuItem.ForeColor = ThemeColor;
            toolStripMenuItem1.BackColor = ThemeBackgroundColor;
            exitToolStripMenuItem.ForeColor = ThemeColor;
            undoToolStripMenuItem.ForeColor = ThemeColor;
            redoToolStripMenuItem.ForeColor = ThemeColor;
            toolStripMenuItem2.BackColor = ThemeBackgroundColor;
            cutToolStripMenuItem.ForeColor = ThemeColor;
            copyToolStripMenuItem.ForeColor = ThemeColor;
            pasteToolStripMenuItem.ForeColor = ThemeColor;
            deleteToolStripMenuItem.ForeColor = ThemeColor;
            toolStripMenuItem3.BackColor = ThemeBackgroundColor;
            selectAllToolStripMenuItem.ForeColor = ThemeColor;
            discreteFourierToolStripMenuItem.ForeColor = ThemeColor;
            inverseDiscreteFourierToolStripMenuItem.ForeColor = ThemeColor;
            windowingToolStripMenuItem.ForeColor = ThemeColor;
            rectangleToolStripMenuItem.ForeColor = ThemeColor;
            triangleToolStripMenuItem.ForeColor = ThemeColor;
            polynomialToolStripMenuItem.ForeColor = ThemeColor;
            filterToolStripMenuItem.ForeColor = ThemeColor;
            recorderToolStripMenuItem.ForeColor = ThemeColor;
            threadingToolStripMenuItem.ForeColor = ThemeColor;
            compressionToolStripMenuItem.ForeColor = ThemeColor;
            runLengthEncodingToolStripMenuItem.ForeColor = ThemeColor;
            chartToolStripMenuItem.ForeColor = ThemeColor;
        }

        /// <summary>
        /// Getter and setter for themeColor
        /// </summary>
        public Color ThemeColor { get => themeColor; set => themeColor = value; }

        /// <summary>
        /// Getter and setter for themeColor2
        /// </summary>
        public Color ThemeColor2 { get => themeColor2; set => themeColor2 = value; }

        /// <summary>
        /// Getter and setter for themeColor3
        /// </summary>
        public Color ThemeColor3 { get => themeColor3; set => themeColor3 = value; }

        /// <summary>
        /// Getter and setter for themeColor4
        /// </summary>
        public Color ThemeColor4 { get => themeColor4; set => themeColor4 = value; }

        /// <summary>
        /// Getter and setter for selectedColor
        /// </summary>
        public Color SelectedColor { get => selectedColor; set => selectedColor = value; }

        /// <summary>
        /// Getter and setter for selectedColor2
        /// </summary>
        public Color SelectedColor2 { get => selectedColor2; set => selectedColor2 = value; }

        /// <summary>
        /// Getter and setter for selectedColor3
        /// </summary>
        public Color SelectedColor3 { get => selectedColor3; set => selectedColor3 = value; }

        /// <summary>
        /// Getter and setter for selectedColor4
        /// </summary>
        public Color SelectedColor4 { get => selectedColor4; set => selectedColor4 = value; }

        /// <summary>
        /// Getter and setter for themeBackgroundColor
        /// </summary>
        public Color ThemeBackgroundColor { get => themeBackgroundColor; set => themeBackgroundColor = value; }

        /// <summary>
        /// Getter and setter for blockSize
        /// </summary>
        public int BlockSize { get => blockSize; set => blockSize = value; }
        public AudioHeaderInfo AudioHeaderInfo { get => audioHeaderInfo; set => audioHeaderInfo = value; }
        public bool Modified { get => modified; set => modified = value; }
        public Color SelectedColorR { get => selectedColorR; set => selectedColorR = value; }
        public SolidBrush BrushR { get => brushR; set => brushR = value; }
        public SolidBrush LineBrushR { get => lineBrushR; set => lineBrushR = value; }
        public Color ThemeColorR { get => themeColorR; set => themeColorR = value; }

        /// <summary>
        /// Call runRecorder in Recorder and call createRecorder if success
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void recorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!runRecorder)
            {
                if (Recorder.runRecorder())
                {
                    recorderOffset = 50;
                    chartVertOffset += 75;
                    chart.Location = new System.Drawing.Point(0, chart.Location.Y + 50);
                    chart.Size = new Size(chart.Size.Width, chart.Size.Height - 50);
                    createRecorder();
                    updatePanelPosition();
                }
                runRecorder = true;
            }
        }

        /// <summary>
        /// Call beginRecord in Recorder and disable record until finish
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void record_Click(object sender, EventArgs e)
        {
            record.Enabled = false;
            Recorder.beginRecord();
            record.Enabled = true;
        }

        /// <summary>
        /// Get data from dll and copy the data over
        /// </summary>
        private void getDataFromDll()
        {
            dllDataSize = Recorder.getDwDataLength();
            dllData = Recorder.getData();
            originalRecord = new byte[dllDataSize];
            Marshal.Copy(dllData, originalRecord, 0, (int)dllDataSize);
        }

        /// <summary>
        /// Update modified volume data into modifiedRecord
        /// </summary>
        private void updateAudioVolumn()
        {
            modifiedRecord = new byte[originalRecord.Length];
            for (uint i = 0; i < originalRecord.Length; i++)
            {
                int adjustedRecord = (int)((float)(originalRecord[i] - Recorder.HALF_BYTE_SIZE)
                    * (float)(setVolume) / (float)Recorder.BASE_MULTIPLIER);
                byte adjustedByte;
                if (adjustedRecord > Recorder.HALF_BYTE_SIZE - 1)
                {
                    adjustedByte = Recorder.BYTE_SIZE - 1;
                }
                else if (adjustedRecord < Recorder.HALF_BYTE_SIZE * -1)
                {
                    adjustedByte = 0;
                }
                else
                {
                    adjustedByte = (byte)(adjustedRecord + Recorder.HALF_BYTE_SIZE);
                }
                modifiedRecord[i] = adjustedByte;
            }
            curVolume = setVolume;
        }

        /// <summary>
        /// Copy updatedRecord into dllData to be played
        /// </summary>
        /// <param name="updatedRecord">updated data byte array</param>
        private void updateAudio(byte[] updatedRecord)
        {
            dllDataSize = (uint)updatedRecord.Length;
            Recorder.SetDataSize(dllDataSize);
            dllData = Recorder.getData();
            Marshal.Copy(updatedRecord, 0, dllData, (int)dllDataSize);
        }

        /// <summary>
        /// Prepare audioFile's data size and byte array to be ready for save
        /// </summary>
        private void prepareSaveAudioFile(byte[] updatedArray)
        {
            am.AudioFiles[chartIndex].FileSize = 36 + updatedArray.Length;
            am.AudioFiles[chartIndex].ByteArray = updatedArray;
            am.AudioFiles[chartIndex].Samps = updatedArray.Length / am.AudioFiles[chartIndex].Channels;
            float[] asFloat = null;
            switch (am.AudioFiles[chartIndex].BitDepth)
            {
                case 64:
                    double[] asDouble = new double[am.AudioFiles[chartIndex].Samps];
                    Buffer.BlockCopy(am.AudioFiles[chartIndex].ByteArray, 0, asDouble, 0, am.AudioFiles[chartIndex].Bytes);
                    asFloat = Array.ConvertAll(asDouble, e => (float)e);
                    break;
                case 32:
                    asFloat = new float[am.AudioFiles[chartIndex].Samps];
                    Buffer.BlockCopy(am.AudioFiles[chartIndex].ByteArray, 0, asFloat, 0, am.AudioFiles[chartIndex].Bytes);
                    break;
                case 16:
                    Int16[] asInt16 = new Int16[am.AudioFiles[chartIndex].Samps];
                    Buffer.BlockCopy(am.AudioFiles[chartIndex].ByteArray, 0, asInt16, 0, am.AudioFiles[chartIndex].Bytes);
                    asFloat = Array.ConvertAll(asInt16, e => (float)e /*/ (float)Int16.MaxValue*/);
                    break;
                case 8:
                    byte[] asByte = new byte[am.AudioFiles[chartIndex].Samps];
                    Buffer.BlockCopy(am.AudioFiles[chartIndex].ByteArray, 0, asByte, 0, am.AudioFiles[chartIndex].Bytes);
                    asFloat = Array.ConvertAll(asByte, e => (float)e/* / (float)Int16.MaxValue*/);
                    break;
                default:
                    break;
            }
            switch (am.AudioFiles[chartIndex].Channels)
            {
                case 1:
                    am.AudioFiles[chartIndex].L1 = asFloat;
                    am.AudioFiles[chartIndex].R1 = null;
                    break;
                case 2:
                    am.AudioFiles[chartIndex].L1 = new float[am.AudioFiles[chartIndex].Samps / 2];
                    am.AudioFiles[chartIndex].R1 = new float[am.AudioFiles[chartIndex].Samps / 2];
                    for (int i = 0, s = 0; i < am.AudioFiles[chartIndex].Samps / 2; i++)
                    {
                        am.AudioFiles[chartIndex].L1[i] = asFloat[s++];
                        am.AudioFiles[chartIndex].R1[i] = asFloat[s++];
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Call endRecord in Recorder to end recording
        /// Draw the recorded data afterward
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void endRecord_Click(object sender, EventArgs e)
        {
            if (Recorder.endRecord())
            {
                isRecordedData = true;
                getDataFromDll();
                selectionObject.Datas[chartIndex] = ArrayTransform.byteRecordToDouble(originalRecord);
                BlockSize = originalRecord.Length - 1;
                viewSize = originalRecord.Length;
                selectionObject.ViewSize[chartIndex] = viewSize;
                selectionObject.BlockSize[chartIndex] = BlockSize;
                selectionObject.pixelCompress(index, selectionObject.Datas[chartIndex]);
                fillChart(chartIndex, "Recorded");
                audioHeaderInfo = Recorder.GetHeaderInfo();
                AudioFile recordedFile = new AudioFile(audioHeaderInfo);
                recordedFile.ByteArray = originalRecord;
                recordedFile.ChunkID = 1179011410;
                recordedFile.DataID = 1635017060;
                recordedFile.FileName = "";
                recordedFile.FilePath = "";
                recordedFile.FmtExtraSize = 0;
                recordedFile.L1 = ArrayTransform.byteRecordToFloat(originalRecord);
                recordedFile.R1 = null;
                recordedFile.RiffType = 1163280727;
                recordedFile.SampleRate = 11025;
                recordedFile.Samps = (int)dllDataSize;
                recordedFile.FmtID = 544501094;
                recordedFile.FmtSize = 16;
                recordedFile.Bytes = (int)dllDataSize;
                recordedFile.ByteRate = 11025;
                recordedFile.FileSize = 36 + (int)dllDataSize;
                am.AudioFiles[chartIndex] = recordedFile;
            }
        }

        /// <summary>
        /// If record data exist, call playRecord in a separate thread
        /// Then call livePlayBegin for live visual
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void playBegin_Click(object sender, EventArgs e)
        {
            if (originalRecord != null)
            {
                if (Modified)
                {
                    if (isRecordedData)
                    {
                        originalRecord = ArrayTransform.doubleRecordToByte(
                            selectionObject.Datas[selectionObject.ChartIndex]);
                    }
                    else
                    {
                        updateAudioFileChannelDataAndSize();
                        originalRecord = getUpdatedByteArray();
                    }
                    Recorder.SetHeaderInfo(audioHeaderInfo);
                    updateAudio(originalRecord);
                    prepareSaveAudioFile(originalRecord);
                }
                if (setVolume != 50 && setVolume != curVolume)
                {
                    updateAudioVolumn();
                    updateAudio(modifiedRecord);
                }
                else if (setVolume == 50)
                {
                    curVolume = 50;
                }
                if (curVolume == 50)
                {
                    modifiedRecord = originalRecord;
                }
                Thread thread = new Thread(playRecordThread);
                thread.Start();
                livePlayBegin(sender, e);
                thread.Join();
            }
        }

        /// <summary>
        /// Translate the float array into byte array according to audio format
        /// </summary>
        /// <returns>byte array little endian format</returns>
        private byte[] getUpdatedByteArray()
        {
            byte[] byteArray = new byte[am.AudioFiles[chartIndex].Samps * am.AudioFiles[chartIndex].Channels];

            if (am.AudioFiles[chartIndex].Channels == 2)
            {
                int index = 0;
                switch (am.AudioFiles[chartIndex].BitDepth)
                {
                    case 64:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes((double)am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                            temp = BitConverter.GetBytes((double)am.AudioFiles[chartIndex].R1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 32:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes(am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                            temp = BitConverter.GetBytes(am.AudioFiles[chartIndex].R1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 16:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes((Int16)am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                            temp = BitConverter.GetBytes((Int16)am.AudioFiles[chartIndex].R1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 8:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byteArray[index++] = (byte)(am.AudioFiles[chartIndex].L1[i]);
                            byteArray[index++] = (byte)(am.AudioFiles[chartIndex].R1[i]);
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (am.AudioFiles[chartIndex].BitDepth)
                {
                    case 64:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes((double)am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 32:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes(am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 16:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byte[] temp = BitConverter.GetBytes((Int16)am.AudioFiles[chartIndex].L1[i]);
                            for (int j = 0; j < temp.Length; j++)
                            {
                                byteArray[index++] = temp[j];
                            }
                        }
                        break;
                    case 8:
                        for (int i = 0; i < am.AudioFiles[chartIndex].L1.Length; i++)
                        {
                            byteArray[index++] = (byte)am.AudioFiles[chartIndex].L1[i];
                        }
                        break;
                    default:
                        break;
                }
            }
            return byteArray;
        }

        /// <summary>
        /// Updates changes to the audiofile left and right channel and file size
        /// </summary>
        private void updateAudioFileChannelDataAndSize()
        {
            if (am.AudioFiles[chartIndex].Channels == 2)
            {
                int leftIndex = selectionObject.ChartPair[selectionObject.ChartIndex];
                int rightIndex = selectionObject.ChartPair[leftIndex];
                if (leftIndex > rightIndex)
                {
                    int temp = leftIndex;
                    leftIndex = rightIndex;
                    rightIndex = temp;
                }
                am.AudioFiles[chartIndex].L1 = ArrayTransform.convertToFloat(selectionObject.Datas[leftIndex]);
                am.AudioFiles[chartIndex].R1 = ArrayTransform.convertToFloat(selectionObject.Datas[rightIndex]);
                am.AudioFiles[chartIndex].FileSize = 36 + am.AudioFiles[chartIndex].L1.Length * 2;
                am.AudioFiles[chartIndex].Samps = am.AudioFiles[chartIndex].L1.Length * 2;
                am.AudioFiles[chartIndex].Bytes = am.AudioFiles[chartIndex].Samps * 2;
            }
            else
            {
                am.AudioFiles[chartIndex].L1 = ArrayTransform.convertToFloat(selectionObject.Datas[selectionObject.ChartIndex]);
                am.AudioFiles[chartIndex].FileSize = 36 + am.AudioFiles[chartIndex].L1.Length;
                am.AudioFiles[chartIndex].Samps = am.AudioFiles[chartIndex].L1.Length;
                am.AudioFiles[chartIndex].Bytes = am.AudioFiles[chartIndex].Samps;
            }
        }

        /// <summary>
        /// Setup for live playback visuals at interval of 125 msec for best sync with windows form timer
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void livePlayBegin(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            toBeProcessedPos = 0;
            processedPos = 0;
            int interval = 20;

            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = interval;
            timer.Start();
        }

        /// <summary>
        /// Timer tick event for live play
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            endTime = DateTime.Now;
            TimeSpan difference = endTime - startTime;
            double milliseconds = difference.TotalMilliseconds;
            dataPortionUpdate();
            if (milliseconds / 1000 > dllDataSize / (double)audioHeaderInfo.nSamplesPerSec)
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Update of data for live play
        /// </summary>
        private void dataPortionUpdate()
        {
            TimeSpan difference = endTime - startTime;
            double milliseconds = difference.TotalMilliseconds;
            double percentPlayed = (milliseconds / 1000.0) / 
                (dllDataSize / (double)audioHeaderInfo.nSamplesPerSec /
                (audioHeaderInfo.wBitsPerSample / 8) / audioHeaderInfo.nChannels);
            if (percentPlayed > 1)
            {
                percentPlayed = 1;
            }
            int seriesSize = chart.Series[selectionObject.ChartIndex].Points.Count;

            toBeProcessedPos = (int)(seriesSize * percentPlayed);
            for (; processedPos < seriesSize && processedPos < toBeProcessedPos; processedPos++)
            {
                chart.Series[selectionObject.ChartIndex].Points[processedPos].Color = selectedColor;
                if (processedPos != seriesSize - 1)
                {
                    double yMax = chart.Series[selectionObject.ChartIndex].Points[processedPos].YValues[0];
                    double yMax2 = chart.Series[selectionObject.ChartIndex].Points[processedPos].YValues[1];

                    yMax *= 1.5;
                    yMax2 *= 1.5;

                    chart.Series[selectionObject.ChartIndex].Points[processedPos].YValues[0] = yMax;
                    chart.Series[selectionObject.ChartIndex].Points[processedPos].YValues[1] = yMax2;
                }
                if (processedPos != 0)
                {
                    double yMaxPrev = chart.Series[selectionObject.ChartIndex].Points[processedPos - 1].YValues[0];
                    double yMaxPrev2 = chart.Series[selectionObject.ChartIndex].Points[processedPos - 1].YValues[1];

                    yMaxPrev /= 1.5;
                    yMaxPrev2 /= 1.5;

                    chart.Series[selectionObject.ChartIndex].Points[processedPos - 1].YValues[0] = yMaxPrev;
                    chart.Series[selectionObject.ChartIndex].Points[processedPos - 1].YValues[1] = yMaxPrev2;
                }
            }
            chart.Refresh();
        }

        /// <summary>
        /// Thread method for handling playRecord in Recorder
        /// </summary>
        private void playRecordThread()
        {
            Recorder.playRecord();
        }

        /// <summary>
        /// Calls playEnd in Recorder when playEnd button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playEnd_Click(object sender, EventArgs e)
        {
            Recorder.playEnd();
        }

        /// <summary>
        /// Calls playPause in Recorder when playPause button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playPause_Click(object sender, EventArgs e)
        {
            Recorder.playPause();
        }

        /// <summary>
        /// Calls playSpeed in Recorder when playSpeed button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void playSpeed_Click(object sender, EventArgs e)
        {
            Recorder.playSpeed();
        }

        /// <summary>
        /// Calls playReverse in Recorder when playReverse button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void playReverse_Click(object sender, EventArgs e)
        {
            Recorder.playReverse();
        }

        /// <summary>
        /// Calls playRepeat in Recorder when playRepeat button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void playRepeat_Click(object sender, EventArgs e)
        {
            Recorder.playRepeat();
        }

        /// <summary>
        /// Create the recorder panel with all recorder buttons
        /// </summary>
        private void createRecorder()
        {
            recorderPanel = new Panel();
            // 
            // panel2
            // 
            recorderPanel.BackColor = themeBackgroundColor;
            panel1.Controls.Add(recorderPanel);
            //panel2.Controls.Add(this.menuStrip1);
            recorderPanel.Location = new Point(0, 25);
            recorderPanel.Name = "recorderPanel";
            recorderPanel.Size = new Size(panel1.Width, 50);
            recorderPanel.TabIndex = 8;
            //
            // record
            //
            record = new CustomButton();
            record.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            record.FlatAppearance.BorderSize = 0;
            record.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            record.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            record.Location = new System.Drawing.Point(50, 10);
            record.Name = "record";
            record.Size = new System.Drawing.Size(30, 30);
            record.TabIndex = 9;
            record.TabStop = false;
            record.Text = "\u23FA";
            record.ForeColor = themeColor2;
            record.UseMnemonic = false;
            record.UseVisualStyleBackColor = true;
            record.BackColor = ThemeBackgroundColor;
            record.Click += new System.EventHandler(this.record_Click);
            recorderPanel.Controls.Add(record);
            //
            // endRecord
            //
            endRecord = new CustomButton();
            endRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            endRecord.FlatAppearance.BorderSize = 0;
            endRecord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            endRecord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            endRecord.Location = new System.Drawing.Point(80, 10);
            endRecord.Name = "endRecord";
            endRecord.Size = new System.Drawing.Size(30, 30);
            endRecord.TabIndex = 10;
            endRecord.TabStop = false;
            endRecord.Text = "\u220E";
            endRecord.ForeColor = themeColor2;
            endRecord.UseMnemonic = false;
            endRecord.UseVisualStyleBackColor = true;
            endRecord.BackColor = ThemeBackgroundColor;
            endRecord.Click += new System.EventHandler(this.endRecord_Click);
            recorderPanel.Controls.Add(endRecord);
            //
            // playBegin
            //
            playBegin = new CustomButton();
            playBegin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playBegin.FlatAppearance.BorderSize = 0;
            playBegin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playBegin.Location = new System.Drawing.Point(110, 10);
            playBegin.Name = "playBegin";
            playBegin.Size = new System.Drawing.Size(30, 30);
            playBegin.TabIndex = 11;
            playBegin.TabStop = false;
            playBegin.Text = "\u25B6";
            playBegin.ForeColor = ThemeColor;
            playBegin.UseMnemonic = false;
            playBegin.UseVisualStyleBackColor = true;
            playBegin.BackColor = ThemeBackgroundColor;
            playBegin.Click += new System.EventHandler(this.playBegin_Click);
            recorderPanel.Controls.Add(playBegin);
            //
            // playend
            //
            playEnd = new CustomButton();
            playEnd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playEnd.FlatAppearance.BorderSize = 0;
            playEnd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playEnd.Location = new System.Drawing.Point(140, 10);
            playEnd.Name = "playEnd";
            playEnd.Size = new System.Drawing.Size(30, 30);
            playEnd.TabIndex = 12;
            playEnd.TabStop = false;
            playEnd.Text = "\u23F9";
            playEnd.ForeColor = ThemeColor;
            playEnd.UseMnemonic = false;
            playEnd.UseVisualStyleBackColor = true;
            playEnd.BackColor = ThemeBackgroundColor;
            playEnd.Click += new System.EventHandler(this.playEnd_Click);
            recorderPanel.Controls.Add(playEnd);
            //
            // playPause
            //
            playPause = new CustomButton();
            playPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playPause.FlatAppearance.BorderSize = 0;
            playPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playPause.Location = new System.Drawing.Point(170, 10);
            playPause.Name = "playPause";
            playPause.Size = new System.Drawing.Size(30, 30);
            playPause.TabIndex = 13;
            playPause.TabStop = false;
            playPause.Text = "❚❚";
            playPause.ForeColor = ThemeColor;
            playPause.UseMnemonic = false;
            playPause.UseVisualStyleBackColor = true;
            playPause.BackColor = ThemeBackgroundColor;
            playPause.Click += new System.EventHandler(this.playPause_Click);
            recorderPanel.Controls.Add(playPause);
            //
            // playSpeed
            //
            playSpeed = new CustomButton();
            playSpeed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playSpeed.FlatAppearance.BorderSize = 0;
            playSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playSpeed.Location = new System.Drawing.Point(200, 10);
            playSpeed.Name = "playSpeed";
            playSpeed.Size = new System.Drawing.Size(30, 30);
            playSpeed.TabIndex = 14;
            playSpeed.TabStop = false;
            playSpeed.Text = "⏩";
            playSpeed.ForeColor = ThemeColor;
            playSpeed.UseMnemonic = false;
            playSpeed.UseVisualStyleBackColor = true;
            playSpeed.BackColor = ThemeBackgroundColor;
            playSpeed.Click += new System.EventHandler(this.playSpeed_Click);
            recorderPanel.Controls.Add(playSpeed);
            //
            // playReverse
            //
            playReverse = new CustomButton();
            playReverse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playReverse.FlatAppearance.BorderSize = 0;
            playReverse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playReverse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playReverse.Location = new System.Drawing.Point(230, 10);
            playReverse.Name = "playReverse";
            playReverse.Size = new System.Drawing.Size(30, 30);
            playReverse.TabIndex = 15;
            playReverse.TabStop = false;
            playReverse.Text = "◀";
            playReverse.ForeColor = ThemeColor;
            playReverse.UseMnemonic = false;
            playReverse.UseVisualStyleBackColor = true;
            playReverse.BackColor = ThemeBackgroundColor;
            playReverse.Click += new System.EventHandler(this.playReverse_Click);
            recorderPanel.Controls.Add(playReverse);
            //
            // playRepeat
            //
            playRepeat = new CustomButton();
            playRepeat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            playRepeat.FlatAppearance.BorderSize = 0;
            playRepeat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            playRepeat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            playRepeat.Location = new System.Drawing.Point(260, 10);
            playRepeat.Name = "playRepeat";
            playRepeat.Size = new System.Drawing.Size(30, 30);
            playRepeat.TabIndex = 16;
            playRepeat.TabStop = false;
            playRepeat.Text = "🔁";
            playRepeat.ForeColor = ThemeColor;
            playRepeat.UseMnemonic = false;
            playRepeat.UseVisualStyleBackColor = true;
            playRepeat.BackColor = ThemeBackgroundColor;
            playRepeat.Click += new System.EventHandler(this.playRepeat_Click);
            recorderPanel.Controls.Add(playRepeat);
            Panel customSliderPanel = new Panel();
            // 
            // customSliderPanel
            // 
            customSliderPanel.BackColor = themeBackgroundColor;
            recorderPanel.Controls.Add(customSliderPanel);
            //panel2.Controls.Add(this.menuStrip1);
            customSliderPanel.Location = new Point(290, 10);
            customSliderPanel.Name = "customSliderPanel";
            customSliderPanel.Size = new Size(105, 30);
            customSliderPanel.TabIndex = 8;
            //
            // CustomSlider 
            //
            //(int height, int radius, SolidBrush brush, SolidBrush brushTwo)
            customSlider = new CustomSlider(5, 100, 5, new SolidBrush(Color.FromArgb(50, 100, 100, 100)), new SolidBrush(ThemeColor));
            customSlider.Location = new System.Drawing.Point(0, 7);
            customSlider.Name = "Custom Slider";
            customSlider.Size = new System.Drawing.Size(100, 15);
            customSlider.BackColor = Color.Black;
            Image knobImage = Image.FromFile("..\\..\\img\\circleGrey.png");
            Image knobHoverImage = Image.FromFile("..\\..\\img\\circleLightGreen.png");
            customSlider.KnobImage = new System.Drawing.Bitmap(knobImage);
            customSlider.KnobHoverImage = new System.Drawing.Bitmap(knobHoverImage);
            customSlider.MouseUp += new MouseEventHandler(this.customSlider_MouseUp);
            customSlider.MouseMove += new MouseEventHandler(this.customSlider_MouseMove);
            customSliderPanel.Controls.Add(customSlider);
        }

        /// <summary>
        /// Handle customSlider mouse up event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void customSlider_MouseUp(object sender, EventArgs e)
        {
            setVolume = customSlider.Value;
        }

        /// <summary>
        /// Handle customSlider mouse move event
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void customSlider_MouseMove(object sender, EventArgs e)
        {
            setVolume = customSlider.Value;
        }

        /// <summary>
        /// Opens threading panel if not already opened
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void threadingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!runThreading)
            {
                chartVertOffset += 75;
                chart.Location = new System.Drawing.Point(0, chart.Location.Y + 50);
                chart.Size = new Size(chart.Size.Width, chart.Size.Height - 50);
                createThreadingPanel();
                updatePanelPosition();
            }
            runThreading = true;
        }

        /// <summary>
        /// Create threading panel with its buttons
        /// </summary>
        private void createThreadingPanel()
        {
            threadingPanel = new Panel();
            // 
            // panel2
            // 
            threadingPanel.BackColor = themeBackgroundColor;
            panel1.Controls.Add(threadingPanel);
            //panel2.Controls.Add(this.menuStrip1);
            threadingPanel.Location = new Point(0, 25 + recorderOffset);
            threadingPanel.Name = "threadingPanel";
            threadingPanel.Size = new Size(panel1.Width, 50);
            //
            // beginThreadTest
            //
            beginThreadTest = new CustomButton();
            beginThreadTest.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            beginThreadTest.FlatAppearance.BorderSize = 0;
            beginThreadTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            beginThreadTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            beginThreadTest.Location = new System.Drawing.Point(50, 10);
            beginThreadTest.Name = "beginThreadTest";
            beginThreadTest.Size = new System.Drawing.Size(30, 30);
            beginThreadTest.TabStop = false;
            beginThreadTest.Text = "\u23FA";
            beginThreadTest.ForeColor = themeColor;
            beginThreadTest.UseMnemonic = false;
            beginThreadTest.UseVisualStyleBackColor = true;
            beginThreadTest.BackColor = ThemeBackgroundColor;
            beginThreadTest.Click += new System.EventHandler(this.beginThreadTest_Click);
            threadingPanel.Controls.Add(beginThreadTest);
            //
            // inputThreadCount
            //
            currentThreadCount = new Label();
            currentThreadCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            currentThreadCount.Location = new System.Drawing.Point(80, 10);
            currentThreadCount.Size = new System.Drawing.Size(30, 30);
            currentThreadCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            currentThreadCount.TextAlign = ContentAlignment.MiddleCenter;
            currentThreadCount.Text = "4";
            currentThreadCount.ForeColor = themeColor;
            currentThreadCount.BackColor = Color.Black;
            threadingPanel.Controls.Add(currentThreadCount);
            //
            // addThreadCount
            //
            addThreadCount = new CustomButton();
            addThreadCount.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            addThreadCount.FlatAppearance.BorderSize = 0;
            addThreadCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            addThreadCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            addThreadCount.Location = new System.Drawing.Point(110, 10);
            addThreadCount.Name = "beginThreadTest";
            addThreadCount.Size = new System.Drawing.Size(30, 30);
            addThreadCount.TabStop = false;
            addThreadCount.Text = "➕";
            addThreadCount.ForeColor = themeColor;
            addThreadCount.UseMnemonic = false;
            addThreadCount.UseVisualStyleBackColor = true;
            addThreadCount.BackColor = ThemeBackgroundColor;
            addThreadCount.Click += new System.EventHandler(this.addThreadCount_Click);
            threadingPanel.Controls.Add(addThreadCount);
            //
            // subtractThreadCount
            //
            subtractThreadCount = new CustomButton();
            subtractThreadCount.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            subtractThreadCount.FlatAppearance.BorderSize = 0;
            subtractThreadCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            subtractThreadCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            subtractThreadCount.Location = new System.Drawing.Point(140, 10);
            subtractThreadCount.Name = "beginThreadTest";
            subtractThreadCount.Size = new System.Drawing.Size(30, 30);
            subtractThreadCount.TabStop = false;
            subtractThreadCount.Text = "➖";
            subtractThreadCount.ForeColor = themeColor;
            subtractThreadCount.UseMnemonic = false;
            subtractThreadCount.UseVisualStyleBackColor = true;
            subtractThreadCount.BackColor = ThemeBackgroundColor;
            subtractThreadCount.Click += new System.EventHandler(this.subtractThreadCount_Click);
            threadingPanel.Controls.Add(subtractThreadCount);
            //
            // performanceTimer
            //
            performanceTime = new CustomButton();
            performanceTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            performanceTime.FlatAppearance.BorderSize = 0;
            performanceTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            performanceTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            performanceTime.Location = new System.Drawing.Point(170, 10);
            performanceTime.Name = "beginThreadTest";
            performanceTime.Size = new System.Drawing.Size(100, 30);
            performanceTime.TabStop = false;
            performanceTime.Text = "0 ms";
            performanceTime.ForeColor = themeColor;
            performanceTime.UseMnemonic = false;
            performanceTime.UseVisualStyleBackColor = true;
            performanceTime.BackColor = ThemeBackgroundColor;
            threadingPanel.Controls.Add(performanceTime);
        }

        /// <summary>
        /// Begin threading test when beginThreadtest button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void beginThreadTest_Click(object sender, EventArgs e)
        {
            performanceTime.Text = "" + PerformanceProfile.Profile(()
                => this.discreteFourierToolStripMenuItem_Click(sender, e)) + " ms";
        }

        /// <summary>
        /// Add thread count when addthreadCount button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void addThreadCount_Click(object sender, EventArgs e)
        {
            ThreadSetting.threadNum = Int32.Parse(currentThreadCount.Text) + 1;
            currentThreadCount.Text = "" + ThreadSetting.threadNum;
        }

        /// <summary>
        /// Subtract thread count when subtractThreadCount button is clicked
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void subtractThreadCount_Click(object sender, EventArgs e)
        {
            if (ThreadSetting.threadNum > 1)
            {
                ThreadSetting.threadNum = Int32.Parse(currentThreadCount.Text) - 1;
                currentThreadCount.Text = "" + ThreadSetting.threadNum;
            }
        }

        /// <summary>
        /// update panel position
        /// </summary>
        private void updatePanelPosition()
        {
            if (runThreading)
            {
                threadingPanel.Location = new Point(0, 25 + recorderOffset);
            }
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Run length encoding toggle button
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void runLengthEncodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FlagSetting.runLengthEncoding = !FlagSetting.runLengthEncoding;
            if (FlagSetting.runLengthEncoding)
            {
                runLengthEncodingToolStripMenuItem.CheckState = CheckState.Checked;
            }
            else
            {
                runLengthEncodingToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        /// <summary>
        /// Add a new chart
        /// Does not add more than one
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!newChart)
            {
                addChart("Sound Wave 2", SeriesChartType.Column);
                chartAreaThemeColor(chart.ChartAreas[numOfChartArea - 1],
                    ThemeColor3, SelectedColor3, brush3, lineBrush3, numOfChartArea - 1);
                int size = 3000;
                data = new double[size];
                N = 100;
                for (int i = 0; i < size; i++)
                {
                    data[i] = 3 * Math.Cos(2 * Math.PI * frequencyOne * i / N) + 2 * Math.Cos(2 * Math.PI * i * frequencyTwo / N);
                }
                selectionObject.Datas[numOfChartArea - 1] = data;
                selectionObject.pixelCompress(numOfChartArea - 1, data);
                selectionObject.ScrollPos[numOfChartArea - 1] = 1;
                selectionObject.CurrentScrollPos[numOfChartArea - 1] = 1;
                fillChart(numOfChartArea - 1, "Sound Wave 2");
                newChart = true;
            }
        }
    }
}

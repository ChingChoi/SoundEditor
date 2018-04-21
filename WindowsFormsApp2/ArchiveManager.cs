using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundEditorOptimize
{
    // Keep track of actions related to copy, cut, paste, and delete
    // Allow for undo and redo up to specific archive size
    class ArchiveManager
    {
        double[][] datas;
        double[][][] archives;  // [chartSize][archiveSize][chartData]
        int datasSize;
        int chartIndex;
        int maxArchiveSize;     // each archive has a max size
        int[] archiveIndex;     // archive position per chart area
        int[] archiveSize;      // archive current size per chart area

        public ArchiveManager(double[][] datas)
        {
            this.datas = datas;
            this.datasSize = datas.Length;
            this.chartIndex = 0;
            this.maxArchiveSize = 3;
            this.archiveIndex = new int[datasSize];
            this.archiveIndex[chartIndex] = 0;
            this.archiveSize = new int[datasSize];
            this.archiveSize[chartIndex] = 1;
            this.archives = new double[datasSize][][];
            for (int i = 0; i < datasSize; i++)
            {
                archives[i] = new double[maxArchiveSize][];
            }
            this.archives[chartIndex][archiveIndex[chartIndex]] = this.datas[chartIndex];
        }

        public double[][] Datas { get => datas; set => datas = value; }

        // Save last action
        public void saveAction(int chartIndex, double[] newData)
        {
            // not yet reaching max archive size, pointing at end
            if (archiveSize[chartIndex] < maxArchiveSize && 
                archiveIndex[chartIndex] + 1 == archiveSize[chartIndex])
            {
                archiveSize[chartIndex]++;
                archiveIndex[chartIndex]++;
                archives[chartIndex][archiveIndex[chartIndex]] = newData;
            }
            // Anytime, pointing at middle
            else if (archiveIndex[chartIndex] + 1 < archiveSize[chartIndex])
            {
                archiveIndex[chartIndex]++;
                archiveSize[chartIndex] = archiveIndex[chartIndex] + 1;
                archives[chartIndex][archiveIndex[chartIndex]] = newData;
            }
            // reached max archive size, pointing at end
            else if (archiveSize[chartIndex] == maxArchiveSize && 
                archiveIndex[chartIndex] + 1 == archiveSize[chartIndex])
            {
                for (int i = 0; i < maxArchiveSize - 1; i++)
                {
                    archives[chartIndex][i] = archives[chartIndex][i + 1];
                }
                archives[chartIndex][archiveIndex[chartIndex]] = newData;
            }
        }

        // Undo last action
        public double[] undo(int chartIndex)
        {

            return datas[chartIndex];
        }

        // Redo last undo
        public double[] redo(int chartIndex)
        {

            return datas[chartIndex];
        }
    }
}

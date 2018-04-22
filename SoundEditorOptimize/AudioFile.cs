using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Wav file object
    /// </summary>
    class AudioFile
    {
        string fileName;
        string filePath;
        int chunkID;
        int fileSize;
        int riffType;
        int fmtID;
        int fmtSize;
        int fmtCode;
        int channels;
        int sampleRate;
        int byteRate;
        int fmtBlockAlign;
        int bitDepth;
        int dataID;
        int bytes;
        byte[] byteArray;
        int bytesForSamp;
        int fmtExtraSize;
        int samps;
        float[] L;
        float[] R;
        bool read;

        public AudioFile(AudioHeaderInfo headerInfo)
        {
            bytesForSamp = (int)headerInfo.nAvgBytesPerSec;
            channels = (int)headerInfo.nChannels;
            SampleRate = (int)headerInfo.nSamplesPerSec;
            bitDepth = (int)headerInfo.wBitsPerSample;
            FmtCode = (int)headerInfo.wFormatTag;
            FmtBlockAlign = (int)headerInfo.wFormatTag;
        }

        /// <summary>
        /// Constructor for wav file object by directly opening the file with given path
        /// </summary>
        /// <param name="filePath">Name of file</param>
        public AudioFile(string filePath)
        {
            fmtExtraSize = 0;
            this.fileName = Regex.Match(filePath, "\\w+(?:\\.\\w+)*$").Groups[0].Value;
            this.filePath = filePath;
            if (WavIO.readWav(this))
            {
                Read = true;
            }
            else
            {
                Read = false;
            }
        }

        public string FileName { get => fileName; set => fileName = value; }
        public int ChunkID { get => chunkID; set => chunkID = value; }
        public int FileSize { get => fileSize; set => fileSize = value; }
        public int RiffType { get => riffType; set => riffType = value; }
        public int FmtID { get => fmtID; set => fmtID = value; }
        public int FmtSize { get => fmtSize; set => fmtSize = value; }
        public int Channels { get => channels; set => channels = value; }
        public int SampleRate { get => sampleRate; set => sampleRate = value; }
        public int ByteRate { get => byteRate; set => byteRate = value; }
        public int FmtBlockAlign { get => fmtBlockAlign; set => fmtBlockAlign = value; }
        public int BitDepth { get => bitDepth; set => bitDepth = value; }
        public int DataID { get => dataID; set => dataID = value; }
        public int Bytes { get => bytes; set => bytes = value; }
        public byte[] ByteArray { get => byteArray; set => byteArray = value; }
        public int BytesForSamp { get => bytesForSamp; set => bytesForSamp = value; }
        public int Samps { get => samps; set => samps = value; }
        public float[] L1 { get => L; set => L = value; }
        public float[] R1 { get => R; set => R = value; }
        public int FmtCode { get => fmtCode; set => fmtCode = value; }
        public int FmtExtraSize { get => fmtExtraSize; set => fmtExtraSize = value; }
        public bool Read { get => read; set => read = value; }
        public string FilePath { get => filePath; set => filePath = value; }
    }
}

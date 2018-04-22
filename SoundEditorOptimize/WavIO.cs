using System;
using System.Diagnostics;
using System.IO;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Define wav input output static methods
    /// </summary>
    class WavIO
    {
        /// <summary>
        /// Read wav file given the wav file
        /// </summary>
        /// <param name="audioFile">Wav file</param>
        /// <returns>true if success</returns>
        public static bool readWav(AudioFile audioFile)
        {
            audioFile.L1 = audioFile.R1 = null;
            //float [] left = new float[1];

            //float [] right;
            try
            {
                using (FileStream fs = File.Open(audioFile.FilePath, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    audioFile.ChunkID = reader.ReadInt32();
                    audioFile.FileSize = reader.ReadInt32();
                    audioFile.RiffType = reader.ReadInt32();
                    // chunk 1
                    audioFile.FmtID = reader.ReadInt32();
                    audioFile.FmtSize = reader.ReadInt32(); // bytes for this chunk
                    audioFile.FmtCode = reader.ReadInt16();
                    audioFile.Channels = reader.ReadInt16();
                    audioFile.SampleRate = reader.ReadInt32();
                    audioFile.ByteRate = reader.ReadInt32();
                    audioFile.FmtBlockAlign = reader.ReadInt16();
                    audioFile.BitDepth = reader.ReadInt16();

                    if (audioFile.FmtSize == 18)
                    {
                        // Read any extra values
                        audioFile.FmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(audioFile.FmtExtraSize);
                    }

                    // chunk 2
                    audioFile.DataID = reader.ReadInt32();
                    while (audioFile.DataID != 0x61746164)
                    {
                        int temp = 0;
                        while (temp != 0x6164)
                        {
                            temp = reader.ReadInt16();
                        }
                        int temp2 = reader.ReadInt16();
                        temp2 = temp2 << 16;
                        audioFile.DataID = temp2 + temp;
                    }
                    audioFile.Bytes = reader.ReadInt32();

                    // DATA!
                    audioFile.ByteArray = reader.ReadBytes(audioFile.Bytes);
                    audioFile.BytesForSamp = audioFile.BitDepth / 8;
                    audioFile.Samps = audioFile.Bytes / audioFile.BytesForSamp;

                    float[] asFloat = null;
                    switch (audioFile.BitDepth)
                    {
                        case 64:
                            double[] asDouble = new double[audioFile.Samps];
                            Buffer.BlockCopy(audioFile.ByteArray, 0, asDouble, 0, audioFile.Bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;
                        case 32:
                            asFloat = new float[audioFile.Samps];
                            Buffer.BlockCopy(audioFile.ByteArray, 0, asFloat, 0, audioFile.Bytes);
                            break;
                        case 16:
                            Int16[] asInt16 = new Int16[audioFile.Samps];
                            Buffer.BlockCopy(audioFile.ByteArray, 0, asInt16, 0, audioFile.Bytes);
                            asFloat = Array.ConvertAll(asInt16, e => (float)e /*/ (float)Int16.MaxValue*/);
                            break;
                        case 8:
                            byte[] asByte = new byte[audioFile.Samps];
                            Buffer.BlockCopy(audioFile.ByteArray, 0, asByte, 0, audioFile.Bytes);
                            double[] asDoubleFromByte = ArrayTransform.byteRecordToDouble(asByte);
                            asFloat = Array.ConvertAll(asDoubleFromByte, e => (float)e/* / (float)Int16.MaxValue*/);
                            break;
                        default:
                            return false;
                    }

                    switch (audioFile.Channels)
                    {
                        case 1:
                            audioFile.L1 = asFloat;
                            audioFile.R1 = null;
                            reader.Close();
                            fs.Close();
                            return true;
                        case 2:
                            audioFile.L1 = new float[audioFile.Samps / 2];
                            audioFile.R1 = new float[audioFile.Samps / 2];
                            for (int i = 0, s = 0; i < audioFile.Samps / 2; i++)
                            {
                                audioFile.L1[i] = asFloat[s++];
                                audioFile.R1[i] = asFloat[s++];
                            }
                            reader.Close();
                            fs.Close();
                            return true;
                        default:
                            reader.Close();
                            fs.Close();
                            return false;
                    }

                }
            }
            catch
            {
                Debug.WriteLine("...Failed to load note: " + audioFile.FileName);
                return false;
                //left = new float[ 1 ]{ 0f };
            }
        }

        public static bool writeWav(AudioFile audioFile, Stream f)
        {
            BinaryWriter wr = new BinaryWriter(f);
            wr.Write(audioFile.ChunkID);
            wr.Write(audioFile.FileSize);
            wr.Write(audioFile.RiffType);
            wr.Write(audioFile.FmtID);
            wr.Write(audioFile.FmtSize);
            wr.Write((Int16)audioFile.FmtCode);
            wr.Write((Int16)audioFile.Channels);
            wr.Write(audioFile.SampleRate);
            wr.Write(audioFile.ByteRate);
            wr.Write((Int16)audioFile.FmtBlockAlign);
            wr.Write((Int16)audioFile.BitDepth);
            wr.Write(audioFile.DataID);
            wr.Write(audioFile.Bytes);
            if (audioFile.Channels == 2)
            {
                switch (audioFile.BitDepth)
                {
                    case 64:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((double)audioFile.L1[i]);
                            wr.Write((double)audioFile.R1[i]);
                        }
                        break;
                    case 32:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write(audioFile.L1[i]);
                            wr.Write(audioFile.R1[i]);
                        }
                        break;
                    case 16:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((Int16)(audioFile.L1[i]/* * (float)Int16.MaxValue*/));
                            wr.Write((Int16)(audioFile.R1[i]/* * (float)Int16.MaxValue*/));
                        }
                        break;
                    case 8:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((byte)(audioFile.L1[i]));
                            wr.Write((byte)(audioFile.R1[i]));
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (audioFile.BitDepth)
                {
                    case 64:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((double)audioFile.L1[i]);
                        }
                        break;
                    case 32:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write(audioFile.L1[i]);
                        }
                        break;
                    case 16:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((Int16)audioFile.L1[i]/* * (float)Int16.MaxValue*/);
                        }
                        break;
                    case 8:
                        for (int i = 0; i < audioFile.L1.Length; i++)
                        {
                            wr.Write((byte)audioFile.L1[i]);
                        }
                        break;
                    default:
                        break;
                }
            }
            wr.Close();
            f.Close();
            return false;
        }
    }
}

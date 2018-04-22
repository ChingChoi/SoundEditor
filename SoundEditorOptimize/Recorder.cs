//
// Author: Choi, Ching
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Recorder library imports
    /// </summary>
    class Recorder
    {
        public const int HALF_BYTE_SIZE = 128;
        public const int BYTE_SIZE = 256;
        public const int BASE_MULTIPLIER = 50;

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool runRecorder();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool beginRecord();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool endRecord();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playRecord();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playPause();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playEnd();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playReverse();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playRepeat();

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern bool playSpeed();

        [DllImport("RecordLibraryT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, EntryPoint = "getData")]
        public unsafe static extern IntPtr getData();

        [DllImport("RecordLibraryT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, EntryPoint = "GetHeaderInfo")]
        public unsafe static extern AudioHeaderInfo GetHeaderInfo();
        
        [DllImport("RecordLibraryT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, EntryPoint = "SetDataSize")]
        public unsafe static extern uint SetDataSize(uint size);

        [DllImport("RecordLibraryT.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto, EntryPoint = "SetHeaderInfo")]
        public unsafe static extern bool SetHeaderInfo(AudioHeaderInfo inputHeaderInfo);

        [DllImport("RecordLibraryT.dll", CharSet = CharSet.Auto)]
        public static extern uint getDwDataLength();  
    }
}

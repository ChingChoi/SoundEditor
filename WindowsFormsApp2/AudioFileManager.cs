using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundEditorOptimize
{
    /// <summary>
    /// Manages all wav file objects
    /// </summary>
    class AudioFileManager
    {
        AudioFile[] audioFiles;

        /// <summary>
        /// Constructor of AudioFileManager
        /// </summary>
        public AudioFileManager(int size)
        {
            audioFiles = new AudioFile[size];
        }

        public AudioFile[] AudioFiles { get => audioFiles; set => audioFiles = value; }
    }
}

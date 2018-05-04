using System;
using System.Diagnostics;

namespace SoundEditorOptimize
{
    class PerformanceProfile
    {
        public static long Profile(Action method)
        {
            Stopwatch st = Stopwatch.StartNew();
            method();
            st.Stop();
            return st.ElapsedMilliseconds;
        }
    }
}
using NALRage.Entities;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine
{
    public static class Common
    {
        public static int Cash { get; internal set; }
        public static int Kills { get; internal set; }
        public static bool Blackout { get; internal set; }
        public static BigMessageThread BigMessage { get; internal set; } = new BigMessageThread();
        public static Difficulty Difficulty { get; internal set; }
    }
}

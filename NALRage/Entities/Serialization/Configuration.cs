using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Entities.Serialization
{
    public struct Configuration
    {
        public Configuration(int version)
        {
            Version = version;
            EventRequirement = 59;
            EventMax = 89;
            EventMinimal = 9;
            ProcessInterval = 100;
            DefaultDifficulty = Difficulty.Initial;
        }

        public int Version { get; set; }
        public int EventRequirement { get; set; }
        public int EventMax { get; set; }
        public int EventMinimal { get; set; }
        public int ProcessInterval { get; set; }
        public Difficulty DefaultDifficulty { get; set; }
    }
}

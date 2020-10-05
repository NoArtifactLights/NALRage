using Rage;
using System;

namespace NALRage.Entities.Serialization
{
    public struct SaveFile
    {
        public int Version { get; set; }
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public float PlayerZ { get; set; }
        public WorldStatus Status { get; set; }
        public bool Blackout { get; set; }
        public Difficulty CurrentDifficulty { get; set; }
        public int Kills { get; set; }
        public int Cash { get; set; }
        public SaveWeaponDescriptor[] Weapons { get; set; }
        public int PlayerArmor { get; set; }
        public int PlayerHealth { get; set; }
    }
}

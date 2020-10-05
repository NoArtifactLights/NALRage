using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Entities.Serialization
{
    public struct SaveWeaponDescriptor
    {
        public SaveWeaponDescriptor(WeaponDescriptor wd)
        {
            if(wd == null)
            {
                throw new ArgumentNullException();
            }
            Hash = wd.Hash;
            Ammo = wd.Ammo;
            LoadedAmmo = wd.LoadedAmmo;
        }

        public void AddToPlayer()
        {
            Game.LocalPlayer.Character.Inventory.GiveNewWeapon(Hash, Ammo, false);
        }

        public WeaponHash Hash { get; set; }
        public short Ammo { get; set; }
        public short LoadedAmmo { get; set; }
        

    }
}

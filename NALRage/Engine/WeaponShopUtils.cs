using Rage;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine
{
    public static class WeaponShopUtils
    {
        public static WeaponDescriptor GetWeaponFromHash(this WeaponDescriptorCollection collection, WeaponHash hash)
        {
            foreach(WeaponDescriptor wd in collection)
            {
                if (wd.Hash == hash) return wd;
                else continue;
            }
            return null;
        }

        private static Vector3[] ammus = { new Vector3(18.18945f, -1120.384f, 28.91654f), new Vector3(-325.6184f, 6072.246f, 31.21228f) };
        internal static UIMenuItem GenerateWeaponSellerItem(string displayName, string description, int price)
        {
            Game.LogTrivial("Creating weapon sell item for: " + displayName + " at price " + price);
            UIMenuItem result = new UIMenuItem(displayName, description);
            result.SetRightLabel("$" + price);
            Game.LogTrivial("Created weapon sell item for: " + displayName);
            return result;
        }

        internal static bool DistanceToAmmu()
        {
            foreach (Vector3 ammu in ammus)
            {
                if (Game.LocalPlayer.Character.Position.DistanceTo(ammu) < 7f) return true;
                else continue;
            }
            return false;
        }

        internal static void SellWeapon(int price, short ammo, WeaponHash weapon)
        {
            if (Common.Cash < price)
            {
                Game.DisplaySubtitle("You don't have enough money for this.");
                return;
            }
            Common.Cash -= price;
            try
            {
                if (Game.LocalPlayer.Character.Inventory.Weapons.Contains(weapon))
                {
                    WeaponDescriptor wp = Game.LocalPlayer.Character.Inventory.Weapons.GetWeaponFromHash(weapon);
                    if(wp == null)
                    {
                        Game.LocalPlayer.Character.Inventory.GiveNewWeapon(weapon, ammo, true);
                        return;
                    }
                    wp.Ammo += ammo;
                }
                else
                {
                    Game.LocalPlayer.Character.Inventory.GiveNewWeapon(weapon, ammo, true);
                }
            }
            catch
            {
                Common.Cash += price;
            }

        }

        internal static void SellArmor(int amount, int price)
        {
            if (Common.Cash < price)
            {
                Game.DisplaySubtitle("You don't have enough money to buy this.");
                return;
            }
            Common.Cash -= price;
            if (Game.LocalPlayer.Character.Armor >= amount)
            {
                Game.DisplaySubtitle("You already had armor.");
                return;
            }
            Game.LocalPlayer.Character.Armor = amount;
        }
    }
}

using Rage;
using Rage.Native;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NALRage.Engine.Menus
{
    public static class MenuManager
    {
        public static MenuPool Pool { get; private set; } = new MenuPool();
        private static UIMenu mainMenu;
        private static UIMenuItem itemSave;
        private static UIMenuItem itemLoad;
        private static UIMenuItem itemCallCops;
        private static UIMenuItem itemDifficulty;
        private static UIMenuItem itemKills;
        private static UIMenuItem itemAppearance;
        private static UIMenuCheckboxItem itemLights;
        private static UIMenuItem itemCash;

        private static UIMenu buyMenu;
        private static UIMenuItem itemPistol;
        private static UIMenuItem itemPumpShotgun;
        private static UIMenuItem itemBodyArmor;

        private static UIMenu modelsMenu;
        private static UIMenuItem itemCop;
        private static UIMenuItem itemClassic;
        private static bool noticed;

        public static void FiberInit()
        {
            mainMenu = new UIMenu("NAL", "Main Menu");
            itemLights = new UIMenuCheckboxItem("Blackout", true, "Sets whether to turn off power of whole San Andreas.");
            itemSave = new UIMenuItem("Save Game", "Saves the current game status to a save file.");
            itemLoad = new UIMenuItem("Load Game", "Loads the game status from a save file.");
            itemCallCops = new UIMenuItem("Call the Cops", "Call for police services.");
            itemDifficulty = new UIMenuItem("Difficulty", "Views the current difficulty.");
            itemKills = new UIMenuItem("Kills", "Views the current kill count.");
            itemAppearance = new UIMenuItem("Appearance", "Changes the model of the current player.");
            itemCash = new UIMenuItem("Cash", "Views the current cash amount.");
            mainMenu.AddItem(itemLights);
            mainMenu.AddItem(itemSave);
            mainMenu.AddItem(itemLoad);
            mainMenu.AddItem(itemCallCops);
            mainMenu.AddItem(itemDifficulty);
            mainMenu.AddItem(itemKills);
            mainMenu.AddItem(itemAppearance);
            mainMenu.AddItem(itemCash);
            itemLights.CheckboxEvent += ItemLights_CheckboxEvent;
            itemSave.Activated += ItemSave_Activated;
            itemLoad.Activated += ItemLoad_Activated;
            itemCallCops.Activated += ItemCallCops_Activated;

            modelsMenu = new UIMenu("NAL", "Appearence Menu");
            itemCop = new UIMenuItem("Male Cop", "The male LSPD officer.");
            itemClassic = new UIMenuItem("Classic", "The classic NAL load model.");
            itemCop.Activated += ItemCop_Activated;
            itemClassic.Activated += ItemClassic_Activated;

            mainMenu.BindMenuToItem(modelsMenu, itemAppearance);
            mainMenu.RefreshIndex();
            Pool.Add(mainMenu);
            Pool.Add(modelsMenu);
            buyMenu = new UIMenu("Ammu-Nation", "Weapon Shop");
            itemPistol = WeaponShopUtils.GenerateWeaponSellerItem("Pistol Ammo x100", "A personal defense weapon that is easy to carry, but has limited clip.", 1000);
            itemPumpShotgun = WeaponShopUtils.GenerateWeaponSellerItem("Pump Shotgun Ammo x50", "A weapon has short range but has strong power when enemy comes close.", 2000);
            itemBodyArmor = WeaponShopUtils.GenerateWeaponSellerItem("Standard Body Armor", "This armor can defend one shotgun round in min-range and can defend several pistol rounds.", 3500);
            itemPistol.Activated += ItemPistol_Activated;
            itemPumpShotgun.Activated += ItemPumpShotgun_Activated;
            itemBodyArmor.Activated += ItemBodyArmor_Activated;
            buyMenu.AddItem(itemPistol);
            buyMenu.AddItem(itemPumpShotgun);
            buyMenu.AddItem(itemBodyArmor);
            buyMenu.RefreshIndex();
            Pool.Add(buyMenu);
            while(true)
            {
                GameFiber.Yield();
                Pool.ProcessMenus();
                if(!noticed)
                {
                    noticed = true;
                    //Game.DisplayNotification("NoArtifactLights ~b~has been loaded~s~. ~g~Enjoy!~s~");
                    //Game.LogTrivial("MenuManager thread has entered loop. Enjoy!");
                }
                if(!buyMenu.Visible)
                {
                    if(Game.IsKeyDown(Keys.N))
                    {
                        Game.LogTrivial("Key is N. hit!");
                        itemCash.SetRightLabel(Common.Cash + "$");
                        itemKills.SetRightLabel(Common.Kills.ToString());
                        itemDifficulty.SetRightLabel(Common.Difficulty.ToString());
                        mainMenu.Visible = !mainMenu.Visible;
                    }
                    if(Game.IsKeyDown(Keys.E) && WeaponShopUtils.DistanceToAmmu())
                    {
                        buyMenu.Visible = !buyMenu.Visible;
                    }
                }
                if(WeaponShopUtils.DistanceToAmmu())
                {
                    Game.DisplayHelp("Press ~INPUT_CONTEXT~ to buy weapon.");
                }
            }
        }

        private static void ItemClassic_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            ChangeModel("a_m_m_bevhills_02", selectedItem);
        }

        private static void ItemCop_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            ChangeModel("s_m_y_cop_01", selectedItem);            
        }

        private static void ChangeModel(string model, UIMenuItem origin)
        {
            List<WeaponDescriptor> weapons = new List<WeaponDescriptor>();
            foreach(WeaponDescriptor wd in Game.LocalPlayer.Character.Inventory.Weapons)
            {
                weapons.Add(wd);
            }
            Game.LocalPlayer.Model = model;
            foreach(WeaponDescriptor wd in weapons)
            {
                Game.LocalPlayer.Character.Inventory.Weapons.Add(wd);
            }
            itemClassic.Enabled = true;
            itemCop.Enabled = true;
            origin.Enabled = false;
        }

        private static void ItemBodyArmor_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopUtils.SellArmor(70, 350);
        private static void ItemPumpShotgun_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopUtils.SellWeapon(200, 50, WeaponHash.PumpShotgun);
        private static void ItemPistol_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopUtils.SellWeapon(100, 100, WeaponHash.Pistol);


        private static void ItemCallCops_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            NativeFunction.Natives.CREATE_INCIDENT(7, Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 2, 3.0f, new NativePointer());
        }

        private static void ItemLoad_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            SaveUtils.SaveManager.Load();
        }

        private static void ItemSave_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            SaveUtils.SaveManager.Save(Common.Blackout);
        }

        private static void ItemLights_CheckboxEvent(UIMenuCheckboxItem sender, bool Checked)
        {
            Common.Blackout = Checked;
            NativeFunction.Natives.x1268615ACE24D504(Checked);
        }
    }
}

using NALRage.Entities.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NALRage.Engine
{
    public static class SaveUtils
    {
        internal static class SaveManager
        {
            static readonly string[] weatherNames = {
            "EXTRASUNNY",
            "CLEAR",
            "CLOUDS",
            "SMOG",
            "FOGGY",
            "OVERCAST",
            "RAIN",
            "THUNDER",
            "CLEARING",
            "NEUTRAL",
            "SNOW",
            "BLIZZARD",
            "SNOWLIGHT",
            "XMAS",
            "HALLOWEEN"
        };

            internal static WeatherType Weather
            {
                get
                {
                    uint hash = NativeFunction.Natives.x564B884A05EC45A3<uint>();
                    for (int i = 0; i < weatherNames.Length; i++)
                    {
                        if (hash == Game.GetHashKey(weatherNames[i]))
                        {
                            return (WeatherType)i;
                        }
                    }
                    return WeatherType.ExtraSunny;

                }

                set
                {
                    if (Enum.IsDefined(typeof(WeatherType), value))
                    {
                        NativeFunction.Natives.SET_WEATHER_TYPE_NOW(weatherNames[(int)value]);
                    }
                }
            }

            internal static void CheckAndFixDataFolder()
            {
                Game.LogTrivial("Checking and fixing data folders...");
                if (!Directory.Exists("NAL")) Directory.CreateDirectory("NAL");
            }

            internal static void Load()
            {
                if (!File.Exists("Newtonsoft.Json.dll"))
                {
                    Game.DisplayNotification("~h~WARNING!~s~ Place ~b~Newtonsoft.Json~s~ in game folder to make save/load function work.");
                    Game.DisplayNotification("This action ~g~does not~s~ requires a game or plugin restart.");
                    return;
                }
                if (!File.Exists("NAL\\Save.json"))
                {
                    Game.DisplayNotification("No save file found.");
                    return;
                }
                SaveFile sf;
                sf = JsonConvert.DeserializeObject<SaveFile>(File.ReadAllText("NAL\\Save.json"));
                if(sf.Version != 3)
                {
                    Game.DisplayNotification("Unsupported save file.");
                    return;
                }
                Weather = sf.Status.CurrentWeather;
                World.TimeOfDay = new TimeSpan(sf.Status.Hour, sf.Status.Minute, 0);
                NativeFunction.Natives.x1268615ACE24D504(sf.Blackout);
                Common.Blackout = sf.Blackout;
                Game.LocalPlayer.Character.Position = new Vector3(sf.PlayerX, sf.PlayerY, sf.PlayerZ);
                Common.Kills = sf.Kills;
                Common.Cash = sf.Cash;
                Common.Difficulty = sf.CurrentDifficulty;
                GameContentUtils.SetRelationship(sf.CurrentDifficulty);
                Game.LocalPlayer.Character.Inventory.Weapons.Clear();
                if(sf.Weapons.Length != 0)
                {
                    foreach(SaveWeaponDescriptor swd in sf.Weapons)
                    {
                        swd.AddToPlayer();
                    }
                }
                Game.LocalPlayer.Character.Inventory.GiveFlashlight();
            }

            internal static void Save(bool blackout)
            {
                if(!File.Exists("Newtonsoft.Json.dll"))
                {
                    Game.DisplayNotification("~h~WARNING!~s~ Place ~b~Newtonsoft.Json~s~ in game folder to make save/load function work.");
                    Game.DisplayNotification("This action ~g~does not~s~ requires a game or plugin restart.");
                    return;
                }
                SaveFile sf = new SaveFile();
                sf.Version = 3;
                sf.Status = new WorldStatus(Weather, World.DateTime.Hour, World.DateTime.Minute);
                sf.PlayerX = Game.LocalPlayer.Character.Position.X;
                sf.PlayerY = Game.LocalPlayer.Character.Position.Y;
                sf.PlayerZ = Game.LocalPlayer.Character.Position.Z;
                sf.Blackout = blackout;
                sf.Kills = Common.Kills;
                sf.CurrentDifficulty = Common.Difficulty;
                sf.Cash = Common.Cash;
                sf.PlayerHealth = Game.LocalPlayer.Character.Health;
                sf.PlayerArmor = Game.LocalPlayer.Character.Armor;
                List<SaveWeaponDescriptor> wds = new List<SaveWeaponDescriptor>();
                foreach(WeaponDescriptor wd in Game.LocalPlayer.Character.Inventory.Weapons)
                {
                    SaveWeaponDescriptor swd = new SaveWeaponDescriptor(wd);
                    wds.Add(swd);
                }
                sf.Weapons = wds.ToArray();
                CheckAndFixDataFolder();
                string result = JsonConvert.SerializeObject(sf);
                File.WriteAllText("NAL\\Save.json", result);
            }
        }
    }
}

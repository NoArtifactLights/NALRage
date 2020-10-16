// Copyright (C) Hot Workshop & contributors 2020.
// Licensed under GNU General Public License version 3.

using NALRage.Engine;
using NALRage.Engine.Menus;
using NALRage.Engine.Modification;
using NALRage.Engine.Modification.GameFibers;
using NALRage.Entities;
using NALRage.Entities.Serialization;
using Newtonsoft.Json;
using Rage;
using Rage.Attributes;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

[assembly: Plugin("NoArtifactLights", Author = "RelaperCrystal", PrefersSingleInstance = true)]

namespace NALRage
{
    public static class Entry
    {
        internal static Configuration config;
        private static GameFiber process;
        private static System.Timers.Timer timer;
        private static bool enabled = true;

        [ConsoleCommand(Name = "ReloadConfigs", Description = "Reloads configuration of NAL.")]
        private static Configuration GetConfig()
        {
            Configuration result;
            SaveUtils.SaveManager.CheckAndFixDataFolder();
            if(!File.Exists("NAL\\Config.json"))
            {
                result = new Configuration(1);
                File.WriteAllText("NAL\\Config.json", JsonConvert.SerializeObject(result));
                return result;
            }
            string s;
            try
            {
                s = File.ReadAllText("NAL\\Config.json");
            }
            catch(Exception ex)
            {
                Game.LogTrivial("Exception caught when loading config");
                Game.LogTrivial(ex.ToString());
                s = "";
            }
            if(string.IsNullOrWhiteSpace(s))
            {
                result = new Configuration(1);
                File.WriteAllText("NAL\\Config.json", JsonConvert.SerializeObject(result));
                return result;
            }
            try
            {
                result = JsonConvert.DeserializeObject<Configuration>(s);
            }
            catch(Exception ex)
            {
                Game.LogTrivial("Exception caught when deserialzing config: ");
                Game.LogTrivial(ex.ToString());
                result = new Configuration(1);
                File.WriteAllText("NAL\\Config.json", JsonConvert.SerializeObject(result));
            }
            if(result.Version != 1)
            {
                result = new Configuration(1);
                File.WriteAllText("NAL\\Config.json", JsonConvert.SerializeObject(result));
                return result;
            }
            return result;
        }

        public static void Main()
        {
            try
            {
                Game.FadeScreenOut(1000);
                GameFiber.Sleep(1000);
                Game.LogTrivial("Initializing NAL...");
                GetConfig();
                Game.LogTrivial("Setting prop density and loading online map...");
                NativeFunction.Natives.x0888C3502DBBEEF5(); // ON_ENTER_MP
                NativeFunction.Natives.x9BAE5AD2508DF078(1); // SET_INSTANCE_PRIORITY_MODE
                Game.LogTrivial("Online map loaded. Changing player model...");
                Game.LocalPlayer.Model = "a_m_m_bevhills_02";
                Game.LogTrivial("Player model changed.");
                Game.LogTrivial("Teleporting player...");
                Game.LocalPlayer.Character.Position = new Vector3(459.8501f, -1001.404f, 24.91487f);
                Game.LocalPlayer.Character.Inventory.GiveFlashlight();
                Game.LocalPlayer.Character.Inventory.GiveNewWeapon(WeaponHash.Pistol, 50, true);
                Game.LogTrivial("Setting up game...");
                Game.MaxWantedLevel = 0;
                GameContentUtils.SetRelationship(Difficulty.Initial);
                NativeFunction.Natives.x1268615ACE24D504(true);
                Game.LogTrivial("Setting up menus...");
                GameFiber menu = new GameFiber(new ThreadStart(MenuManager.FiberInit));
                menu.Start();
                process = GameFiber.ExecuteNewFor(new ThreadStart(GameManager.ProcessEach100), 100);
                GameFiber.Sleep(5000);
                Game.FadeScreenIn(1000);
                Game.LogTrivial("Done!");
                Game.DisplayHelp("Welcome to NoArtifactLights!");
                Game.DisplayNotification("You have currently playing the ~h~RAGE Plugin Hook~s~ version.");
                GameFiber.Hibernate();
            }
            catch(Exception ex)
            {
                CrashReporter cr = new CrashReporter(ex);
                cr.ReportAndCrashPlugin();
            }
        }

        /// <summary>
        /// Consider use <see cref="GameManager.ProcessEach100()"/> as of this is no longer used.
        /// </summary>
        [Obsolete]
        public static void ProcessEvents()
        {
            List<uint> ids = new List<uint>();
            List<uint> armedIds = new List<uint>();
            List<uint> armedCarIds = new List<uint>();
            List<uint> killedIds = new List<uint>();
            while(true)
            {
                GameFiber.Yield();
                Ped[] peds = World.GetAllPeds();
                
                foreach(Ped p in peds)
                {
                    if(!p.Exists())
                    {
                        continue;
                    }
                    if (p.HasBeenDamagedBy(Game.LocalPlayer.Character) && p.IsDead && !killedIds.Contains(p.Handle.Index))
                    {
                        killedIds.Add(p.Handle.Index);
                        Common.Kills++;
                        Common.Cash += new Random().Next(5, 15);
                        if (armedIds.Contains(p.Handle.Index))
                        {
                            Common.Cash += 30;
                            Game.DisplayHelp("Kill armed NPC bonus +$10");
                        }
                        if (p.GetAttachedBlip().Exists())
                        {
                            p.GetAttachedBlip().Delete();
                        }
                        switch (Common.Kills)
                        {
                            case 1:
                                Game.DisplayHelp("You just killed a person. Once you killed amount of person, the difficulty will raise.");
                                break;

                            case 100:
                                Common.Difficulty = Difficulty.Easy;
                                Common.BigMessage.MessageInstance.ShowSimpleShard("Difficulty Changed", "Difficulty is changed to Easy.");
                                GameContentUtils.SetRelationship(Difficulty.Easy);
                                break;

                            case 300:
                                Common.Difficulty = Difficulty.Normal;
                                Common.BigMessage.MessageInstance.ShowSimpleShard("Difficulty Changed", "Difficulty is changed to Normal.");
                                GameContentUtils.SetRelationship(Difficulty.Normal);
                                break;

                            case 700:
                                Common.Difficulty = Difficulty.Hard;
                                Common.BigMessage.MessageInstance.ShowSimpleShard("Difficulty Changed", "Difficulty is changed to Hard.");
                                GameContentUtils.SetRelationship(Difficulty.Hard);
                                break;

                            case 1500:
                                Common.Difficulty = Difficulty.Nether;
                                Common.BigMessage.MessageInstance.ShowSimpleShard("Difficulty Changed", "Difficulty is changed to Nether.");
                                GameContentUtils.SetRelationship(Difficulty.Nether);
                                break;
                        }
                    }
                //if (!enabled) // we don't want our while runs until 100 interval
                //{
                //    continue;
                //}
                //else
                //{
                //}
                //enabled = false;
                //GameFiber.Sleep(1);
                foreach (Ped p2 in peds)
                {
                    if (!p2.Exists()) continue;
                    if (!p2.IsHuman) continue;
                    

                        
                    }
                    int var = new Random().Next(config.EventMinimal, config.EventMax);
                    if (var == config.EventRequirement && !armedIds.Contains(p.Handle.Index) && !(p.Model.Name == "s_m_y_cop_01" || p.Model.Name == "s_f_y_cop_01") && !p.IsPlayer)
                    {
                        armedIds.Add(p.Handle.Index);
                        p.EquipWeapon();
#if DEBUG
                        Game.DisplayNotification("Creating event");
#endif
                        if(p.IsInAnyVehicle(false))
                        {
                            armedCarIds.Add(p.CurrentVehicle.Handle.Index);
                        }
                    }
                    else
                    {
#if DEBUG
#endif
                    }
                    
                }
                if (ids.Count == 10000)
                {
                    Game.LogTrivial("Cleaning ids");
                    ids.Clear();
                }
                if (armedIds.Count == 10000)
                {
                    Game.LogTrivial("Cleaning armed ids");
                    armedIds.Clear();
                }
                if (armedCarIds.Count == 10000)
                {
                    Game.LogTrivial("Clearing armed car ids");
                    armedIds.Clear();
                }
                if (killedIds.Count == 100)
                {
                    Game.LogTrivial("Cleaning killed ids");
                    killedIds.Clear();
                }
                GameFiber.Sleep(100);
            }
        }
    }
}

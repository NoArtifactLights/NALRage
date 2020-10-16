using NALRage.Engine.Modification.API.Events;
using NALRage.Entities;
using Rage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine.Modification.GameFibers
{
    internal static class GameManager
    {
        private static List<PoolHandle> peds = new List<PoolHandle>();
        private static List<PoolHandle> armedPeds = new List<PoolHandle>();
        private static List<PoolHandle> killedPeds = new List<PoolHandle>();

        internal static void ProcessEach100()
        {
            Ped[] peds = World.GetAllPeds();
            foreach(Ped p in peds)
            {
                GameFiber.Yield();
                if (!p.Exists()) continue;
                // Detects whether a ped was killed by the player
                if(p.HasBeenDamagedBy(Game.LocalPlayer.Character) && p.IsDead && !killedPeds.Contains(p.Handle))
                {
                    killedPeds.Add(p.Handle);
                    Common.Kills++;
                    Common.Cash += new Random().Next(5, 15);
                    if(armedPeds.Contains(p.Handle))
                    {
                        Common.Cash += Common.armedBouns;
                        Game.DisplayHelp("Kill armed ped bonus +$" + Common.armedBouns);
                    }

                    // Checks and removes it's blip as this ped is currently dead.
                    if (p.GetAttachedBlip().Exists())
                    {
                        p.GetAttachedBlip().Delete();
                    }
                    DetermineDiff();
                }
            }
            foreach(Ped p2 in peds)
            {
                if (!p2.Exists()) continue;
                // There is a bug earliar causing a lion to be flagged
                if (!p2.IsHuman) continue;

                int var = new Random().Next(Entry.config.EventMinimal, Entry.config.EventMax);
                if (var == Entry.config.EventRequirement && !armedPeds.Contains(p2.Handle) && !(p2.Model.Name == "s_m_y_cop_01" || p2.Model.Name == "s_f_y_cop_01") && !p2.IsPlayer)
                {
                    EventManager.StartRandomEvent(p2);
                }

            }
            if (GameManager.peds.Count == 10000)
            {
                Game.LogTrivial("Cleaning ids");
                GameManager.peds.Clear();
            }
            if (armedPeds.Count == 1000)
            {
                Game.LogTrivial("Cleaning armed ids");
                armedPeds.Clear();
            }
            if (killedPeds.Count == 100)
            {
                Game.LogTrivial("Cleaning killed ids");
                killedPeds.Clear();
            }
        }

        private static void DetermineDiff()
        {
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
    }
}

using NALRage.Entities;
using Rage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NALRage.Engine
{
    public static class GameContentUtils
    {
        public static readonly RelationshipGroup CivilianMale = new RelationshipGroup("CIVMALE");
        public static readonly RelationshipGroup CivilianFemale = new RelationshipGroup("CIVFEMALE");

        public static void SetRelationship(Difficulty difficulty)
        {
            Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, CivilianFemale, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(CivilianFemale, CivilianMale, Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Player, RelationshipGroup.Cop, Relationship.Companion);
            Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.Cop, RelationshipGroup.Player, Relationship.Companion);
            switch (difficulty)
            {
                default:
                case Difficulty.Initial:
                    break;
                case Difficulty.Easy:
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Player, Relationship.Hate);
                    break;
                case Difficulty.Normal:
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Player, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Cop, Relationship.Hate);
                    break;
                case Difficulty.Hard:
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Player, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Cop, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, CivilianMale, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, CivilianMale, Relationship.Hate);
                    break;
                case Difficulty.Nether:
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Player, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.Cop, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.AmbientGangBallas, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.AmbientGangFamily, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangBallas, CivilianMale, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(RelationshipGroup.AmbientGangFamily, CivilianMale, Relationship.Hate);
                    Game.SetRelationshipBetweenRelationshipGroups(CivilianMale, RelationshipGroup.DomesticAnimal, Relationship.Hate);
                    break;
            }
        }

        internal static void EquipWeapon(this Ped ped)
        {
            if(!ped.Exists())
            {
                return;
            }
            ped.IsPersistent = true;
            WeaponHash wp;
            switch (Common.Difficulty)
            {
                default:
                case Difficulty.Initial:
                    if (new Random().Next(200, 272) == 40) wp = WeaponHash.PumpShotgun;
                    else wp = WeaponHash.Pistol;
                    break;

                case Difficulty.Easy:
                    wp = WeaponHash.PumpShotgun;
                    break;

                case Difficulty.Normal:
                    wp = WeaponHash.MicroSMG;
                    break;

                case Difficulty.Hard:
                    wp = WeaponHash.CarbineRifle;
                    break;

                case Difficulty.Nether:
                    wp = WeaponHash.RPG;
                    break;
            }
            ped.Inventory.GiveNewWeapon(wp, short.MaxValue, true);
            if(ped.IsInAnyVehicle(false))
            {
                ped.Tasks.LeaveVehicle(ped.CurrentVehicle, LeaveVehicleFlags.LeaveDoorOpen);
                GameFiber.Wait(1);
            }
            ped.Dismiss();
            ped.KeepTasks = true;
            Blip b = ped.AttachBlip();
            b.IsFriendly = false;
            b.Sprite = BlipSprite.Enemy;
            b.Scale = 0.5f;
            b.Color = Color.Red;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CombatExtended;
using RimWorld;
using Verse;
using Verse.AI;
using HarmonyLib;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace VFESecurity
{
    [HarmonyPatch(typeof(JobDriver_ManTurret), nameof(JobDriver_ManTurret.FindAmmoForTurret))]
    public static class Patch_TryFindRandomShellDef
    {
        /// <summary>
        /// Maintain compatibility with existing mods that attempt to fix this issue while allowing other mods to register non-shell based artillery
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="gun"></param>
        /// <param name="__result"></param>
        private static bool Prefix(Pawn pawn, Building_TurretGunCE gun, ref Thing __result)
        {
            if (gun.TryGetArtillery(out var group))
            {
                Log.Message($"Trying to get custom ammo for {gun.Label}");
                //TruGerman: I'm not sure if it's even possible to convert from an AmmoSet to StorageSettings. Either way, this is something for the experienced modders to figure out
                StorageSettings allowedShellsSettings =  RetrieveParentSettings(gun);
                bool validator(Thing t) => !t.IsForbidden(pawn) && pawn.CanReserve(t, 10, 1, null, false) && (allowedShellsSettings == null || allowedShellsSettings.AllowedToAccept(t));
                __result = GenClosest.ClosestThingReachable(gun.Position, gun.Map, ThingRequest.ForGroup(group), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 
                    40f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
                return false;
            }
            return true;
        }

        private static StorageSettings RetrieveParentSettings(Building_TurretGunCE gun)
        {
            return gun.Gun.TryGetComp<CompAmmoUser>().parent.def.building.fixedStorageSettings;
        }
    }
}

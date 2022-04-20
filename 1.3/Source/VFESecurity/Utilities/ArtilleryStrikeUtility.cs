using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CombatExtended;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;
using Random = System.Random;

namespace VFESecurity
{
    public static class ArtilleryStrikeUtility
    {
        //TruGerman: Global fields for debugging
        private static Random rng = new Random();
        private static List<AmmoDef> allowedEnemyShellDefs = new List<AmmoDef>();

        //TruGerman: It's now iterating through all AmmoDefs instead of ThingDefs, saving me type checks and whatnot. I'm not sure if item.detonateProjectile.projectile is the correct way of doing it, but it's working. Refactored into a loop to help with debugging
        public static void SetCache()
        {
            for (int i = 0; i < DefDatabase<AmmoDef>.AllDefsListForReading.Count; i++)
            {
                var item = DefDatabase<AmmoDef>.AllDefsListForReading[i];
                if (item.isMortarAmmo && item.detonateProjectile != null && item.detonateProjectile.projectile.damageDef.harmsHealth)
                {
                    allowedEnemyShellDefs.Add(item);
                }
            }
        }

        //TruGerman: Same deal here, for loops are far easier to debug than LINQ
        public static AmmoDef GetRandomShellFor(ThingDef artilleryGunDef, FactionDef faction)
        {
            List<AmmoDef> validShells = new List<AmmoDef>();
            for (int i = 0; i < allowedEnemyShellDefs.Count; i++)
            {
                var thingDef = allowedEnemyShellDefs[i];
                if(thingDef.techLevel <= faction.techLevel && artilleryGunDef.HasComp(typeof(CompAmmoUser)) && CanAcceptAmmo(artilleryGunDef.GetCompProperties<CompProperties_AmmoUser>(), thingDef)) validShells.Add(thingDef);
            }

            return validShells[rng.Next(validShells.Count)];
            //TruGerman: Original method
            //return allowedEnemyShellDefs.Where(s => s.techLevel <= faction.techLevel && artilleryGunDef.building.defaultStorageSettings.AllowedToAccept(s)).Select(s => s.projectileWhenLoaded).RandomElement();
        }

        //TruGerman: Another easy to debug method. Seems crude, but it works?
        private static bool CanAcceptAmmo(CompProperties_AmmoUser prop, AmmoDef shellDef) => shellDef.AmmoSetDefs.Contains(prop.ammoSet);

        public static float FinalisedMissRadius(float forcedMissRadius, float maxRadiusFactor, int tileA, int tileB, int range)
        {
            return forcedMissRadius * Mathf.Lerp(1, maxRadiusFactor, (float)Find.WorldGrid.TraversalDistanceBetween(tileA, tileB) / range);
        }

        public static IEnumerable<IntVec3> PotentialStrikeCells(Map map, float missRadius)
        {
            return missRadius < GenRadial.MaxRadialPatternRadius ? GenRadial.RadialCellsAround(map.AllCells.RandomElement(), missRadius, true).Where(c => c.InBounds(map)) : map.AllCells;
        }

        //TruGerman: Change to AmmoDef
        public static ArtilleryStrikeIncoming SpawnArtilleryStrikeSkyfaller(AmmoDef shellDef, Map map, IntVec3 position)
        {
            var artilleryStrikeIncoming = (ArtilleryStrikeIncoming)SkyfallerMaker.MakeSkyfaller(ThingDefOf.VFES_ArtilleryStrikeIncoming);
            artilleryStrikeIncoming.artilleryShellDef = shellDef;
            return (ArtilleryStrikeIncoming)GenSpawn.Spawn(artilleryStrikeIncoming, position, map);
        }

        public static IEnumerable<Vector3> WorldLineDrawPoints(Vector3 start, Vector3 end)
        {
            float dist = Vector3.Distance(start, end);
            float distDone = 0;

            while (distDone < dist)
            {
                var point = Vector3.Slerp(start, end, distDone / dist);
                point += point.normalized * 0.05f;
                yield return point;
                distDone = Mathf.Min(distDone + 2, dist);
            }

            yield return end + end.normalized * 0.05f;
        }

    }

}

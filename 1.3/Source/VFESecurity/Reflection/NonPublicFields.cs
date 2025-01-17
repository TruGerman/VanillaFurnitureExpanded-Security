﻿using System;
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

namespace VFESecurity
{

    [StaticConstructorOnStartup]
    public static class NonPublicFields
    {

        public static FieldInfo Building_Trap_autoRearm = AccessTools.Field(typeof(Building_Trap), "autoRearm");

        public static FieldInfo Building_TurretGun_burstCooldownTicksLeft = AccessTools.Field(typeof(Building_TurretGun), "burstCooldownTicksLeft");
        public static FieldInfo Building_TurretGun_top = AccessTools.Field(typeof(Building_TurretGun), "top");

        //TruGerman: Use CE's fields instead
        public static FieldInfo Building_TurretGun_burstCooldownTicksLeftCE = AccessTools.Field(typeof(Building_TurretGunCE), "burstCooldownTicksLeft");
        public static FieldInfo Building_TurretGun_topCE = AccessTools.Field(typeof(Building_TurretGunCE), "top");

        public static FieldInfo Explosion_cellsToAffect = AccessTools.Field(typeof(Explosion), "cellsToAffect");

        public static FieldInfo GlowGrid_litGlowers = AccessTools.Field(typeof(GlowGrid), "litGlowers");

        public static FieldInfo Projectile_launcher = AccessTools.Field(typeof(Projectile), "launcher");
        public static FieldInfo Projectile_ticksToImpact = AccessTools.Field(typeof(Projectile), "ticksToImpact");
        public static FieldInfo Projectile_origin = AccessTools.Field(typeof(Projectile), "origin");
        public static FieldInfo Projectile_usedTarget = AccessTools.Field(typeof(Projectile), "usedTarget");

        //TruGerman: More CE fields for the shield generator. Might be redundant due to your own patches
        public static FieldInfo Projectile_launcherCE = AccessTools.Field(typeof(ProjectileCE), "launcher");
        public static FieldInfo Projectile_ticksToImpactCE = AccessTools.Field(typeof(ProjectileCE), "ticksToImpact");
        public static FieldInfo Projectile_originCE = AccessTools.Field(typeof(ProjectileCE), "origin");
        public static FieldInfo Projectile_usedTargetCE = AccessTools.Field(typeof(ProjectileCE), "intendedTarget");

        public static FieldInfo Site_allEnemiesDefeatedSignalSent = AccessTools.Field(typeof(Site), "allEnemiesDefeatedSignalSent");

        public static FieldInfo TurretTop_ticksUntilIdleTurn = AccessTools.Field(typeof(TurretTop), "ticksUntilIdleTurn");

    }

}

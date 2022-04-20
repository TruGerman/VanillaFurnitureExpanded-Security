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
using HarmonyLib;

namespace VFESecurity
{

    public class ActiveArtilleryStrike : Thing
    {

        public override void ExposeData()
        {
            Scribe_Values.Look(ref missRadius, "missRadius");
            Scribe_Defs.Look(ref shellDef, "shellDef");
            Scribe_Values.Look(ref shellCount, "count");
            base.ExposeData();
        }

        public override void Tick()
        {
        }

        public float missRadius;
        //TruGerman: AmmoDef. Enough said.
        public AmmoDef shellDef;
        public int shellCount;

        //TruGerman: Another redirect to detonateProjectile
        public float Speed => shellDef.detonateProjectile.projectile.speed;

    }

}

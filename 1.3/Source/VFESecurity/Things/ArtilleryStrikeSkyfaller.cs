using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CombatExtended;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;

namespace VFESecurity
{

    public abstract class ArtilleryStrikeSkyfaller : Skyfaller
    {

        private Sustainer ambientSustainer;

        //TruGerman: Use AmmoDef instead
        protected abstract AmmoDef ShellDef
        {
            get;
        }

        public override void Tick()
        {
            // Sounds
            //TruGerman: Another reroute to detonateProjectile
            if (ambientSustainer == null && !ShellDef.detonateProjectile.projectile.soundAmbient.NullOrUndefined())
                ambientSustainer = ShellDef.detonateProjectile.projectile.soundAmbient.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
            if (ambientSustainer != null)
                ambientSustainer.Maintain();

            base.Tick();
        }

    }

}

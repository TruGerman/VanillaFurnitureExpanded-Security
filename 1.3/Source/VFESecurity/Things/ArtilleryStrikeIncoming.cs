﻿using System;
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
    public class ArtilleryStrikeIncoming : ArtilleryStrikeSkyfaller
    {

        //TruGerman: Yeah, you guessed it, more AmmoDefs
        public AmmoDef artilleryShellDef;

        protected override AmmoDef ShellDef => artilleryShellDef;

        public override Graphic Graphic
        {
            get
            {
                if (artilleryShellDef.GetModExtension<ThingDefExtension>() is ThingDefExtension thingDefExtension && thingDefExtension.incomingSkyfallerGraphicData != null)
                    return thingDefExtension.incomingSkyfallerGraphicData.Graphic;
                return base.Graphic;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            //TruGerman: Whoah, a shiny detonateProjectile redirect!
            var projectileProps = ShellDef.detonateProjectile.projectile;
            //TruGerman: I don't know if damageAmountBase is a good metric
            ShieldGeneratorUtility.CheckIntercept(this, map, projectileProps.damageAmountBase, projectileProps.damageDef, () => this.OccupiedRect().Cells,
            postIntercept: s =>
            {
                if (s.Energy > 0)
                    Destroy();
            });
        }

        public override void Tick()
        {
            // Sounds
            //TruGerman: Another redirect to detonateProjectile
            if (ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && !artilleryShellDef.detonateProjectile.projectile.soundImpactAnticipate.NullOrUndefined())
                artilleryShellDef.detonateProjectile.projectile.soundImpactAnticipate.PlayOneShot(this);

            base.Tick();
        }

        public override void HitRoof()
        {
            if (Map.roofGrid.RoofAt(Position) is RoofDef roof && roof.isThickRoof)
            {
                Impact();
                return;
            }

            base.HitRoof();
        }

        public override void Impact()
        {
            //TruGerman: Cast this to ProjectileCE instead
            var projectile = (ProjectileCE)ThingMaker.MakeThing(artilleryShellDef.detonateProjectile);
            //TruGerman: This seems to work well enough, though I feel like there are double explosions every now and then. Without it, the internal exactposition will contain NaNs and therefore ALWAYS spawn out of bounds
            GenSpawn.Spawn(projectile, Position, Map);
            projectile.Launch(this, new Vector2(Position.x, Position.z));
            projectile.ticksToImpact = 0;
            //TruGerman: Seems like the impactSomething call isn't needed
            base.Impact();
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref artilleryShellDef, "artilleryShellDef");
            base.ExposeData();
        }

    }

}

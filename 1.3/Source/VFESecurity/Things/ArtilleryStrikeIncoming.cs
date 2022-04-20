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
            ShieldGeneratorUtility.CheckIntercept(this, map, projectileProps.GetDamageAmount(1), projectileProps.damageDef, () => this.OccupiedRect().Cells,
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
            //TruGerman: With this, the projectiles only seem to impact at the bottom of the map, rarely spawning out of bounds. Without it, the internal exactposition will contain NaNs and therefore ALWAYS spawn out of bounds
            projectile.destinationInt = Position.ToVector3();
            GenSpawn.Spawn(projectile, Position, Map);
            //Redirect this to use CE's system instead
            NonPublicMethods.Projectile_ImpactSomethingCE(projectile);
            base.Impact();
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref artilleryShellDef, "artilleryShellDef");
            base.ExposeData();
        }

    }

}

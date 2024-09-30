using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SanyaCards.Monos;

// Thanks to: Boss sloth Inc.
// :)

namespace SanyaCards.Patches
{
    [HarmonyPatch(typeof(ProjectileCollision))]
    public class ProjectileCollisionPatch
    {
        [HarmonyPatch("HitSurface")]
        [HarmonyPrefix]
        public static bool hitSurface(ProjectileCollision __instance, ref ProjectileHitSurface.HasToStop __result, GameObject projectile, HitInfo hit)
        {
            if (projectile.GetComponent<ProjectileHit>().ownPlayer == __instance.GetComponentInParent<ProjectileHit>().ownPlayer && projectile.GetComponentInChildren<NoSelfCollide>())
            {
                return false;
            }
            return true;
        }
    }
}

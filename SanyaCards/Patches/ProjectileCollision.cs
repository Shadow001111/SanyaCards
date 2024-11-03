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
            ProjectileHit? bullet2 = __instance.GetComponent<ProjectileHit>();
            if (bullet2 == null)
            {
                //UnityEngine.Debug.Log($"{__instance.gameObject.name} {projectile.name}");
                return true;
            }
            ProjectileHit bullet1 = projectile.GetComponent<ProjectileHit>();

            if (bullet1.ownPlayer == bullet2.ownPlayer
                && (projectile.GetComponentInChildren<NoSelfCollide>() != null || __instance.GetComponentInChildren<NoSelfCollide>() != null))
            {
                return false;
            }
            return true;
        }
    }
}

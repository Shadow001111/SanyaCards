using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using SimulationChamber;
using UnityEngine;

namespace SanyaCards.Patches
{
    //[HarmonyPatch(typeof(RayHitBulletSound))]
    //public class RayHitBulletSoundPatch
    //{
    //    [HarmonyPatch("DoHitEffect")]
    //    [HarmonyPrefix]
    //    static bool Prefix(HitInfo hit, ref HasToReturn __result)
    //    {
    //        if (hit == null)
    //        {
    //            UnityEngine.Debug.Log("Hit is null");
    //            __result = HasToReturn.canContinue;
    //            return false;
    //        }
    //        return true;
    //    }
    //}
}

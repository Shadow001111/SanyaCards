using HarmonyLib;
using Photon.Pun;
using SanyaCards.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SanyaCards.Patches
{
    //[HarmonyDebug]
    //[HarmonyPatch(typeof(Explosion))]
    //[HarmonyPatch("DoExplosionEffects")]
    //public class ExplosionPatch
    //{
    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    //    {
    //        var codes = instructions.ToList();

    //        bool found1 = false;

    //        MethodInfo? getCharacterStatModifiersMethod = null;

    //        MethodInfo? getAdditionalDataMethod = null;


    //        try
    //        {
    //            getCharacterStatModifiersMethod = typeof(Damagable).GetMethod("GetComponent", new Type[] { typeof(Type) });
    //        }
    //        catch (Exception e)
    //        {
    //            UnityEngine.Debug.Log("EXPLOSION: Method1: " + e.Message);
    //        }
    //        try
    //        {
    //            getAdditionalDataMethod = typeof(CharacterStatModifiersExtension).GetMethod(
    //                "GetAdditionalData",
    //                BindingFlags.Static | BindingFlags.Public,
    //                null,
    //                new Type[] { typeof(CharacterStatModifiers) },
    //                null
    //            );
    //        }
    //        catch (Exception e)
    //        {
    //            UnityEngine.Debug.Log("EXPLOSION: Method2: " + e.Message);
    //        }

    //        var explosiveResistanceField = typeof(CharacterStatModifiersAdditionalData).GetField("explosionResistance", BindingFlags.Public | System.Reflection.BindingFlags.Instance);

    //        if (getCharacterStatModifiersMethod == null)
    //        {
    //            UnityEngine.Debug.Log("Method 1 is null");
    //        }
    //        if (getAdditionalDataMethod == null)
    //        {
    //            UnityEngine.Debug.Log($"Method 2 is null");
    //        }
    //        if (explosiveResistanceField == null)
    //        {
    //            UnityEngine.Debug.Log("Field 1 is null");
    //        }

    //        for (int i = 0; i < codes.Count; i++)
    //        {
    //            if (!found1)
    //            {
    //                if (codes[i].opcode != OpCodes.Brfalse)
    //                {
    //                    continue;
    //                }
    //                found1 = true;
    //            }


    //            if (codes[i].opcode == OpCodes.Callvirt)
    //            {
    //                var methodInfo = codes[i].operand as MethodInfo;

    //                if (methodInfo != null && methodInfo.Name == "GetComponentInParent")
    //                {
    //                    List<CodeInstruction> newInstructions = new List<CodeInstruction>();

    //                    // Load `componentInParent` onto the stack (assuming it is stored in V_7)
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Ldloc_S, 6));

    //                    // Check if it's not null
    //                    var op_Implicit = typeof(UnityEngine.Object).GetMethod("op_Implicit", new Type[] { typeof(UnityEngine.Object) });
    //                    if (op_Implicit == null)
    //                    {
    //                        UnityEngine.Debug.Log("OP_IMPLICIT is null");
    //                        break;
    //                    }
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Call, op_Implicit));

    //                    // Define the label here
    //                    Label statsNullLabel = generator.DefineLabel();

    //                    // Branch if the result is null
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Brfalse, statsNullLabel)); // Branch to label if null

    //                    // If not null, log that stats are not null
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Ldstr, "Stats aren't null"));
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Call, typeof(UnityEngine.Debug).GetMethod("Log", new Type[] { typeof(object) })));

    //                    // Retrieve CharacterStatModifiers
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, typeof(Damagable).GetMethod("GetComponent", new Type[] { typeof(Type) })));
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Callvirt, getCharacterStatModifiersMethod)); // Call GetComponent<CharacterStatModifiers>()

    //                    // If it was null, log that stats are null
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Ldstr, "Stats are null"));
    //                    newInstructions.Add(new CodeInstruction(OpCodes.Call, typeof(UnityEngine.Debug).GetMethod("Log", new Type[] { typeof(object) })));

    //                    // Mark the label for the jump to handle null stats
    //                    generator.MarkLabel(statsNullLabel);

    //                    // Insert the new instructions into the codes list
    //                    codes.InsertRange(i, newInstructions);
    //                    break; // Exit loop after patching
    //                }
    //            }
    //        }

    //        return codes.AsEnumerable();
    //    }
    //}

}



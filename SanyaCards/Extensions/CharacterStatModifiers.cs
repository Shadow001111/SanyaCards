using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using HarmonyLib;
using SanyaCards.Extensions;

namespace SanyaCards.Extensions
{
    //[Serializable]
    public class CharacterStatModifiersAdditionalData
    {
        public float explosionResistance;

        public CharacterStatModifiersAdditionalData()
        {
            explosionResistance = 0.0f;
        }
    }

    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData> data =
            new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersAdditionalData>();

        public static CharacterStatModifiersAdditionalData GetAdditionalData(this CharacterStatModifiers data_)
        {
            return data.GetOrCreateValue(data_);
        }

        public static void AddData(this CharacterStatModifiers data_, CharacterStatModifiersAdditionalData value)
        {
            try
            {
                data.Add(data_, value);
            }
            catch (Exception) { }
        }
    }
}

[HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
class CharacterStatModifiersPatchResetStats
{
    static void Prefix(CharacterStatModifiers __instance)
    {
        var data = __instance.GetAdditionalData();
        data.explosionResistance = 0.0f;
    }
}

﻿using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using SanyaCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils;
using ModdingUtils.MonoBehaviours;
using UnityEngine;

namespace SanyaCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.playerjumppatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.patch.regeneration", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("io.olavim.rounds.mapsextended", BepInDependency.DependencyFlags.SoftDependency)]
    
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class SanyaCards : BaseUnityPlugin
    {
        private const string ModId = "com.Shadow.SanyaCards";
        private const string ModName = "SanyaCards";
        public const string Version = "0.7.1";
        public const string ModInitials = "SANYA";

        public static SanyaCards instance { get; private set; }

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            instance = this;
            //Unbound.RegisterCredits();
            //Unbound.RegisterMenu();

            CustomCard.BuildCard<NukeCard>();
            CustomCard.BuildCard<ScoutTF2Card>();
            //CustomCard.BuildCard<SplitBulletCard>(); // BROKEN
            CustomCard.BuildCard<PrecisionStrikeCard>();
            CustomCard.BuildCard<SprayAndPrayCard>();
            CustomCard.BuildCard<AmmoBoostCard>();
            CustomCard.BuildCard<BouncesToDamageCard>();
            CustomCard.BuildCard<SandwichCard>();
            CustomCard.BuildCard<BloodySpeedCard>();
            CustomCard.BuildCard<StompCard>();
            CustomCard.BuildCard<FleeCard>();
            CustomCard.BuildCard<AccuracyCard>();
            CustomCard.BuildCard<MagneticBulletsCard>();
            CustomCard.BuildCard<ExplosionResistanceCard>();
            CustomCard.BuildCard<GlueCard>();

            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(ExplosionResistanceCard.Validation);
            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(BouncesToDamageCard.Validation);
        }

        //void NewGUI(GameObject menu)
        //{
        //    //MenuHandler.CreateSlider();
        //    //MenuHandler.CreateText();
        //}
    }
}

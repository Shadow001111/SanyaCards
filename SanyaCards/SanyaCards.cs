using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using SanyaCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils;
using ModdingUtils.MonoBehaviours;
using UnityEngine;
//using UnboundLib.Utils.UI;

// TODO: add MapsExtended dependency
namespace SanyaCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.playerjumppatch", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.willuwontu.rounds.simulationChamber", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.willuwontu.rounds.evenspreadpatch", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("root.patch.regeneration", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("io.olavim.rounds.mapsextended", BepInDependency.DependencyFlags.SoftDependency)]
    
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class SanyaCards : BaseUnityPlugin
    {
        private const string ModId = "com.Shadow.SanyaCards";
        private const string ModName = "SanyaCards";
        public const string Version = "0.6.0";
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
        }

        //void NewGUI(GameObject menu)
        //{
        //    //MenuHandler.CreateSlider();
        //    //MenuHandler.CreateText();
        //}
    }
}

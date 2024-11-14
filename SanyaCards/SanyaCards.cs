using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using SanyaCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using ModdingUtils;
using ModdingUtils.MonoBehaviours;
using UnityEngine;
using UnboundLib.GameModes;
using ModdingUtils.GameModes;
using SanyaCards.Monos;

namespace SanyaCards
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.playerjumppatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("io.olavim.rounds.mapsextended", BepInDependency.DependencyFlags.HardDependency)] // make it soft dependency and make it, so stomp can work without it
    [BepInDependency("Root.Gun.bodyRecoil.Patch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("root.patch.regeneration", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class SanyaCards : BaseUnityPlugin
    {
        private const string ModId = "com.Shadow.SanyaCards";
        private const string ModName = "SanyaCards";
        public const string Version = "0.9.0";
        public const string ModInitials = "SANYA";

        public static SanyaCards instance { get; private set; }

        void Awake()
        {
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
            CustomCard.BuildCard<KickBackCard>();
            CustomCard.BuildCard<KineticBatteryCard>();
            CustomCard.BuildCard<TeamSpiritCard>();
            CustomCard.BuildCard<RewindCard>();
            CustomCard.BuildCard<HealthTaxesCard>();

            //CustomCard.BuildCard<AcidShieldCard>(); // Bullets arent shrinking
            //CustomCard.BuildCard<SplitBulletCard>(); // BROKEN

            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(ExplosionResistanceCard.Validation);
            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(BouncesToDamageCard.Validation);
            ModdingUtils.Utils.Cards.instance.AddCardValidationFunction(TeamSpiritCard.Validation);

            GameModeManager.AddHook("PointEnd", (IGameModeHandler gm) => KineticBatteryMono.OnPointEnd());
            GameModeManager.AddHook("PointEnd", (IGameModeHandler gm) => GlueBulletMono.OnPointEnd());
        }

        //void NewGUI(GameObject menu)
        //{
        //    //MenuHandler.CreateSlider();
        //    //MenuHandler.CreateText();
        //}
    }
}

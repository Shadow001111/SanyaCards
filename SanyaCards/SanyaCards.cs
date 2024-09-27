using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using SanyaCards.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;


namespace SanyaCards
{
    // These are the mods required for our mod to work
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    // Declares our mod to Bepin
    [BepInPlugin(ModId, ModName, Version)]
    // The game our mod is associated with
    [BepInProcess("Rounds.exe")]
    public class SanyaCards : BaseUnityPlugin
    {
        private const string ModId = "com.Shadow.SanyaCards";
        private const string ModName = "SanyaCards";
        public const string Version = "0.0.0"; // What version are we on (major.minor.patch)?
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
            CustomCard.BuildCard<WildCard>();
            CustomCard.BuildCard<NukeCard>();
            CustomCard.BuildCard<Pogo2Card>();
            CustomCard.BuildCard<ScoutTF2Card>();
            CustomCard.BuildCard<SplitBulletCard>();
        }
    }
}

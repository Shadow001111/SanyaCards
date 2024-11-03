using SanyaCards.Monos;
using System.Linq;
using UnboundLib.Cards;
using UnityEngine;


namespace SanyaCards.Cards
{
    class SplitBulletCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");

            cardInfo.allowMultiple = false;
            gun.reloadTimeAdd = 0.25f;
            gun.damage = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            GameObject splitBulletObject = new GameObject("SANYA_splitBullet");
            splitBulletObject.hideFlags = HideFlags.HideAndDontSave;
            var objMono = splitBulletObject.AddComponent<SplitBulletMono2>();
            objMono.player = player;

            var objectsToSpawnList = gun.objectsToSpawn.ToList();
            objectsToSpawnList.Add
            (
                new ObjectsToSpawn
                {
                    AddToProjectile = splitBulletObject
                }
            );
            gun.objectsToSpawn = objectsToSpawnList.ToArray();
        }

        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }

        protected override string GetTitle()
        {
            return "Split bullet";
        }
        protected override string GetDescription()
        {
            return "Splits bullet into ten smaller ones after 0.5s delay\nNot in sync in mutiplayer :C";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Splitted bullet damage",
                    amount = "20% from original",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload time",
                    amount = "+0.25s",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Damage",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.TechWhite;
        }
        public override string GetModName()
        {
            return SanyaCards.ModInitials;
        }
    }
}

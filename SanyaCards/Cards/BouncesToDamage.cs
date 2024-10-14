using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;


namespace SanyaCards.Cards
{
    class BouncesToDamageCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            if (gun.reflects <= 0)
            {
                return;
            }

            int removeBouncesCount = Mathf.Min(gun.reflects, 5);
            gun.reflects -= removeBouncesCount;
            gun.damage *= (1.0f + 0.25f * removeBouncesCount);

            if (gun.reflects > 0)
            {
                return;
            }

            for (int i = 0; i < gun.objectsToSpawn.Length; i++)
            {
                ObjectsToSpawn obj = gun.objectsToSpawn[i];
                GameObject? projectile = obj.AddToProjectile;
                if (projectile == null)
                {
                    continue;
                }

                ScreenEdgeBounce? comp = projectile.GetComponent<ScreenEdgeBounce>();
                if (comp == null)
                {
                    continue;
                }

                var newProjectile = Instantiate(projectile);
                gun.objectsToSpawn[i].AddToProjectile = newProjectile;
                comp = newProjectile.GetComponent<ScreenEdgeBounce>();
                Destroy(comp);
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

        }
        protected override string GetTitle()
        {
            return "Bounces to damage";
        }
        protected override string GetDescription()
        {
            return "Get rid of 5 bounces for more damage";
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
                    positive = true,
                    stat = "Damage",
                    amount = "+25% per bounce",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bounces",
                    amount = "0",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return SanyaCards.ModInitials;
        }
    }
}

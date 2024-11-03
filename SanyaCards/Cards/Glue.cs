using SanyaCards.Monos;
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
    class GlueCard : CustomCard
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

            bool addNew = true;
            foreach (ObjectsToSpawn objToSpawn in gun.objectsToSpawn)
            {
                if (objToSpawn.AddToProjectile != null && objToSpawn.AddToProjectile.name == "SANYA_glueBullet")
                {
                    objToSpawn.AddToProjectile.transform.localScale += Vector3.right;
                    addNew = false;
                    break;
                }
            }
            if (addNew)
            {
                GameObject glueObject = new GameObject("SANYA_glueBullet");
                glueObject.hideFlags = HideFlags.HideAndDontSave;
                GlueBulletMono objMono = glueObject.AddComponent<GlueBulletMono>();

                var objectsToSpawnList = gun.objectsToSpawn.ToList();
                objectsToSpawnList.Add
                (
                    new ObjectsToSpawn
                    {
                        AddToProjectile = glueObject,
                        scaleFromDamage = 1f,
                    }
                );
                gun.objectsToSpawn = objectsToSpawnList.ToArray();
            }
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");
        }
        protected override string GetTitle()
        {
            return "Glue";
        }
        protected override string GetDescription()
        {
            return "Your shots pull enemies back to where they hit";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.MagicPink;
        }
        public override string GetModName()
        {
            return SanyaCards.ModInitials;
        }
    }
}

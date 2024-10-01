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
    class NukeCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");

            cardInfo.allowMultiple = false;

            gun.damage = 4.0f;
            gun.drag = 0.5f;
            gun.attackSpeed = 5.0f;
            gun.reloadTimeAdd = 2.0f;

            gun.explodeNearEnemyRange = 100.0f;
            gun.explodeNearEnemyDamage = 50.0f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            gun.projectileSpeed *= 0.75f;
            gun.gravity *= 0.5f;
            gunAmmo.maxAmmo = 1;

            var objectsToSpawn = ((GameObject)Resources.Load("0 cards/Explosive bullet")).GetComponent<Gun>().objectsToSpawn[0];

            var addToProjectile = Instantiate(objectsToSpawn.AddToProjectile);
            addToProjectile.hideFlags = HideFlags.HideAndDontSave;
            addToProjectile.transform.position = new Vector3(1000, 0, 0);
            addToProjectile.name = "A_SANYA_NukeBullet";

            var effect = Instantiate(objectsToSpawn.effect);
            effect.hideFlags = HideFlags.HideAndDontSave;
            effect.transform.position = new Vector3(1000, 0, 0);
            effect.name = "A_SANYA_NukeExplosion";

            Destroy(effect.GetComponent<RemoveAfterSeconds>());
            var explosion = effect.GetComponent<Explosion>();
            explosion.force = 10000;
            explosion.range = 3;

            gun.objectsToSpawn = new[]
            {
                new ObjectsToSpawn
                {
                    AddToProjectile = addToProjectile,
                    effect = effect,
                    normalOffset = 0.1f,
                    numberOfSpawns = 1,
                    scaleFromDamage = 1.0f,
                    scaleStackM = 0.7f,
                    scaleStacks = true,
                }
            };
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

        }
        protected override string GetTitle()
        {
            return "Nuke";
        }
        protected override string GetDescription()
        {
            return "Friendly pellets";
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
                    amount = "+300%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Drag",
                    amount = "+50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Bullet speed",
                    amount = "-25%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Gravity",
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Ammo",
                    amount = "1",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Attack speed",
                    amount = "-400%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Reload time",
                    amount = "+2s",
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

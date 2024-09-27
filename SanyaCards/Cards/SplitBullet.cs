using ModdingUtils.RoundsEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using Photon.Pun;


namespace SanyaCards.Cards
{
    class SplitBulletMono : MonoBehaviour
    {
        static public readonly int bulletsAfterSplitCount = 3;
        static public readonly float splitAngle = 90;
        static public readonly float splitDelay = 1;

        private float splitTime;

        GameObject parent;

        private void Start()
        {
            parent = transform.parent.gameObject;
            splitTime = Time.time + splitDelay;
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(this);
        }

        private void Update()
        {
            if (Time.time >= splitTime)
            {
                var parentMoveTransform = parent.GetComponent<MoveTransform>();
                Vector2 parentVelocity = parentMoveTransform.velocity;
                float startAngle = -splitAngle * 0.5f;
                float angleBetweenBullets = splitAngle / (bulletsAfterSplitCount - 1);

                for (int i = 0; i < bulletsAfterSplitCount; i++)
                {
                    // Calculate the angle for each new bullet
                    float currentAngle = startAngle + i * angleBetweenBullets;

                    // Instantiate the new bullet
                    GameObject newBullet = Instantiate(parent, parent.transform.position, parent.transform.rotation);

                    // Set the velocity of the new bullet
                    // newBullet.GetComponent<MoveTransform>().velocity = Quaternion.Euler(0, 0, currentAngle) * parentVelocity;
                    Vector2 rdDpos = UnityEngine.Random.insideUnitCircle;
                    newBullet.transform.position = parent.transform.position + new Vector3(rdDpos.x, 0, rdDpos.y);
                }

                UnityEngine.Object.Destroy(parent);
            }
        }
    }

    class SplitBulletCard : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");

            cardInfo.allowMultiple = false;

            var obj = new GameObject("splitBullet");
            obj.AddComponent<SplitBulletMono>();

            gun.objectsToSpawn = new[]
            {
                new ObjectsToSpawn
                {
                    AddToProjectile = obj
                }
            };
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

        }
        protected override string GetTitle()
        {
            return "SplitBullet";
        }
        protected override string GetDescription()
        {
            return "Splits bullet";
        }
        protected override GameObject GetCardArt()
        {
            return null;
        }
        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Common;
        }
        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[]
            {
                new CardInfoStat()
                {
                    positive = true,
                    stat = "Effect",
                    amount = "No",
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

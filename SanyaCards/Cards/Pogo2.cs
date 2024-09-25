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
    class Pogo2Mono : MonoBehaviour
    {
        private readonly float jumpDelay = 1.0f;
        private readonly float jumpSpeed = 3.0f;
        private float nextJumpTime = 0.0f;
        private Player player;

        private void Awake()
        {
            this.player = base.GetComponent<Player>();
        }

        private void Start()
        {
            nextJumpTime = Time.time + jumpDelay;
        }

        public void Destroy()
        {
            Destroy(this);
        }

        private void Update()
        {
            if (Time.time < nextJumpTime)
            {
                return;
            }
            nextJumpTime = Time.time + jumpDelay;
            player.data.jump.Jump(true, jumpSpeed);
        }
    }

    class Pogo2Card : CustomCard
    {
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");

            cardInfo.allowMultiple = false;

            block.cdMultiplier = 0.1f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            player.gameObject.AddComponent<Pogo2Mono>();

            data.maxHealth *= 0.5f;
        }
        public override void OnRemoveCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Run when the card is removed from the player
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been removed from player {player.playerID}.");

            Pogo2Mono pogo2Mono = player.gameObject.GetComponent<Pogo2Mono>();
            if (pogo2Mono != null)
            {
                UnityEngine.Object.Destroy(pogo2Mono);
            }
        }
        protected override string GetTitle()
        {
            return "Pogo 2.0";
        }
        protected override string GetDescription()
        {
            return "Make you jump on dick";
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
                    stat = "BlockCooldown",
                    amount = "10%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                },
                new CardInfoStat()
                {
                    positive = false,
                    stat = "Health",
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.aLotOf
                }
            };
        }
        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.EvilPurple;
        }
        public override string GetModName()
        {
            return SanyaCards.ModInitials;
        }
    }
}

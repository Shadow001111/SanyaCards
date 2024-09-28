using ModdingUtils.RoundsEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;
using SimulationChamber;
using Photon.Pun;
using System.Collections;


namespace SanyaCards.Cards
{
    class SplitBulletMono : MonoBehaviour
    {
        static public readonly int bulletsAfterSplitCount = 10;
        static public readonly float splitAngle = 90;
        static public readonly float splitDelay = 0.5f;

        private float TEMPTIME = Time.time;

        private float splitTime;

        private GameObject parent;
        public Player player;
        private Gun gun;
        private SimulatedGun simulatedGun;

        public void Start()
        {
            parent = transform.parent.gameObject;
            splitTime = Time.time + splitDelay;

            this.gun = player.data.weaponHandler.gun;
            Gun gun = this.gun;
            simulatedGun = new GameObject("SANYA_simulatedGun").AddComponent<SimulatedGun>();
            simulatedGun.CopyGunStatsExceptActions(gun);
            simulatedGun.CopyAttackAction(gun);
            simulatedGun.CopyShootProjectileAction(gun);

            simulatedGun.numberOfProjectiles = bulletsAfterSplitCount;
            simulatedGun.bursts = 0;
            simulatedGun.timeBetweenBullets = 0.0f;

            simulatedGun.spread = 2.0f;
            simulatedGun.evenSpread = 1.0f;
            simulatedGun.damage = gun.damage / (float)bulletsAfterSplitCount;
            simulatedGun.projectileSpeed = 1.0f;
            simulatedGun.reflects = 0;

            simulatedGun.objectsToSpawn = simulatedGun.objectsToSpawn
            .Where(go => go != null && go.AddToProjectile.GetComponent<SplitBulletMono>() == null)
            .ToArray();
        }

        public void OnDestroy()
        {
            Destroy(simulatedGun);
            //StartCoroutine(DestroyGun());
        }

        private void Update()
        {
            if (Time.time >= splitTime)
            {
                bool flag2 = !player.data.view.IsMine && !PhotonNetwork.OfflineMode;
                var parentMoveTransform = parent.GetComponent<MoveTransform>();
                Vector2 parentVelocity = parentMoveTransform.velocity;
                if (!flag2)
                {
                    simulatedGun.SimulatedAttack(player.playerID, parent.transform.position, parentVelocity.normalized, 1.0f, 1.0f);
                }
                UnityEngine.Object.Destroy(parent);
            }
        }

        private IEnumerator DestroyGun()
        {
            yield return new WaitForSeconds(simulatedGun.timeBetweenBullets * simulatedGun.bursts);
            Destroy(simulatedGun);
        }
    }

    class SplitBulletCard : CustomCard
    {
        private static GameObject splitBulletObject;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
        {
            //Edits values on card itself, which are then applied to the player in `ApplyCardStats`
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been setup.");

            cardInfo.allowMultiple = false;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            if (splitBulletObject == null)
            {
                splitBulletObject = new GameObject("SANYA_splitBullet");
                var objMono = splitBulletObject.AddComponent<SplitBulletMono>();
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

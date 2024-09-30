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
using SanyaCards.Monos;


namespace SanyaCards.Cards
{
    class SplitBulletMono : MonoBehaviour
    {
        static public readonly int bulletsAfterSplitCount = 10;
        static public readonly float splitAngle = 90;
        static public readonly float splitDelay = 0.5f;

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

            simulatedGun.numberOfProjectiles = bulletsAfterSplitCount / 2; // we shoot twice
            simulatedGun.bursts = 1;
            simulatedGun.timeBetweenBullets = 0.0f;

            simulatedGun.spread = 1.0f;
            simulatedGun.evenSpread = 1.0f;
            simulatedGun.damage = gun.damage / bulletsAfterSplitCount * 2.0f;
            simulatedGun.damageAfterDistanceMultiplier = 1.0f;
            simulatedGun.projectileSpeed = 0.5f;
            simulatedGun.reflects = 0;
            simulatedGun.destroyBulletAfter = 0.0f;
            simulatedGun.gravity = 1.0f;

            // Doesnt work with: Dazzle, ToxicClouds

            var newObjectsToSpawn = new List<ObjectsToSpawn>();
            foreach (var oldObjectsToSpawn in gun.objectsToSpawn)
            {
                if (oldObjectsToSpawn.AddToProjectile.name == "A_ScreenEdge")
                {
                    continue;
                }

                GameObject newAddToProjectile;
                if (oldObjectsToSpawn.AddToProjectile.GetComponent<SplitBulletMono>() != null)
                {
                    newAddToProjectile = Instantiate(oldObjectsToSpawn.AddToProjectile);

                    SplitBulletMono splitBulletComp = newAddToProjectile.GetComponent<SplitBulletMono>();
                    Destroy(splitBulletComp);
                    newAddToProjectile.AddComponent<NoSelfCollide>();
                }
                else
                {
                    newAddToProjectile = oldObjectsToSpawn.AddToProjectile;
                }

                newObjectsToSpawn.Add(new ObjectsToSpawn
                {
                    effect = oldObjectsToSpawn.effect,
                    direction = oldObjectsToSpawn.direction,
                    spawnOn = oldObjectsToSpawn.spawnOn,
                    spawnAsChild = oldObjectsToSpawn.spawnAsChild,
                    numberOfSpawns = oldObjectsToSpawn.numberOfSpawns,
                    normalOffset = oldObjectsToSpawn.normalOffset,
                    stickToBigTargets = oldObjectsToSpawn.stickToBigTargets,
                    stickToAllTargets = oldObjectsToSpawn.stickToAllTargets,
                    zeroZ = oldObjectsToSpawn.zeroZ,
                    AddToProjectile = newAddToProjectile
                });
            }
            simulatedGun.objectsToSpawn = newObjectsToSpawn.ToArray();
        }

        public void OnDestroy()
        {
            Destroy(simulatedGun.gameObject);
        }

        private void Update()
        {
            if (Time.time >= splitTime)
            {
                if (player.data.view.IsMine || PhotonNetwork.OfflineMode)
                {
                    var parentMoveTransform = parent.GetComponent<MoveTransform>();
                    Vector2 shootDirection = parentMoveTransform.velocity.normalized;
                    if (shootDirection == Vector2.zero)
                    {
                        shootDirection = Vector2.right;
                    }
                    simulatedGun.SimulatedAttack(player.playerID, parent.transform.position, shootDirection, 1.0f, 1.0f);
                    simulatedGun.SimulatedAttack(player.playerID, parent.transform.position, -shootDirection, 1.0f, 1.0f);
                }
                Destroy(parent);
            }
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
            gun.reloadTimeAdd = 0.25f;
            gun.damage = 0.75f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            //Edits values on player when card is selected
            UnityEngine.Debug.Log($"[{SanyaCards.ModInitials}][Card] {GetTitle()} has been added to player {player.playerID}.");

            if (splitBulletObject == null)
            {
                splitBulletObject = new GameObject("A_SANYA_splitBullet");
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
            return "Splits bullet into ten smaller ones after 0.5s delay\n Does not work with Dazzle and ToxicClouds :C";
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
                    positive = false,
                    stat = "Splitted bullet damage",
                    amount = "20%",
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
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }
        public override string GetModName()
        {
            return SanyaCards.ModInitials;
        }
    }
}

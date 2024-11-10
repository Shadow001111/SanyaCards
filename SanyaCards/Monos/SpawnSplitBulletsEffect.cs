using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using ModdingUtils.Extensions;
using Photon.Pun;
using UnityEngine;
using System;

// Thanks to Pykess

namespace SanyaCards.Monos
{
    // Token: 0x02000032 RID: 50
    public class SpawnSplitBulletsEffect : MonoBehaviour
    {
        // Token: 0x060000F2 RID: 242 RVA: 0x000070D0 File Offset: 0x000052D0
        private void Awake()
        {
            this.player = base.gameObject.GetComponent<Player>();

            foreach (var obj in this.gunToShootFrom.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide Awake");
                        break;
                    }
                }
            }
        }

        // Token: 0x060000F3 RID: 243 RVA: 0x000070E3 File Offset: 0x000052E3
        private void Start()
        {
            foreach (var obj in this.gunToShootFrom.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide Start");
                        break;
                    }
                }
            }
        }

        void Update()
        {
            foreach (var obj in this.gunToShootFrom.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide Update");
                        break;
                    }
                }
            }
            this.Shoot();
            Destroy(this);
        }

        // Token: 0x060000F5 RID: 245 RVA: 0x0000713D File Offset: 0x0000533D
        private void OnDisable()
        {
            Destroy(this);
        }

        // Token: 0x060000F6 RID: 246 RVA: 0x00007145 File Offset: 0x00005345
        private void OnDestroy()
        {
            Destroy(this.newWeaponsBase);
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x00007154 File Offset: 0x00005354
        private void Shoot()
        {
            int num = this.gunToShootFrom.numberOfProjectiles;
            for (int i = 0; i < this.gunToShootFrom.projectiles.Length; i++)
            {
                for (int j = 0; j < this.gunToShootFrom.numberOfProjectiles; j++)
                {
                    Vector3 vector = this.directionsToShoot[this.numShot % this.directionsToShoot.Count];
                    if (this.gunToShootFrom.spread != 0f)
                    {
                        float multiplySpread = this.gunToShootFrom.multiplySpread;
                        float num2 = UnityEngine.Random.Range(-this.gunToShootFrom.spread, this.gunToShootFrom.spread);
                        num2 /= (1f + this.gunToShootFrom.projectileSpeed * 0.5f) * 0.5f;
                        vector += Vector3.Cross(vector, Vector3.forward) * num2 * multiplySpread;
                    }
                    if ((bool)typeof(Gun).InvokeMember("CheckIsMine", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, this.gunToShootFrom, new object[0]))
                    {
                        GameObject gameObject = PhotonNetwork.Instantiate(this.gunToShootFrom.projectiles[i].objectToSpawn.gameObject.name, positionToShootFrom, Quaternion.LookRotation(vector), 0, null);

                        if (PhotonNetwork.OfflineMode)
                        {
                            this.RPCA_Shoot(gameObject.GetComponent<PhotonView>().ViewID, num, 1f, UnityEngine.Random.Range(0f, 1f));
                        }
                        else
                        {
                            base.gameObject.GetComponent<PhotonView>().RPC("RPCA_Shoot", RpcTarget.All, new object[]
                            {
                                gameObject.GetComponent<PhotonView>().ViewID,
                                num,
                                1f,
                                UnityEngine.Random.Range(0f, 1f)
                            });
                        }
                    }
                }
            }
            this.ResetTimer();
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x0000739C File Offset: 0x0000559C
        [PunRPC]
        private void RPCA_Shoot(int bulletViewID, int numProj, float dmgM, float seed)
        {
            GameObject gameObject = PhotonView.Find(bulletViewID).gameObject;

            foreach (var obj in this.gunToShootFrom.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide 5");
                        break;
                    }
                }
            }

            this.gunToShootFrom.BulletInit(gameObject, numProj, dmgM, seed, true);
            this.numShot++;

            if (gameObject.GetComponentInChildren<NoSelfCollide>())
            {
                UnityEngine.Debug.Log("Found NoSelfCollide 6");
            }
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x000073D4 File Offset: 0x000055D4
        public void SetGun(Gun gun)
        {
            this.newWeaponsBase = Instantiate<GameObject>(this.player.GetComponent<Holding>().holdable.GetComponent<Gun>().gameObject, new Vector3(500f, 500f, -100f), Quaternion.identity);
            DontDestroyOnLoad(this.newWeaponsBase);
            foreach (object obj in this.newWeaponsBase.transform)
            {
                Transform transform = (Transform)obj;
                if (transform.GetComponentInChildren<Renderer>() != null)
                {
                    Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        componentsInChildren[i].enabled = false;
                    }
                }
            }
            this.gunToShootFrom = this.newWeaponsBase.GetComponent<Gun>();
            SpawnSplitBulletsEffect.CopyGunStats(gun, this.gunToShootFrom);

            foreach (var obj in this.gunToShootFrom.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide 4");
                        break;
                    }
                }
            }
        }

        // Token: 0x060000FA RID: 250 RVA: 0x000074C0 File Offset: 0x000056C0
        public void SetNumBullets(int num)
        {
            this.numBullets = num;
        }

        // Token: 0x060000FC RID: 252 RVA: 0x000074DD File Offset: 0x000056DD
        public void SetDirection(Vector3 dir)
        {
            this.directionsToShoot = new List<Vector3>
            {
                dir
            };
        }

        // Token: 0x060000FD RID: 253 RVA: 0x000074F1 File Offset: 0x000056F1
        public void SetPosition(Vector3 pos)
        {
            positionToShootFrom = pos;
        }

        // Token: 0x060000FE RID: 254 RVA: 0x000074FA File Offset: 0x000056FA
        public void SetDirections(List<Vector3> dir)
        {
            this.directionsToShoot = dir;
        }

        // Token: 0x060000FF RID: 255 RVA: 0x00007503 File Offset: 0x00005703
        public void SetTimeBetweenShots(float delay)
        {
            this.timeBetweenShots = delay;
        }

        // Token: 0x06000100 RID: 256 RVA: 0x0000750C File Offset: 0x0000570C
        public void SetInitialDelay(float delay)
        {
            this.initialDelay = delay;
        }

        // Token: 0x06000101 RID: 257 RVA: 0x00007515 File Offset: 0x00005715
        private void ResetTimer()
        {
            this.timeSinceLastShot = Time.time;
        }

        // Token: 0x06000102 RID: 258 RVA: 0x00007524 File Offset: 0x00005724
        public static void CopyGunStats(Gun copyFromGun, Gun copyToGun)
        {
            copyToGun.ammo = copyFromGun.ammo;
            copyToGun.ammoReg = copyFromGun.ammoReg;
            copyToGun.attackID = copyFromGun.attackID;
            copyToGun.attackSpeed = copyFromGun.attackSpeed;
            copyToGun.attackSpeedMultiplier = copyFromGun.attackSpeedMultiplier;
            copyToGun.bodyRecoil = copyFromGun.bodyRecoil;
            copyToGun.bulletDamageMultiplier = copyFromGun.bulletDamageMultiplier;
            copyToGun.bulletPortal = copyFromGun.bulletPortal;
            copyToGun.bursts = copyFromGun.bursts;
            copyToGun.chargeDamageMultiplier = copyFromGun.chargeDamageMultiplier;
            copyToGun.chargeEvenSpreadTo = copyFromGun.chargeEvenSpreadTo;
            copyToGun.chargeNumberOfProjectilesTo = copyFromGun.chargeNumberOfProjectilesTo;
            copyToGun.chargeRecoilTo = copyFromGun.chargeRecoilTo;
            copyToGun.chargeSpeedTo = copyFromGun.chargeSpeedTo;
            copyToGun.chargeSpreadTo = copyFromGun.chargeSpreadTo;
            copyToGun.cos = copyFromGun.cos;
            copyToGun.currentCharge = copyFromGun.currentCharge;
            copyToGun.damage = copyFromGun.damage;
            copyToGun.damageAfterDistanceMultiplier = copyFromGun.damageAfterDistanceMultiplier;
            copyToGun.defaultCooldown = copyFromGun.defaultCooldown;
            copyToGun.dmgMOnBounce = copyFromGun.dmgMOnBounce;
            copyToGun.dontAllowAutoFire = copyFromGun.dontAllowAutoFire;
            copyToGun.drag = copyFromGun.drag;
            copyToGun.dragMinSpeed = copyFromGun.dragMinSpeed;
            copyToGun.evenSpread = copyFromGun.evenSpread;
            copyToGun.explodeNearEnemyDamage = copyFromGun.explodeNearEnemyDamage;
            copyToGun.explodeNearEnemyRange = copyFromGun.explodeNearEnemyRange;
            copyToGun.forceSpecificAttackSpeed = copyFromGun.forceSpecificAttackSpeed;
            copyToGun.forceSpecificShake = copyFromGun.forceSpecificShake;
            copyToGun.gravity = copyFromGun.gravity;
            copyToGun.hitMovementMultiplier = copyFromGun.hitMovementMultiplier;
            copyToGun.ignoreWalls = copyFromGun.ignoreWalls;
            copyToGun.isProjectileGun = copyFromGun.isProjectileGun;
            copyToGun.isReloading = copyFromGun.isReloading;
            copyToGun.knockback = copyFromGun.knockback;
            copyToGun.lockGunToDefault = copyFromGun.lockGunToDefault;
            copyToGun.multiplySpread = copyFromGun.multiplySpread;
            copyToGun.numberOfProjectiles = copyFromGun.numberOfProjectiles;
            copyToGun.objectsToSpawn = copyFromGun.objectsToSpawn;
            copyToGun.overheatMultiplier = copyFromGun.overheatMultiplier;
            copyToGun.percentageDamage = copyFromGun.percentageDamage;
            copyToGun.player = copyFromGun.player;
            copyToGun.projectielSimulatonSpeed = copyFromGun.projectielSimulatonSpeed;
            copyToGun.projectileColor = copyFromGun.projectileColor;
            copyToGun.projectiles = copyFromGun.projectiles;
            copyToGun.projectileSize = copyFromGun.projectileSize;
            copyToGun.projectileSpeed = copyFromGun.projectileSpeed;
            copyToGun.randomBounces = copyFromGun.randomBounces;
            copyToGun.recoil = copyFromGun.recoil;
            copyToGun.recoilMuiltiplier = copyFromGun.recoilMuiltiplier;
            copyToGun.reflects = copyFromGun.reflects;
            copyToGun.reloadTime = copyFromGun.reloadTime;
            copyToGun.reloadTimeAdd = copyFromGun.reloadTimeAdd;
            copyToGun.shake = copyFromGun.shake;
            copyToGun.shakeM = copyFromGun.shakeM;
            copyToGun.ShootPojectileAction = copyFromGun.ShootPojectileAction;
            copyToGun.sinceAttack = copyFromGun.sinceAttack;
            copyToGun.size = copyFromGun.size;
            copyToGun.slow = copyFromGun.slow;
            copyToGun.smartBounce = copyFromGun.smartBounce;
            copyToGun.spawnSkelletonSquare = copyFromGun.spawnSkelletonSquare;
            copyToGun.speedMOnBounce = copyFromGun.speedMOnBounce;
            copyToGun.spread = copyFromGun.spread;
            copyToGun.teleport = copyFromGun.teleport;
            copyToGun.timeBetweenBullets = copyFromGun.timeBetweenBullets;
            copyToGun.timeToReachFullMovementMultiplier = copyFromGun.timeToReachFullMovementMultiplier;
            copyToGun.unblockable = copyFromGun.unblockable;
            copyToGun.useCharge = copyFromGun.useCharge;
            copyToGun.waveMovement = copyFromGun.waveMovement;
            Traverse.Create(copyToGun).Field("attackAction").SetValue((Action)Traverse.Create(copyFromGun).Field("attackAction").GetValue());
            Traverse.Create(copyToGun).Field("gunID").SetValue((int)Traverse.Create(copyFromGun).Field("gunID").GetValue());
            Traverse.Create(copyToGun).Field("spreadOfLastBullet").SetValue((float)Traverse.Create(copyFromGun).Field("spreadOfLastBullet").GetValue());
            Traverse.Create(copyToGun).Field("forceShootDir").SetValue((Vector3)Traverse.Create(copyFromGun).Field("forceShootDir").GetValue());

            foreach (var obj in copyFromGun.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide 2");
                        break;
                    }
                }
            }

            foreach (var obj in copyToGun.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide 3");
                        break;
                    }
                }
            }
        }

        // Token: 0x06000103 RID: 259 RVA: 0x00007952 File Offset: 0x00005B52
        public void Destroy()
        {
            Destroy(this);
        }

        // Token: 0x04000099 RID: 153
        private float initialDelay = 1f;

        // Token: 0x0400009A RID: 154
        private int numBullets = 1;

        // Token: 0x0400009B RID: 155
        private int numShot;

        // Token: 0x0400009C RID: 156
        public Gun gunToShootFrom;

        // Token: 0x0400009D RID: 157
        private List<Vector3> directionsToShoot = new List<Vector3>();

        // Token: 0x0400009E RID: 158
        private Vector3 positionToShootFrom;

        // Token: 0x0400009F RID: 159
        private float timeBetweenShots;

        // Token: 0x040000A0 RID: 160
        private float timeSinceLastShot;

        // Token: 0x040000A1 RID: 161
        private GameObject newWeaponsBase;

        // Token: 0x040000A2 RID: 162
        private Player player;
    }
}

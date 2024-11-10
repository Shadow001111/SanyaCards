using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SanyaCards.Monos
{
    class SplitBulletMono2 : MonoBehaviour
    {
        static public readonly int bulletsAfterSplitCount = 10;
        static public readonly float splitDelay = 0.5f;

        float splitTime;

        public Player player;
        List<GameObject>? newInstacesOfAddToProjectile = null;

        void Start()
        {
            if (transform.parent == null)
            {
                return;
            }
            splitTime = Time.time + splitDelay;
        }

        void OnDestroy()
        {
            if (newInstacesOfAddToProjectile != null)
            {
                foreach (GameObject addOnProjectile in newInstacesOfAddToProjectile)
                {
                    Destroy(addOnProjectile);
                }
                newInstacesOfAddToProjectile.Clear();
            }
        }

        void Update()
        {
            if (transform.parent == null)
            {
                return;
            }
            else if (Time.time >= splitTime)
            {
                Shoot();
                Destroy(transform.parent.gameObject);
            }
        }

        void Shoot()
        {
            Gun gun = player.data.weaponHandler.gun;

            Gun? newGun = player.GetComponent<SplitBulletGun>();
            if (newGun == null)
            {
                newGun = player.gameObject.AddComponent<SplitBulletGun>();
            }

            SpawnSplitBulletsEffect? spawnBulletsEffect = player.GetComponent<SpawnSplitBulletsEffect>();
            if (spawnBulletsEffect == null)
            {
                spawnBulletsEffect = player.gameObject.AddComponent<SpawnSplitBulletsEffect>();
            }

            UnityEngine.Debug.Log("\nSHOOT:");

            SpawnSplitBulletsEffect.CopyGunStats(gun, newGun);

            var newObjectsToSpawn = new List<ObjectsToSpawn>();
            newInstacesOfAddToProjectile = new List<GameObject>();
            foreach (var oldObjectsToSpawn in gun.objectsToSpawn)
            {
                GameObject? addToProjectile = oldObjectsToSpawn.AddToProjectile;
                if (addToProjectile != null)
                {
                    bool addToProjectileChanged = false;
                    if (addToProjectile.GetComponent<SplitBulletMono2>() != null)
                    {
                        addToProjectile = Instantiate(addToProjectile);
                        newInstacesOfAddToProjectile.Add(addToProjectile);
                        addToProjectileChanged = true;

                        Destroy(addToProjectile.GetComponent<SplitBulletMono2>());
                        addToProjectile.AddComponent<NoSelfCollide>();
                    }

                    if (addToProjectile.GetComponent<ScreenEdgeBounce>() != null)
                    {
                        if (!addToProjectileChanged)
                        {
                            addToProjectile = Instantiate(addToProjectile);
                            newInstacesOfAddToProjectile.Add(addToProjectile);
                            addToProjectileChanged = true;
                        }
                        Destroy(addToProjectile.GetComponent<ScreenEdgeBounce>());
                    }
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
                    AddToProjectile = addToProjectile,
                    removeScriptsFromProjectileObject = false,
                    scaleStacks = oldObjectsToSpawn.scaleStacks,
                    scaleStackM = oldObjectsToSpawn.scaleStackM,
                    scaleFromDamage = oldObjectsToSpawn.scaleFromDamage,
                    stacks = oldObjectsToSpawn.stacks
                });
            }
            newGun.objectsToSpawn = newObjectsToSpawn.ToArray();

            newGun.numberOfProjectiles = bulletsAfterSplitCount;
            newGun.bursts = 1;
            newGun.timeBetweenBullets = 0.0f;

            ProjectileHit projectileHitComponent = transform.parent.GetComponent<ProjectileHit>();

            newGun.spread = 0.0f;
            newGun.evenSpread = 0.0f;
            newGun.damage = projectileHitComponent.damage / 55.0f / bulletsAfterSplitCount * 2.0f;
            newGun.bulletDamageMultiplier = projectileHitComponent.dealDamageMultiplierr;
            newGun.damageAfterDistanceMultiplier = 1.5f;
            newGun.projectileSpeed = 0.5f;
            newGun.projectielSimulatonSpeed = 1.0f;
            newGun.reflects = 0;
            newGun.destroyBulletAfter = 20.0f;
            newGun.gravity = 1.0f;
            newGun.drag = 0.0f;

            newGun.chargeDamageMultiplier = 1.0f;
            newGun.chargeEvenSpreadTo = 0.0f;
            newGun.chargeNumberOfProjectilesTo = 0;
            newGun.chargeRecoilTo = 0.0f;
            newGun.chargeSpeedTo = 0.0f;
            newGun.chargeSpreadTo = 0.0f;

            List<Vector3> directions = GetDirections();

            spawnBulletsEffect.SetPosition(transform.position);
            spawnBulletsEffect.SetDirections(directions);

            //newGun.projectiles = (from e in Enumerable.Range(0, newGun.numberOfProjectiles)
            //                      from x in newGun.projectiles
            //                      select x).ToList<ProjectilesToSpawn>().Take(newGun.numberOfProjectiles).ToArray<ProjectilesToSpawn>();

            foreach (var obj in newGun.objectsToSpawn)
            {
                if (obj.AddToProjectile != null)
                {
                    if (obj.AddToProjectile.GetComponent<NoSelfCollide>())
                    {
                        UnityEngine.Debug.Log("Found NoSelfCollide 1");
                        break;
                    }
                }
            }

            spawnBulletsEffect.SetGun(newGun);
        }

        List<Vector3> GetDirections()
        {
            List<Vector3> list = new List<Vector3>();
            float dangle = (360f * Mathf.Deg2Rad) / bulletsAfterSplitCount;
            for (int i = 0; i < bulletsAfterSplitCount; i++)
            {
                float angle = i * dangle;
                float x = Mathf.Cos(angle);
                float y = Mathf.Sin(angle);

                Vector3 direction = new Vector3(x, y, 0f);
                list.Add(direction);
            }
            return list;
        }
    }
}

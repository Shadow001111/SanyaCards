using Photon.Pun;
using SanyaCards.Monos;
using SimulationChamber;
using System.Collections.Generic;
using UnityEngine;

class SplitBulletMono : MonoBehaviour
{
    static public readonly int bulletsAfterSplitCount = 10;
    static public readonly float splitDelay = 0.5f;

    private float splitTime;

    private GameObject? parent = null;
    public Player player;
    private Gun? gun;
    private SimulatedGun? simulatedGun;
    private List<GameObject>? newInstacesOfAddToProjectile;

    public void Start()
    {
        if (transform.parent == null)
        {
            UnityEngine.Debug.Log("Parent is null");
            return;
        }
        parent = transform.parent.gameObject;
        splitTime = Time.time + splitDelay;

        this.gun = player.data.weaponHandler.gun;
        Gun gun = this.gun;
        simulatedGun = parent.AddComponent<SimulatedGun>();

        simulatedGun.CopyGunStatsExceptActions(gun);
        simulatedGun.CopyAttackAction(gun);
        simulatedGun.CopyShootProjectileAction(gun);

        simulatedGun.numberOfProjectiles = bulletsAfterSplitCount / 2; // we shoot twice
        simulatedGun.bursts = 1;
        simulatedGun.timeBetweenBullets = 0.0f;

        ProjectileHit projectileHitComponent = parent.GetComponent<ProjectileHit>();

        simulatedGun.spread = 1.0f;
        simulatedGun.evenSpread = 1.0f;
        simulatedGun.damage = projectileHitComponent.damage / 55.0f / bulletsAfterSplitCount * 2.0f;
        simulatedGun.bulletDamageMultiplier = projectileHitComponent.dealDamageMultiplierr;
        simulatedGun.damageAfterDistanceMultiplier = 1.0f;
        simulatedGun.projectileSpeed = 0.5f;
        simulatedGun.projectielSimulatonSpeed = 1.0f;
        simulatedGun.reflects = 0;
        simulatedGun.destroyBulletAfter = 20.0f;
        simulatedGun.gravity = 1.0f;

        var newObjectsToSpawn = new List<ObjectsToSpawn>();
        newInstacesOfAddToProjectile = new List<GameObject>();
        foreach (var oldObjectsToSpawn in gun.objectsToSpawn)
        {
            GameObject? addToProjectile = oldObjectsToSpawn.AddToProjectile;
            bool addToProjectileChanged = false;
            if (addToProjectile != null)
            {
                if (addToProjectile.GetComponent<SplitBulletMono>() != null)
                {
                    addToProjectile = Instantiate(addToProjectile);
                    newInstacesOfAddToProjectile.Add(addToProjectile);
                    addToProjectileChanged = true;

                    Destroy(addToProjectile.GetComponent<SplitBulletMono>());
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
                AddToProjectile = addToProjectile
            });
        }
        simulatedGun.objectsToSpawn = newObjectsToSpawn.ToArray();
    }

    public void OnDestroy()
    {
        if (parent != null)
        {
            UnityEngine.Debug.Log("Cleanup: " + newInstacesOfAddToProjectile.Count);
            foreach (GameObject addOnProjectile in newInstacesOfAddToProjectile)
            {
                Destroy(addOnProjectile);
            }
            newInstacesOfAddToProjectile.Clear();

        }
    }

    private void Update()
    {
        if (parent == null)
        {
            return;
        }
        else if (Time.time >= splitTime)
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
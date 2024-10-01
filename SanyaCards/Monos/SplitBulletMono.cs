using Photon.Pun;
using SanyaCards.Monos;
using SimulationChamber;
using System.Collections.Generic;
using UnityEngine;

class SplitBulletMono : MonoBehaviour
{
    static public readonly int bulletsAfterSplitCount = 10;
    static public readonly float splitAngle = 90;
    static public readonly float splitDelay = 0.5f;

    private float splitTime;

    private GameObject? parent = null;
    public Player player;
    private Gun gun;
    private SimulatedGun simulatedGun;

    public void Start()
    {
        if (transform.parent == null)
        {
            return;
        }
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
        simulatedGun.destroyBulletAfter = 20.0f;
        simulatedGun.gravity = 1.0f;

        var newObjectsToSpawn = new List<ObjectsToSpawn>();
        foreach (var oldObjectsToSpawn in gun.objectsToSpawn)
        {
            var oldAddToProjectile = oldObjectsToSpawn.AddToProjectile;
            if (oldAddToProjectile != null && oldAddToProjectile.name == "A_ScreenEdge")
            {
                continue;
            }

            GameObject? newAddToProjectile = oldAddToProjectile;
            if (oldAddToProjectile != null)
            {
                if (oldAddToProjectile.GetComponent<SplitBulletMono>() != null)
                {
                    newAddToProjectile = Instantiate(oldAddToProjectile);

                    SplitBulletMono splitBulletComp = newAddToProjectile.GetComponent<SplitBulletMono>();
                    Destroy(splitBulletComp);
                    newAddToProjectile.AddComponent<NoSelfCollide>();
                }
                else
                {
                    newAddToProjectile = oldAddToProjectile;
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
                AddToProjectile = newAddToProjectile
            });
        }
        simulatedGun.objectsToSpawn = newObjectsToSpawn.ToArray();
    }

    public void OnDestroy()
    {
        if (simulatedGun != null)
        {
            Destroy(simulatedGun.gameObject);
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
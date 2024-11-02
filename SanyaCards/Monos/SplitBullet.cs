using Photon.Pun;
using SanyaCards.Monos;
using SimulationChamber;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

class SplitBulletMono : MonoBehaviour
{
    static public readonly int bulletsAfterSplitCount = 10;
    static public readonly float splitDelay = 0.5f;

    private float splitTime;

    private GameObject? parent = null;
    public Player player;
    private List<GameObject>? newInstacesOfAddToProjectile = null;
    private SimulatedGun simulatedGun;

    private void BuildSimulatedGun()
    {
        simulatedGun = new GameObject("A_SANYA_SimulatedGun").AddComponent<SimulatedGun>();
        simulatedGun.gameObject.hideFlags = HideFlags.HideAndDontSave;
        //System.Type type = typeof(SimulatedGun);
        //FieldInfo fieldInfo = type.GetField("simulationID", BindingFlags.NonPublic | BindingFlags.Instance);
        //if (fieldInfo != null)
        //{
        //    int ID = (int)fieldInfo.GetValue(simulatedGun);
        //    UnityEngine.Debug.Log("SimID: " + ID);
        //}

        Gun gun = player.data.weaponHandler.gun;
        simulatedGun.CopyGunStatsExceptActions(gun);
        simulatedGun.CopyAttackAction(gun);
        simulatedGun.CopyShootProjectileAction(gun);

        simulatedGun.numberOfProjectiles = bulletsAfterSplitCount / 2; // we shoot with 2 guns
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
        simulatedGun.drag = 0.0f;

        simulatedGun.chargeDamageMultiplier = 1.0f;
        simulatedGun.chargeEvenSpreadTo = 0.0f;
        simulatedGun.chargeNumberOfProjectilesTo = 0;
        simulatedGun.chargeRecoilTo = 0.0f;
        simulatedGun.chargeSpeedTo = 0.0f;
        simulatedGun.chargeSpreadTo = 0.0f;

        var newObjectsToSpawn = new List<ObjectsToSpawn>();
        newInstacesOfAddToProjectile = new List<GameObject>();
        foreach (var oldObjectsToSpawn in gun.objectsToSpawn)
        {
            GameObject? addToProjectile = oldObjectsToSpawn.AddToProjectile;
            if (addToProjectile != null)
            {
                bool addToProjectileChanged = false;
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
                AddToProjectile = addToProjectile,
                removeScriptsFromProjectileObject = oldObjectsToSpawn.removeScriptsFromProjectileObject,
                scaleStacks = oldObjectsToSpawn.scaleStacks,
                scaleStackM = oldObjectsToSpawn.scaleStackM,
                scaleFromDamage = oldObjectsToSpawn.scaleFromDamage,
                stacks = oldObjectsToSpawn.stacks
            });
        }
        simulatedGun.objectsToSpawn = newObjectsToSpawn.ToArray();
    }

    public void Start()
    {
        if (transform.parent == null)
        {
            return;
        }
        parent = transform.parent.gameObject;
        splitTime = Time.time + splitDelay;
    }

    public void OnDestroy()
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

    private void Update()
    {
        if (parent == null)
        {
            return;
        }
        else if (Time.time >= splitTime)
        {
            BuildSimulatedGun();
            if (player.data.view.IsMine || PhotonNetwork.OfflineMode)
            {
                var parentMoveTransform = parent.GetComponent<MoveTransform>();
                Vector2 shootDirection = parentMoveTransform.velocity.normalized;
                if (shootDirection == Vector2.zero)
                {
                    shootDirection = Vector2.right;
                }

                simulatedGun.SimulatedAttack(player.playerID, parent.transform.position, shootDirection, 1.0f, 1.0f, null, false);
                simulatedGun.SimulatedAttack(player.playerID, parent.transform.position, -shootDirection, 1.0f, 1.0f, null, false);
            }
            Destroy(parent);
        }
    }
}
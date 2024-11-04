using Photon.Pun.Simple;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class KineticBatteryMono : MonoBehaviour
    {
        public static readonly float absorbFactor = 0.5f;

        Player player;
        Gun gun;
        float damageStored;

        void Start()
        {
            player = GetComponentInParent<Player>();
            player.data.stats.WasDealtDamageAction += WasDealthDamage;

            Gun gun = player.data.weaponHandler.gun;
            gun.ShootPojectileAction += ShootAction;
        }

        void OnDisable()
        {
            damageStored = 0f;
        }

        void OnDestroy()
        {
            player.data.stats.WasDealtDamageAction -= WasDealthDamage;
            gun.ShootPojectileAction -= ShootAction;
        }

        void WasDealthDamage(Vector2 damage, bool selfDamage)
        {
            if (player.data.lastSourceOfDamage == null || player.data.lastSourceOfDamage.teamID == player.teamID)
            {
                return;
            }

            damageStored += damage.magnitude * absorbFactor;
            //player.data.healthHandler.Heal(damage.magnitude);
        }

        void ShootAction(GameObject projectile)
        {
            if (damageStored <= 0f)
            {
                return;
            }

            SpawnedAttack component = projectile.GetComponent<SpawnedAttack>();
            if (!component)
            {
                return;
            }

            // TODO: bullet wont have impact sound (if initial damage isnt enough)
            ProjectileHit hit = projectile.GetComponent<ProjectileHit>();
            hit.damage += damageStored;
            damageStored = 0f;
        }
    }
}

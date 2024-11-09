using ModdingUtils.Extensions;
using ModdingUtils.GameModes;
using Photon.Pun.Simple;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class KineticBatteryMono : MonoBehaviour
    {
        public static readonly float absorbFactor = 0.75f;

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

        public void ResetDamageStored()
        {
            // can be called 2 times if player did lose
            damageStored = 0f;
        }

        internal static IEnumerator OnPointEnd()
        {
            using (List<Player>.Enumerator enumerator = PlayerManager.instance.players.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Player player = enumerator.Current;
                    KineticBatteryMono? mono = player.GetComponentInChildren<KineticBatteryMono>();
                    if (mono != null)
                    {
                        mono.ResetDamageStored();
                    }
                }
                yield break;
            }
        }

        void OnDisable()
        {
            // this thing exists, just for Sandbox
            if (player.data.dead) // if not revive
            {
                ResetDamageStored();
            }
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

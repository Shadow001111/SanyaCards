using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class HealthTaxesMono : MonoBehaviour
    {
        Player player;

        void Start()
        {
            player = GetComponentInParent<Player>();

            foreach (Player other in PlayerManager.instance.players)
            {
                if (player.teamID != other.teamID)
                {
                    other.data.weaponHandler.gun.ShootPojectileAction += ShootProjectile;
                }
            }
        }

        void ShootProjectile(GameObject projectile)
        {
            ProjectileHit proj = projectile.GetComponent<ProjectileHit>();
            Player player = proj.ownPlayer;
            player.data.healthHandler.TakeDamage(Vector2.right * (proj.damage / 55f) * 3f, Vector2.zero, lethal: false);
        }

        void OnDestroy()
        {
            foreach (Player other in PlayerManager.instance.players)
            {
                if (player.teamID != other.teamID)
                {
                    other.data.weaponHandler.gun.ShootPojectileAction -= ShootProjectile;
                }
            }
        }
    }
}

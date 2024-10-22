using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Reflection;

namespace SanyaCards.Monos
{
    class MagneticBulletMono : MonoBehaviour
    {
        MoveTransform moveTransform;
        ProjectileHit bullet;

        public int ignoreTeamID;

        static FieldInfo playerVelocityField = typeof(PlayerVelocity).GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        void Start()
        {
            if (transform.parent == null)
            {
                return;
            }
            moveTransform = transform.parent.gameObject.GetComponent<MoveTransform>();
            bullet = transform.parent.gameObject.GetComponent<ProjectileHit>();
        }

        void FixedUpdate()
        {
            if (bullet == null)
            {
                return;
            }

            float gravity = 700.0f;

            Vector2 bulletPosition = transform.position;
            float bulletMass = (bullet.damage / 55f) * bullet.dealDamageMultiplierr;
            bulletMass = Mathf.Min(25f, bulletMass * bulletMass);

            foreach (var player in PlayerManager.instance.players)
            {
                if (player.teamID == ignoreTeamID)
                {
                    continue;
                }

                Vector2 playerPosition = player.transform.position;
                float playerMass = player.transform.localScale.x;
                playerMass = Mathf.Min(20f, playerMass * playerMass);

                Vector2 dpos = playerPosition - bulletPosition;
                Vector2 acc = dpos.normalized / Mathf.Max(9f, dpos.sqrMagnitude) * Time.fixedDeltaTime * gravity;

                moveTransform.velocity.x += (playerMass * acc).x;
                moveTransform.velocity.y += (playerMass * acc).y;

                PlayerVelocity playerVelocityMono = player.data.playerVel;
                Vector2 playerVelocity = (Vector2)playerVelocityField.GetValue(playerVelocityMono);
                playerVelocityField.SetValue(playerVelocityMono, playerVelocity - bulletMass * acc);
            }
        }
    }
}

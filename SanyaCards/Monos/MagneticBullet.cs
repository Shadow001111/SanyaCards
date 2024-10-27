using UnityEngine;
using System.Reflection;

namespace SanyaCards.Monos
{
    class MagneticBulletMono : MonoBehaviour
    {
        MoveTransform moveTransform;
        ProjectileHit bullet;

        public int ignoreTeamID;

        static FieldInfo playerVelocityField = typeof(PlayerVelocity).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance);

        void Start()
        {
            if (transform.parent == null)
            {
                return;
            }
            moveTransform = transform.parent.GetComponent<MoveTransform>();
            bullet = transform.parent.GetComponent<ProjectileHit>();
        }

        void FixedUpdate()
        {
            if (bullet == null)
            {
                return;
            }

            float gravity = 120.0f;

            float bulletMass = (bullet.damage / 55f) * bullet.dealDamageMultiplierr;
            bulletMass = Mathf.Min(40f, 3.37f * bulletMass);

            foreach (Player player in PlayerManager.instance.players)
            {
                if (player.teamID == ignoreTeamID)
                {
                    continue;
                }

                Vector2 dpos = (Vector2)player.transform.position - (Vector2)transform.position;
                float distance = dpos.magnitude;
                if (distance < 5.0f)
                {
                    Destroy(this);
                }

                Vector2 acc = dpos.normalized / Mathf.Max(5f, distance) * Time.fixedDeltaTime * gravity;

                float playerMass = player.transform.localScale.x;
                playerMass = Mathf.Min(20f, playerMass * playerMass);

                moveTransform.velocity.x += (playerMass * acc).x;
                moveTransform.velocity.y += (playerMass * acc).y;

                PlayerVelocity playerVelocityMono = player.data.playerVel;
                Vector2 playerVelocity = (Vector2)playerVelocityField.GetValue(playerVelocityMono);
                playerVelocityField.SetValue(playerVelocityMono, playerVelocity - bulletMass * acc);
            }
        }
    }
}

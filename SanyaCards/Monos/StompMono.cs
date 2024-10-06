using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class StompMono : MonoBehaviour
    {
        public static readonly float abilityCooldown = 2.0f;
        private float abilityUseTime;

        private Player player;
        private CharacterData characterData;
        private CircleCollider2D collider;

        private Explosion explosion;

        private void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponent<Player>();
            player.data.block.BlockAction += this.OnBlock;

            characterData = GetComponent<CharacterData>();
            collider = GetComponent<CircleCollider2D>();

            var objectsToSpawn = ((GameObject)Resources.Load("0 cards/Explosive bullet")).GetComponent<Gun>().objectsToSpawn[0];
            var explosionEffect = Instantiate(objectsToSpawn.effect);
            explosionEffect.hideFlags = HideFlags.HideAndDontSave;
            explosionEffect.transform.position = new Vector3(1000, 0, 0);
            explosionEffect.name = "A_SANYA_StompExplosion";

            Destroy(explosionEffect.GetComponent<RemoveAfterSeconds>());
            explosionEffect.AddComponent<SpawnedAttack>().spawner = player;

            explosion = explosionEffect.GetComponent<Explosion>();
            explosion.range = 5.0f;
        }

        public void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType != BlockTrigger.BlockTriggerType.Default || Time.time < abilityUseTime)
            {
                return;
            }

            LayerMask obstacleLayers = (1 << 18) | (1 << 17) | (1 << 10) |(1 << 0);
            Vector2 rayPosition = new Vector2(transform.position.x, transform.position.y - collider.radius - 0.5f);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.down, 100.0f, obstacleLayers);

            bool usedAbility = false;
            if (hit.collider != null && hit.collider.gameObject)
            {
                if (hit.distance < 5.0f)
                {
                    return;
                }
                usedAbility = true;

                // calculate attack power based on height
                const float minHeight = 3.0f;
                const float maxHeight = 15.0f;

                float power = Mathf.Min((hit.distance - minHeight) / (maxHeight - minHeight), 1.0f);
                explosion.damage = Mathf.Lerp(20.0f, 150.0f, power);
                explosion.force = Mathf.Lerp(1.0f, 5.0f, power) * 10000.0f;

                // move player
                Vector3 newPosition = new Vector3(transform.position.x, hit.point.y + collider.radius + 0.1f, transform.position.z);
                transform.position = newPosition;
                player.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);

                // set player velocity y to 0
                var velocityField = typeof(PlayerVelocity).GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Vector2 currentVelocity = (Vector2)velocityField.GetValue(characterData.playerVel);
                currentVelocity.y = 0;
                velocityField.SetValue(characterData.playerVel, currentVelocity);

                Vector3 hitPosition = new Vector3(hit.point.x + hit.normal.x * 0.1f, hit.point.y + hit.normal.y * 0.1f, 0.0f);
                Instantiate(explosion.gameObject, hitPosition, Quaternion.identity);
            }
            if (usedAbility)
            {
                abilityUseTime = Time.time + abilityCooldown;
            }
        }
    }
}

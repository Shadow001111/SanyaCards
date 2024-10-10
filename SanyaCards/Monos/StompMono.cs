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

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += this.OnBlock;

            characterData = player.GetComponent<CharacterData>();
            collider = player.GetComponent<CircleCollider2D>();

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

            float colliderOffset = collider.offset.y - collider.bounds.extents.y;

            LayerMask obstacleLayers = (1 << 18) | (1 << 17) | (1 << 10) | (1 << 0);
            Vector2 rayPosition = new Vector2(player.transform.position.x, player.transform.position.y + colliderOffset - 0.1f);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.down, 100.0f, obstacleLayers);

            if (hit.collider != null && hit.collider.gameObject)
            {
                if (hit.distance < 5.0f)
                {
                    UnityEngine.Debug.Log($"Not enough  height: {hit.distance}");
                    return;
                }
                abilityUseTime = Time.time + abilityCooldown;

                // calculate attack power based on height
                const float minHeight = 3.0f;
                const float maxHeight = 15.0f;

                float power = Mathf.Min((hit.distance - minHeight) / (maxHeight - minHeight), 1.0f);
                explosion.damage = Mathf.Lerp(20.0f, 150.0f, power);
                explosion.force = Mathf.Lerp(1.0f, 5.0f, power) * 10000.0f;

                // move player
                player.transform.position = new Vector3(player.transform.position.x, hit.point.y - colliderOffset, player.transform.position.z);

                // set player velocity y to 0
                var velocityField = typeof(PlayerVelocity).GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Vector2 currentVelocity = (Vector2)velocityField.GetValue(characterData.playerVel);
                currentVelocity.y = 0;
                velocityField.SetValue(characterData.playerVel, currentVelocity);

                // creating explosion effect
                Vector3 hitPosition = hit.point + hit.normal * 0.1f;
                Instantiate(explosion.gameObject, hitPosition, Quaternion.identity);
            }
        }
    }
}

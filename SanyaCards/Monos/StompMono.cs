using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UnityEngine;
using MapsExt;
using MapsExt.Utils;

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

        void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

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
            explosion.scaleRadius = true;
            explosion.scaleDmg = true;
            explosion.scaleForce = true;
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
        }

        // TODO: maybe use reflectections of MapsExtended methods
        static Vector2 GetMapSize()
        {
            CustomMap customMap = MapManager.instance.GetCurrentCustomMap();
            return (customMap == null) ? new Vector2(71.12f, 40f) : ConversionUtils.ScreenToWorldUnits(customMap.Settings.MapSize);
        }

        static float GetHeightToBottomEdge(Vector3 worldPosition)
        {
            Vector2 mapSize = GetMapSize();
            float bottomEdgeY = -mapSize.y * 0.5f;
            float distanceToBottom = worldPosition.y - bottomEdgeY;
            return distanceToBottom;
        }

        public void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType != BlockTrigger.BlockTriggerType.Default || Time.time < abilityUseTime)
            {
                return;
            }

            float colliderOffset = collider.offset.y - collider.bounds.extents.y;
            float size = player.transform.localScale.x;

            // raycast
            LayerMask obstacleLayers = (1 << 18) | (1 << 17) | (1 << 10) | (1 << 0);
            Vector2 rayPosition = new Vector2(player.transform.position.x, player.transform.position.y + colliderOffset - 0.1f);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.down, 100.0f, obstacleLayers);

            if (hit.collider != null && hit.collider.gameObject)
            {
                const float minHeight = 4.0f;
                const float maxHeight = 15.0f;

                if (hit.distance < minHeight)
                {
                    return;
                }

                float edgeDistance = GetHeightToBottomEdge(rayPosition);
                if (hit.distance >= edgeDistance)
                {
                    return;
                }

                abilityUseTime = Time.time + abilityCooldown;

                // calculate attack power based on height
                float power = Mathf.Min((hit.distance - minHeight) / (maxHeight - minHeight), 1.0f);
                explosion.damage = Mathf.Lerp(10.0f, 120.0f, power);
                explosion.force = Mathf.Lerp(1.0f, 5.0f, power) * 1000.0f;
                explosion.range = Mathf.Lerp(4.0f, 7.0f, power);

                // move player
                player.transform.position = new Vector3(player.transform.position.x, hit.point.y - colliderOffset, player.transform.position.z);

                // set player velocity y to 0
                var velocityField = typeof(PlayerVelocity).GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                Vector2 currentVelocity = (Vector2)velocityField.GetValue(characterData.playerVel);
                currentVelocity.y = 0;
                velocityField.SetValue(characterData.playerVel, currentVelocity);

                // creating explosion effect
                Vector3 hitPosition = hit.point + hit.normal * 0.1f;
                Instantiate(explosion.gameObject, hitPosition, Quaternion.identity).transform.localScale = Vector3.one * size;
            }
        }
    }
}

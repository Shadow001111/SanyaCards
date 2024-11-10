using System;
using System.Reflection;
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
        static readonly float stompMinHeight = 4.0f;
        static readonly float stompMaxHeight = 15.0f;
        float abilityUseTime;

        Player player;
        bool playerIsMine;
        CharacterData characterData;
        CircleCollider2D collider;

        Explosion explosion;

        class StompRaycastInfo
        {
            public float dx = 0.0f;
            public float distance = -1.0f;
            public float stompDistance = -1.0f;
            public Vector2 point = Vector2.zero;
            public Vector2 normal = Vector2.zero;
        }

        static readonly int raycastHitsCount = 5;

        static readonly int positionIndicatorSegments = 25;
        LineRenderer positionIndicatorLineRenderer;

        static FieldInfo velocityField = typeof(PlayerVelocity).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance);

        void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            characterData = player.data;
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

            //
            playerIsMine = player.data.view.IsMine && player.GetComponentInChildren<PlayerAI>() == null;
            if (playerIsMine)
            {
                positionIndicatorLineRenderer = gameObject.AddComponent<LineRenderer>();
                positionIndicatorLineRenderer.positionCount = positionIndicatorSegments + 1;
                positionIndicatorLineRenderer.useWorldSpace = true;
                positionIndicatorLineRenderer.material = new Material(Shader.Find("Unlit/Color"));
                positionIndicatorLineRenderer.startColor = UnityEngine.Color.white;
                positionIndicatorLineRenderer.endColor = positionIndicatorLineRenderer.startColor;
                positionIndicatorLineRenderer.startWidth = 0.1f;
                positionIndicatorLineRenderer.endWidth = positionIndicatorLineRenderer.startWidth;
            }
        }

        void FixedUpdate()
        {
            if (!playerIsMine)
            {
                return;
            }

            StompRaycastInfo? info = DoRaycasts();
            if (info == null)
            {
                positionIndicatorLineRenderer.enabled = false;
                return;
            }

            positionIndicatorLineRenderer.enabled = true;

            float playerRadius = collider.bounds.extents.y;
            float radius2 = Mathf.Sqrt(playerRadius * playerRadius - info.dx * info.dx);
            float colliderOffset = collider.offset.y - radius2 - 0.05f;
            Vector3 position = new Vector3(player.transform.position.x, player.transform.position.y + colliderOffset - info.distance + playerRadius, player.transform.position.z);

            float angle = 0f;
            float angleStep = 360f / positionIndicatorSegments;
            float progress = Mathf.Min(1f, 1f - (abilityUseTime - Time.time) / abilityCooldown);
            float progress360 = 360f * progress;

            int segmentsCount = Mathf.CeilToInt(progress * positionIndicatorSegments);
            positionIndicatorLineRenderer.positionCount = segmentsCount + 1;
            for (int i = 0; i < positionIndicatorLineRenderer.positionCount; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * playerRadius;
                float y = Mathf.Sin(Mathf.Deg2Rad * angle) * playerRadius;

                positionIndicatorLineRenderer.SetPosition(i, position + new Vector3(x, y, 0f));
                angle += angleStep;
            }
        }

        void DoRaycastAt(int i, ref StompRaycastInfo info)
        {
            float radius = collider.bounds.extents.x;
            LayerMask obstacleLayers = (1 << 18) | (1 << 17) | (1 << 10) | (1 << 0);
            float raycastHalfWidth = Mathf.Min(radius * 0.5f, 1.0f);

            float dx0 = -1.0f + 2.0f * i / (raycastHitsCount - 1);
            float dx = dx0 * raycastHalfWidth;

            float radius2 = Mathf.Sqrt(radius * radius - dx * dx);
            float distance = -1.0f;

            float colliderOffset = collider.offset.y - radius2 - 0.05f;
            Vector2 rayPosition = new Vector2(player.transform.position.x + dx, player.transform.position.y + colliderOffset);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.down, 100.0f, obstacleLayers);

            distance = hit.distance;
            if (hit.collider == null || hit.collider.gameObject == null || hit.distance >= GetHeightToBottomEdge(rayPosition))
            {
                distance = -1.0f;
            }

            info.dx = dx;
            info.distance = distance;
            info.stompDistance = distance - (radius - radius2);
            info.point = hit.point;
            info.normal = hit.normal;
        }

        StompRaycastInfo? DoRaycasts()
        {
            StompRaycastInfo info = new StompRaycastInfo();

            int center = raycastHitsCount / 2;
            DoRaycastAt(center, ref info);
            if (info.stompDistance >= stompMinHeight)
            {
                return info;
            }

            for (int offset = 1; offset <= center; offset++)
            {
                DoRaycastAt(center - offset, ref info);
                if (info.stompDistance >= stompMinHeight)
                {
                    return info;
                }

                DoRaycastAt(center + offset, ref info);
                if (info.stompDistance >= stompMinHeight)
                {
                    return info;
                }
            }
            return null;
        }

        void OnDisable()
        {
            abilityUseTime = Time.time;
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

            // raycast
            StompRaycastInfo? hit = DoRaycasts();
            if (hit == null)
            {
                return;
            }

            abilityUseTime = Time.time + abilityCooldown;

            // calculate attack power based on height
            float power = Mathf.Min((hit.stompDistance - stompMinHeight) / (stompMaxHeight - stompMinHeight), 1.0f);
            explosion.damage = Mathf.Lerp(10.0f, 120.0f, power);
            explosion.force = Mathf.Lerp(1.0f, 5.0f, power) * 1000.0f;
            explosion.range = Mathf.Lerp(4.0f, 7.0f, power);
            //float shake = Mathf.Lerp(1.0f, 5.0f, power) * player.transform.localScale.x;

            // move player
            float radius = collider.bounds.extents.y;
            float radius2 = Mathf.Sqrt(radius * radius - hit.dx * hit.dx);
            float colliderOffset = collider.offset.y - radius2 - 0.05f;
            player.GetComponent<PlayerCollision>().IgnoreWallForFrames(2);
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - colliderOffset - hit.distance, player.transform.position.z);

            // set player velocity y to 0
            Vector2 currentVelocity = (Vector2)velocityField.GetValue(characterData.playerVel);
            currentVelocity.y = 0;
            velocityField.SetValue(characterData.playerVel, currentVelocity);

            // creating explosion effect
            Vector3 hitPosition = hit.point + hit.normal * 0.1f;
            Instantiate(explosion.gameObject, hitPosition, Quaternion.identity).transform.localScale = Vector3.one * player.transform.localScale.x;
            //GamefeelManager.GameFeel(new Vector2(shake, 1.0f));
        }
    }
}

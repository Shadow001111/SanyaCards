#define SHOW_HIT_POINTS

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
        float abilityUseTime;

        Player player;
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

        static readonly int raycastHitCubesCount = 5;
        StompRaycastInfo[] raycastHitInfo;
#if SHOW_HIT_POINTS
        GameObject[] raycastHitCubes;
#endif

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

            // raycastHitInfo
            raycastHitInfo = new StompRaycastInfo[raycastHitCubesCount];
            for (int i = 0; i < raycastHitCubesCount; i++)
            {
                raycastHitInfo[i] = new StompRaycastInfo();
            }

#if SHOW_HIT_POINTS
            //rayscastHitCubes
            GameObject rhCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rhCube.transform.localScale = Vector3.one * 0.2f;
            var rhCubeRenderer = rhCube.GetComponent<Renderer>();
            rhCubeRenderer.material.color = new UnityEngine.Color(0.0f, 2.0f, 0.0f);

            raycastHitCubes = new GameObject[raycastHitCubesCount];
            raycastHitCubes[0] = rhCube;
            for (int i = 1; i < raycastHitCubesCount; i++)
            {
                raycastHitCubes[i] = Instantiate(rhCube);
            }
#endif
        }

        void Update()
        {
#if SHOW_HIT_POINTS
            GetRaycasts();

            float radius = collider.bounds.extents.y;
            for (int i = 0; i < raycastHitCubesCount; i++)
            {
                StompRaycastInfo info = raycastHitInfo[i];
                bool active = info.distance >= 0.0f;
                raycastHitCubes[i].SetActive(active);
                if (!active)
                {
                    continue;
                }
                float radius2 = Mathf.Sqrt(radius * radius - info.dx * info.dx);
                float colliderOffset = collider.offset.y - radius2 - 0.05f;
                raycastHitCubes[i].transform.position = new Vector2(player.transform.position.x + info.dx, player.transform.position.y + colliderOffset - info.distance);
            }
#endif
        }

        void GetRaycasts()
        {
            float radius = collider.bounds.extents.x;
            LayerMask obstacleLayers = (1 << 18) | (1 << 17) | (1 << 10) | (1 << 0);
            float raycastHalfWidth = Mathf.Min(radius * 0.5f, 1.0f);

            for (int i = 0; i < raycastHitCubesCount; i++)
            {
                float dx0 = -1.0f + 2.0f * i / (raycastHitCubesCount - 1);
                float dx = dx0 * raycastHalfWidth;

                var info = raycastHitInfo[i];
                float radius2 = Mathf.Sqrt(radius * radius - dx * dx);
                float distance = -1.0f;

                float colliderOffset = collider.offset.y - radius2 - 0.05f;
                Vector2 rayPosition = new Vector2(player.transform.position.x + dx, player.transform.position.y + colliderOffset);
                RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.down, 100.0f, obstacleLayers);

                distance = hit.distance;
                if (hit.collider == null || hit.collider.gameObject == null)
                {
                    distance = -1.0f;
                }
                else if (hit.distance >= GetHeightToBottomEdge(rayPosition))
                {
                    distance = -1.0f;
                }

                info.dx = dx;
                info.distance = distance;
                info.stompDistance = distance - (radius - radius2);
                info.point = hit.point;
                info.normal = hit.normal;
            }
        }

        StompRaycastInfo GetRaycastInfo()
        {
            int center = raycastHitCubesCount / 2;
            StompRaycastInfo info = raycastHitInfo[center];
            if (info.stompDistance >= 0.0f)
            {
                return info;
            }

            for (int offset = 1; offset <= center; offset++)
            {
                info = raycastHitInfo[center - offset];
                if (info.stompDistance >= 0.0f)
                {
                    return info;
                }

                info = raycastHitInfo[center + offset];
                if (info.stompDistance >= 0.0f)
                {
                    return info;
                }
            }
            return raycastHitInfo[center];
        }

        void OnDisable()
        {
            abilityUseTime = Time.time;
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
#if SHOW_HIT_POINTS
            for (int i = 1; i < raycastHitCubesCount; i++)
            {
                Destroy(raycastHitCubes[i]);
            }
#endif
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

            GetRaycasts();
            StompRaycastInfo hit = GetRaycastInfo();
            if (hit.stompDistance < 0.0f)
            {
                return;
            }

            // raycast
            const float minHeight = 4.0f;
            const float maxHeight = 15.0f;

            if (hit.stompDistance < minHeight)
            {
                return;
            }

            abilityUseTime = Time.time + abilityCooldown;

            // calculate attack power based on height
            float power = Mathf.Min((hit.stompDistance - minHeight) / (maxHeight - minHeight), 1.0f);
            explosion.damage = Mathf.Lerp(10.0f, 120.0f, power);
            explosion.force = Mathf.Lerp(1.0f, 5.0f, power) * 1000.0f;
            explosion.range = Mathf.Lerp(4.0f, 7.0f, power);
            //float shake = Mathf.Lerp(1.0f, 5.0f, power) * player.transform.localScale.x;

            // move player
            float radius = collider.bounds.extents.y;
            float radius2 = Mathf.Sqrt(radius * radius - hit.dx * hit.dx);
            float colliderOffset = collider.offset.y - radius2 - 0.05f;
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + colliderOffset - hit.distance, player.transform.position.z);

            // set player velocity y to 0
            var velocityField = typeof(PlayerVelocity).GetField("velocity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
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

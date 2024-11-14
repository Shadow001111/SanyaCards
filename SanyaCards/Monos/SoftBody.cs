using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class SoftBodyMono : MonoBehaviour
    {
        public static int segmentsCount = 500;
        public static float segmentLength = 5f;
        public static float k = 45f;

        bool isActive;

        Player player;
        Vector3 lastPlayerPosition;
        LineRenderer lineRenderer;

        static FieldInfo velocityField = typeof(PlayerVelocity).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance);

        void Start ()
        {
            player = GetComponentInParent<Player>();

            lineRenderer = gameObject.AddComponent<LineRenderer>();

            lineRenderer.startWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.white;
            lineRenderer.positionCount = segmentsCount + 1;
            lineRenderer.useWorldSpace = true;

            player.data.block.BlockAction += OnBlock;

            lastPlayerPosition = player.transform.position;
        }

        void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType == BlockTrigger.BlockTriggerType.Default)
            {
                isActive = !isActive;
                lineRenderer.enabled = isActive;
            }
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
        }

        void FixedUpdate()
        {
            if (!isActive)
            {
                lastPlayerPosition = player.transform.position;
                return;
            }

            Vector2 addVelocity = Vector2.zero;

            int mask = (1 << 18) | (1 << 17) | (1 << 0);
            float angleStep = (360f * Mathf.Deg2Rad) / segmentsCount;
            for (int i = 0;  i < segmentsCount; i++)
            {
                float angle = i * angleStep;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);

                Vector2 rayPosition = player.transform.position;
                Vector2 rayDirection = new Vector2(cos, sin);
                RaycastHit2D hit = Physics2D.Raycast(rayPosition, rayDirection, segmentLength, mask);
                if (hit.collider == null)
                {
                    lineRenderer.SetPosition(i, rayPosition + rayDirection * segmentLength);
                    continue;
                }

                float distance = hit.distance;
                float f = (segmentLength - distance) / segmentLength;
                f = 1f - f;
                f *= f;
                f = 1f - f;
                float F = -k * f;
                addVelocity += rayDirection * (F * Time.fixedDeltaTime);

                lineRenderer.SetPosition(i, rayPosition + rayDirection * distance);
            }

            Vector2 playerVelocity = (player.transform.position - lastPlayerPosition) / Time.fixedDeltaTime;
            playerVelocity += addVelocity;

            velocityField.SetValue(player.data.playerVel, playerVelocity);
            lineRenderer.SetPosition(segmentsCount, lineRenderer.GetPosition(0));

            UnityEngine.Debug.Log($"{(player.transform.position.y - lastPlayerPosition.y) / Time.fixedDeltaTime} {addVelocity.y}");

            lastPlayerPosition = player.transform.position;
        }
    }
}

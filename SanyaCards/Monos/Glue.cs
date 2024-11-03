using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class GlueBulletMono : RayHitEffect
    {
        class GlueMono : MonoBehaviour
        {
            public float glueMultiplier;
            Player target;
            Vector2 myPosition;
            Vector2 myOffset;

            static FieldInfo playerVelocityField = typeof(PlayerVelocity).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance);

            LineRenderer lineRenderer;

            void Start()
            {
                target = transform.parent.GetComponent<Player>();

                myPosition = transform.position;
                myOffset = transform.position - target.transform.position;

                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = lineRenderer.startWidth;
                lineRenderer.positionCount = 2;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
            }

            void OnDisable()
            {
                Destroy(lineRenderer);
                Destroy(this);
            }

            void FixedUpdate()
            {
                Vector2 targetPosition = target.transform.position;
                PlayerVelocity targetVelocityComponent = target.data.playerVel;
                float targetMass = target.transform.localScale.x; targetMass *= targetMass;

                Vector2 dpos = myPosition - (targetPosition + myOffset);
                Vector2 force = dpos * 15.0f * glueMultiplier;

                Vector2 playerVelocity = (Vector2)playerVelocityField.GetValue(targetVelocityComponent);
                playerVelocity += force * (Time.fixedDeltaTime / targetMass);
                playerVelocityField.SetValue(targetVelocityComponent, playerVelocity);

                // update line renderer
                lineRenderer.SetPosition(0, myPosition);
                lineRenderer.SetPosition(1, targetPosition + myOffset);
            }
        }

        public override HasToReturn DoHitEffect(HitInfo hit)
        {
            if (hit.transform == null)
            {
                return HasToReturn.canContinue;
            }

            if (hit.transform.GetComponent<Player>() == null)
            {
                return HasToReturn.canContinue;
            }

            if (!hit.transform.gameObject.activeSelf)
            {
                foreach (GlueMono removeGlue in hit.transform.GetComponentsInChildren<GlueMono>())
                {
                    Destroy(removeGlue.gameObject);
                }
                return HasToReturn.canContinue;
            }

            GameObject glue = new GameObject("Glue");
            glue.transform.position = hit.point;
            glue.transform.SetParent(hit.transform, true);

            GlueMono glueMono = glue.AddComponent<GlueMono>();
            glueMono.glueMultiplier = transform.localScale.x;
            UnityEngine.Debug.Log($"{transform.localScale.x}");

            Destroy(glue, 5.0f);

            return HasToReturn.canContinue;
        }
    }
}

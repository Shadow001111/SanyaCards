using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class AcidShieldMono : MonoBehaviour
    {
        public static readonly float abilityCooldown = 4f;
        public static readonly float abilityDuration = 3f;
        public static readonly float abilityRadius = 15f;

        Player player;
        bool abilityActive;
        float abilityUseTime;

        GameObject lineEffectGO;
        LineRenderer lineRenderer;
        LineEffect lineEffect;

        void Start()
        {
            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            lineEffectGO = new GameObject("LineEffect");
            lineEffectGO.transform.SetParent(transform);

            lineRenderer = lineEffectGO.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.startColor = Color.green;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));

            lineRenderer.endWidth = lineRenderer.startWidth;
            lineRenderer.endColor = lineRenderer.startColor;

            lineEffect = lineEffectGO.AddComponent<LineEffect>();
            lineEffect.lineType = LineEffect.LineType.Ring;
            lineEffect.radius = abilityRadius;
            lineEffect.segments = 50;
            lineEffect.globalTimeSpeed = 1f;
            lineEffect.loop = true;
            lineEffect.useColorOverTime = false;
            lineEffect.effects = new LineEffectInstance[] { };
        }

        void FixedUpdate()
        {
            if (!abilityActive)
            {
                return;
            }

            int mask = LayerMask.GetMask(new string[]
            {
                "Projectile"
            });

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, abilityRadius, 1 << 15);
            foreach (Collider2D collider in hitColliders)
            {
                if (!collider.transform.root.CompareTag("Bullet"))
                {
                    continue;
                }

                SpawnedAttack attack = collider.GetComponentInParent<SpawnedAttack>();
                if (attack == null)// || attack.spawner.teamID == player.teamID)
                {
                    continue;
                }

                ProjectileHit projectile = collider.gameObject.GetComponentInParent<ProjectileHit>();
                float factor = Mathf.Pow(0.25f, Time.fixedDeltaTime);
                projectile.damage *= factor;
                projectile.transform.localScale *= factor; // doesnt work

                if (projectile.damage < 0.1f)
                {
                    Destroy(projectile.gameObject);
                }
            }
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
        }

        void OnDisable()
        {
            if (abilityActive)
            {
                StopAllCoroutines();
                abilityActive = false;
                abilityUseTime = Time.time;
            }
        }

        public void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType != BlockTrigger.BlockTriggerType.Default || Time.time < abilityUseTime)
            {
                return;
            }
            abilityUseTime = Time.time + abilityCooldown;

            StartCoroutine(useAbility());
        }

        IEnumerator useAbility()
        {
            if (abilityActive)
            {
                yield break;
            }

            abilityActive = true;
            lineEffect.Play(player.transform);
            UnityEngine.Debug.Log("Enable");

            yield return new WaitForSeconds(abilityDuration);

            abilityActive = false;
            lineEffect.Stop();
            lineEffect.gameObject.SetActive(false);
            UnityEngine.Debug.Log("Disable");
        }
    }
}

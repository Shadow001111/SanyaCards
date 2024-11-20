using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class ShieldTemplateMono : MonoBehaviour
    {
        protected float baseAbilityCooldown;
        protected float baseAbilityDuration;
        protected float baseAbilityRadius;

        protected Player player { private set; get; }
        protected bool abilityActive { private set; get; }
        float abilityUseTime;

        GameObject lineEffectGO;
        LineRenderer lineRenderer;
        LineEffect lineEffect;

        protected virtual void Start()
        {
            transform.localPosition = Vector3.zero;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            lineEffectGO = new GameObject("LineEffect");
            lineEffectGO.transform.SetParent(transform);
            lineEffectGO.transform.localPosition = Vector3.zero;

            lineRenderer = lineEffectGO.AddComponent<LineRenderer>();
            lineEffect = lineEffectGO.AddComponent<LineEffect>();

            lineRenderer.startWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.white;
            lineRenderer.positionCount = 50;
            lineRenderer.useWorldSpace = true;

            lineRenderer.endWidth = lineRenderer.startWidth;

            lineEffect.lineType = LineEffect.LineType.Ring;
            lineEffect.radius = baseAbilityRadius;
            lineEffect.segments = lineRenderer.positionCount;
            lineEffect.globalTimeSpeed = 1f;
            lineEffect.loop = true;
            lineEffect.effects = new LineEffectInstance[0];
            lineEffect.raycastCollision = true;
        }

        protected virtual void FixedUpdate()
        {
            if (!abilityActive)
            {
                return;
            }

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, baseAbilityRadius * transform.root.localScale.x, 1 << 15);
            foreach (Collider2D collider in hitColliders)
            {
                ProjectileHit? projectile = collider.gameObject.GetComponentInParent<ProjectileHit>();
                if (projectile == null)
                { 
                    continue;
                }
                if (!bulletValidation(projectile))
                {
                    continue;
                }
                affectBullet(projectile);
            }
        }

        protected virtual void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
        }

        protected virtual void OnDisable()
        {
            if (abilityActive)
            {
                StopAllCoroutines();
                abilityActive = false;
                abilityUseTime = Time.time;

                lineEffect.Stop();
                lineEffect.gameObject.SetActive(false);
            }
        }

        void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType != BlockTrigger.BlockTriggerType.Default || Time.time < abilityUseTime)
            {
                return;
            }
            abilityUseTime = Time.time + baseAbilityCooldown;

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
            onShieldEnable();

            yield return abilityWaitDuration();

            abilityActive = false;
            lineEffect.Stop();
            lineEffect.gameObject.SetActive(false);
            onShieldDisable();
        }

        protected virtual IEnumerator abilityWaitDuration()
        {
            yield return new WaitForSeconds(baseAbilityDuration);
        }

        protected virtual bool bulletValidation(ProjectileHit projectile)
        {
            return true;
        }

        protected virtual void affectBullet(ProjectileHit projectile)
        {

        }

        protected virtual void onShieldEnable()
        {

        }

        protected virtual void onShieldDisable()
        {

        }

        protected void setRadius(float newRadius)
        {
            baseAbilityRadius = newRadius;
            lineEffect.radius = baseAbilityRadius;
        }

        protected void setColor(Color color)
        {
            lineRenderer.material.color = color;
        }
    }
}

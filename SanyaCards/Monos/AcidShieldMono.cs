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
        public static readonly float abilityRadius = 5f;

        Player player;
        bool abilityActive;
        float abilityUseTime;

        GameObject lineEffectGO;
        LineRenderer lineRenderer;
        LineEffect lineEffect;

        void Start()
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
            lineRenderer.material.color = Color.green;
            //lineRenderer.startColor = Color.green;
            lineRenderer.positionCount = 50;
            lineRenderer.useWorldSpace = true;

            lineRenderer.endWidth = lineRenderer.startWidth;
            //lineRenderer.endColor = lineRenderer.startColor;

            lineEffect.lineType = LineEffect.LineType.Ring;
            lineEffect.radius = abilityRadius;
            lineEffect.segments = lineRenderer.positionCount;
            lineEffect.globalTimeSpeed = 1f;
            lineEffect.loop = true;
            lineEffect.effects = new LineEffectInstance[0];
            lineEffect.raycastCollision = true;
        }

        void FixedUpdate()
        {
            if (!abilityActive)
            {
                return;
            }

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, abilityRadius * transform.root.localScale.x, 1 << 15);
            foreach (Collider2D collider in hitColliders)
            {
                if (!collider.transform.root.CompareTag("Bullet"))
                {
                    continue;
                }

                SpawnedAttack attack = collider.GetComponentInParent<SpawnedAttack>();
                if (attack == null || attack.spawner.teamID == player.teamID)
                {
                    continue;
                }

                if (!PlayerManager.instance.CanSeePlayer(collider.transform.position, player).canSee)
                {
                    continue;
                }

                ProjectileHit projectile = collider.gameObject.GetComponentInParent<ProjectileHit>();

                float dmgFactor = Mathf.Pow(0.25f, Time.fixedDeltaTime);
                float velFactor = Mathf.Pow(0.75f, Time.fixedDeltaTime);

                // damage
                projectile.damage *= dmgFactor;
                //projectile.transform.localScale *= dmgFactor; // doesnt seem to work
                //TrailRenderer trailRenderer = projectile.GetComponentInChildren<TrailRenderer>();
                //trailRenderer.widthMultiplier *= dmgFactor;
                //trailRenderer.time *= dmgFactor;

                // speed
                float bulletMass = projectile.damage / 55f;
                MoveTransform moveTransform = projectile.gameObject.GetComponent<MoveTransform>();
                float speedMinus = moveTransform.velocity.sqrMagnitude * (1f - velFactor) / bulletMass;
                if (speedMinus > moveTransform.velocity.magnitude)
                {
                    moveTransform.velocity = Vector3.zero;
                }
                else
                {
                    moveTransform.velocity -= moveTransform.velocity.normalized * speedMinus;
                }

                if (projectile.damage < 5f)
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

                lineEffect.Stop();
                lineEffect.gameObject.SetActive(false);
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

            yield return new WaitForSeconds(abilityDuration);

            abilityActive = false;
            lineEffect.Stop();
            lineEffect.gameObject.SetActive(false);
        }
    }
}

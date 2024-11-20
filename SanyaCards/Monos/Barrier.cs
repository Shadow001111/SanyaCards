using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    // TODO: add hit effects (barrier hit)
    // TODO: add barrier breaking effect
    // TODO: maybe add progress bar for barrier
    class BarrierMono : ShieldTemplateMono
    {
        public static readonly float abilityCooldown = 15f;
        public static readonly float abilityRadius = 5f;

        public static readonly float barrierInitialHealth = 100f;
        public static readonly float barrierHealthFadingNormalized = 0.1f;

        static MethodInfo destroyProjectileMethod = typeof(ProjectileHit).GetMethod("DestroyMe", BindingFlags.NonPublic | BindingFlags.Instance);

        float barrierHealth;

        protected override void Start()
        {
            baseAbilityCooldown = abilityCooldown;
            baseAbilityRadius = abilityRadius;

            base.Start();

            setColor(Color.cyan);
        }

        protected override void FixedUpdate()
        {
            if (abilityActive)
            {
                float barrierMaxHealth = getBarriedMaxHealth();
                barrierHealth -= barrierHealthFadingNormalized * barrierMaxHealth * Time.fixedDeltaTime;
                setRadius(0.5f + (barrierHealth / barrierMaxHealth) * (abilityRadius - 0.5f));
            }

            base.FixedUpdate();
        }

        protected override IEnumerator abilityWaitDuration()
        {
            return new WaitUntil(() => (barrierHealth <= 0));
        }

        protected override bool bulletValidation(ProjectileHit projectile)
        {
            return projectile.ownPlayer.teamID != player.teamID && PlayerManager.instance.CanSeePlayer(projectile.transform.position, player).canSee;
        }

        protected override void affectBullet(ProjectileHit projectile)
        {
            if (barrierHealth > projectile.damage)
            {
                barrierHealth -= projectile.damage;

                ProjectileCollision? collision = projectile.GetComponentInChildren<ProjectileCollision>();
                if (collision != null)
                {
                    collision.Die();
                }

                destroyProjectileMethod.Invoke(projectile, new object[] { });
            }
            else
            {
                projectile.damage -= barrierHealth;
                barrierHealth = 0f;
                // TODO: rescale bullet
            }
        }

        protected override void onShieldEnable()
        {
            barrierHealth = getBarriedMaxHealth();
        }

        float getBarriedMaxHealth()
        {
            return player.transform.localScale.x / player.data.stats.sizeMultiplier * barrierInitialHealth;
        }
    }
}

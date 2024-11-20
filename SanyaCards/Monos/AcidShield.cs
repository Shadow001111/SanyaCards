using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class AcidShieldMono : ShieldTemplateMono
    {
        public static readonly float abilityCooldown = 4f;
        public static readonly float abilityDuration = 3f;
        public static readonly float abilityRadius = 5f;

        protected override void Start()
        {
            baseAbilityCooldown = abilityCooldown;
            baseAbilityDuration = abilityDuration;
            baseAbilityRadius = abilityRadius;

            base.Start();

            setColor(Color.green);
        }

        protected override bool bulletValidation(ProjectileHit projectile)
        {
            return projectile.ownPlayer.teamID != player.teamID && PlayerManager.instance.CanSeePlayer(projectile.transform.position, player).canSee;
        }

        protected override void affectBullet(ProjectileHit projectile)
        {
            float dmgFactor = Mathf.Pow(0.25f, Time.fixedDeltaTime);
            float velFactor = Mathf.Pow(0.75f, Time.fixedDeltaTime);

            // damage
            projectile.damage /= dmgFactor;
            ScaleTrailFromDamage? scaleTrailFromDamage = projectile.gameObject.GetComponent<ScaleTrailFromDamage>();
            if (scaleTrailFromDamage == null)
            {
                projectile.gameObject.AddComponent<TrailRenderer>();
                scaleTrailFromDamage = projectile.gameObject.AddComponent<ScaleTrailFromDamage>();
            }
            scaleTrailFromDamage.Rescale();

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

            UnityEngine.Debug.Log(projectile.damage);
        }
    }
}

using ModdingUtils.MonoBehaviours;
using SoundImplementation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class SandwichMono : MonoBehaviour
    {
        public static readonly float abilityCooldown = 10.0f;
        public static readonly float abilityDuration = 2.0f;
        public static readonly float abilitySpeedDivider = 4.0f;

        private float abilityUseTime;

        private Player player;
        private CharacterStatModifiers stats;
        private HealthHandler healthHandler;
        private bool abilityActive = false;

        private void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            stats = player.GetComponent<CharacterStatModifiers>();
            healthHandler = player.GetComponent<HealthHandler>();
        }

        void OnDestroy()
        {
            player.data.block.BlockAction -= OnBlock;
        }

        private void OnDisable()
        {
            if (abilityActive)
            {
                StopAllCoroutines();
                abilityActive = false;
                stats.movementSpeed *= abilitySpeedDivider;
            }
        }

        public void OnBlock(BlockTrigger.BlockTriggerType triggerType)
        {
            if (triggerType != BlockTrigger.BlockTriggerType.Default || Time.time < abilityUseTime || player.data.health >= player.data.maxHealth)
            {
                return;
            }
            abilityUseTime = Time.time + abilityCooldown;
            StartCoroutine(useAbility());
        }

        private IEnumerator useAbility()
        {
            if (abilityActive)
            {
                yield break;
            }
            abilityActive = true;

            stats.movementSpeed /= abilitySpeedDivider;

            yield return new WaitForSeconds(abilityDuration);

            stats.movementSpeed *= abilitySpeedDivider;

            healthHandler.Heal(300.0f);

            abilityActive = false;
        }
    }
}

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
        public static readonly float heal = 300.0f;
        public static readonly float abilitySpeedDivider = 4.0f;
        static readonly int bitesCount = 5;

        float abilityUseTime;

        Player player;
        CharacterStatModifiers stats;
        HealthHandler healthHandler;
        bool abilityActive = false;

        AudioSource playerAudioSource;

        void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            stats = player.GetComponent<CharacterStatModifiers>();
            healthHandler = player.GetComponent<HealthHandler>();

            playerAudioSource = gameObject.AddComponent<AudioSource>();
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
                DisableEffect();
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

        IEnumerator useAbility()
        {
            if (abilityActive)
            {
                yield break;
            }
            abilityActive = true;

            EnableEffect();

            playerAudioSource.PlayOneShot(Assets.eatingSandwich);

            for (int i = 0; i < bitesCount; i++)
            {
                healthHandler.Heal(heal / bitesCount);
                yield return new WaitForSeconds(abilityDuration / bitesCount);
            }

            DisableEffect();

            abilityActive = false;
        }

        void EnableEffect()
        {
            stats.movementSpeed /= abilitySpeedDivider;
        }

        void DisableEffect()
        {
            stats.movementSpeed *= abilitySpeedDivider;
        }
    }
}

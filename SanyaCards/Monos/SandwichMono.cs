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

        static AudioSource playerAudioSource;

        private void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;

            stats = player.GetComponent<CharacterStatModifiers>();
            healthHandler = player.GetComponent<HealthHandler>();

            if (playerAudioSource == null)
            {
                playerAudioSource = player.gameObject.AddComponent<AudioSource>();
                UnityEngine.Debug.Log("[SanyaCards][Sandwich] playerAudioSource created");
            }
            else
            {
                UnityEngine.Debug.Log("[SanyaCards][Sandwich] playerAudioSource already was created");
            }
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

            if (playerAudioSource != null)
            {
                if (Assets.eatingSandwich != null)
                {
                    playerAudioSource.PlayOneShot(Assets.eatingSandwich);
                }
                else
                {
                    UnityEngine.Debug.Log("[SanyaCards][Sandwich] Assets.eatingSandwich is null");
                }
            }
            else
            {
                UnityEngine.Debug.Log("[SanyaCards][Sandwich] playerAudioSource Is null");
            }

            yield return new WaitForSeconds(abilityDuration);

            stats.movementSpeed *= abilitySpeedDivider;

            healthHandler.Heal(300.0f);

            abilityActive = false;
        }
    }
}

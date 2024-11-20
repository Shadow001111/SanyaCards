using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class RewindMono : MonoBehaviour
    {
        public static readonly float abilityCooldown = 15f;
        public static readonly float rewindDelay = 3f;

        Player player;
        float abilityUseTime;
        bool abilityActive = false;

        void Start()
        {
            abilityUseTime = Time.time;

            player = GetComponentInParent<Player>();
            player.data.block.BlockAction += OnBlock;
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

        void OnBlock(BlockTrigger.BlockTriggerType triggerType)
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

            Vector3 position = player.transform.position;

            yield return new WaitForSeconds(rewindDelay);

            player.GetComponent<PlayerCollision>().IgnoreWallForFrames(2);
            player.transform.position = position;

            abilityActive = false;
        }
    }
}

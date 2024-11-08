using System.Collections.Generic;
using UnityEngine;

namespace SanyaCards.Monos
{
    class TeamSpiritMono : MonoBehaviour
    {
        public static readonly float activationRange = 5f;

        public static readonly float regenerationEffect = 10f;
        public static readonly float damageEffect = 1f + 0.2f;

        Player player;

        List<Player> playersUnderEffect;

        bool isActive = false;

        void Start()
        {
            player = GetComponentInParent<Player>();

            playersUnderEffect = new List<Player>();
        }

        void OnDisable()
        {
            turnOff();
            foreach (Player teammate in playersUnderEffect)
            {
                turnOffAnother(teammate);
            }
        }

        void FixedUpdate()
        {
            int teammatesAround = 0;
            float playerRadius = getPlayerRadius(player);
            foreach (Player teammate in PlayerManager.instance.GetPlayersInTeam(player.teamID))
            {
                if (player.playerID == teammate.playerID)
                {
                    continue;
                }

                float distance = Vector3.Distance(player.transform.position, teammate.transform.position);
                if (distance - playerRadius - getPlayerRadius(teammate) < activationRange && 
                    PlayerManager.instance.CanSeePlayer(player.transform.position, teammate).canSee)
                {
                    turnOnAnother(teammate);
                    teammatesAround++;
                }
                else
                {
                    turnOffAnother(teammate);
                }
            }

            if (teammatesAround > 0)
            {
                turnOn();
            }
            else
            {
                turnOff();
            }
        }

        static float getPlayerRadius(Player player)
        {
            return player.GetComponent<CircleCollider2D>().bounds.extents.y;
        }

        void turnOn()
        {
            if (!isActive)
            {
                turnOnNoCheck(player);
                isActive = true;
            }
        }

        void turnOff()
        {
            if (isActive)
            {
                turnOffNoCheck(player);
                isActive = false;
            }
        }

        void turnOnAnother(Player player)
        {
            if (!playersUnderEffect.Contains(player))
            {
                turnOnNoCheck(player);
                playersUnderEffect.Add(player);
            }
        }

        void turnOffAnother(Player player)
        {
            if (playersUnderEffect.Contains(player))
            {
                turnOffNoCheck(player);
                playersUnderEffect.Remove(player);
            }
        }

        void turnOnNoCheck(Player player)
        {
            player.data.healthHandler.regeneration += regenerationEffect;
            player.data.weaponHandler.gun.damage *= damageEffect;
        }

        void turnOffNoCheck(Player player)
        {
            player.data.healthHandler.regeneration -= regenerationEffect;
            player.data.weaponHandler.gun.damage /= damageEffect;
        }
    }
}

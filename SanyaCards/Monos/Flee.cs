using ModdingUtils.AIMinion.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class FleeMono : MonoBehaviour
    {
        public static readonly float speedPercentBonus = 60.0f;

        //static FieldInfo particleField = typeof(TasteOfBlood).GetField("part", BindingFlags.NonPublic | BindingFlags.Instance);
        //static GameObject addObjectToPlayer = ((GameObject)Resources.Load("0 cards/TasteOfBlood")).GetComponent<CharacterStatModifiers>().AddObjectToPlayer;
        //ParticleSystem speedBoostParticle;

        Player player;
        CharacterStatModifiers stats;
        Player? target;

        bool isActive = false;

        void Start()
        {
            player = GetComponentInParent<Player>();
            stats = player.GetComponent<CharacterStatModifiers>();
            
            //TasteOfBlood tasteOfBloodComponent = addObjectToPlayer.GetComponent<TasteOfBlood>();
            //if (tasteOfBloodComponent == null)
            //{
            //    UnityEngine.Debug.Log("tasteOfBloodComponent is null");
            //}

            //speedBoostParticle = (ParticleSystem)particleField.GetValue(tasteOfBloodComponent);
            //if (speedBoostParticle == null)
            //{
            //    UnityEngine.Debug.Log("speedBoostParticle is null");
            //}
        }

        void OnDisable()
        {
            turnOff();
            target = null;
        }

        void Update()
        {
            // Find target
            target = PlayerManager.instance.GetClosestPlayerInOtherTeam(player.transform.position, player.teamID, true);
            if (target != null &&
                (player.data.input.direction == Vector3.zero || 
                Vector2.Angle(target.transform.position - player.transform.position, -player.data.input.direction) > 70f))
            {
                target = null;
            }


            if (target != null)
            {
                turnOn();
            }
            else
            {
                turnOff();
            }
        }

        void turnOn()
        {
            if (!isActive)
            {
                isActive = true;
                stats.movementSpeed *= (1.0f + speedPercentBonus * 0.01f);
                //speedBoostParticle.Play();
            }
        }

        void turnOff()
        {
            if (isActive)
            {
                isActive = false;
                stats.movementSpeed /= (1.0f + speedPercentBonus * 0.01f);
                //speedBoostParticle.Stop();
            }
        }
    }
}

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

        bool isActive = false;

        void Start()
        {
            player = GetComponentInParent<Player>();
            
            //TasteOfBlood tasteOfBloodComponent = addObjectToPlayer.GetComponentInChildren<TasteOfBlood>();
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
        }

        void FixedUpdate()
        {
            bool targetFound = false;
            if (player.data.input.direction != Vector3.zero)
            {
                foreach (Player other in PlayerManager.instance.players)
                {
                    if (other.teamID == player.teamID)
                    {
                        continue;
                    }
                    if (Vector2.Angle(other.transform.position - player.transform.position, -player.data.input.direction) > 70f)
                    {
                        continue;
                    }
                    if (!PlayerManager.instance.CanSeePlayer(player.transform.position, other).canSee)
                    {
                        continue;
                    }

                    targetFound = true;
                    break;
                }
            }

            if (targetFound)
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
                player.data.stats.movementSpeed *= (1.0f + speedPercentBonus * 0.01f);
                //speedBoostParticle.Play();
            }
        }

        void turnOff()
        {
            if (isActive)
            {
                isActive = false;
                player.data.stats.movementSpeed /= (1.0f + speedPercentBonus * 0.01f);
                //speedBoostParticle.Stop();
            }
        }
    }
}

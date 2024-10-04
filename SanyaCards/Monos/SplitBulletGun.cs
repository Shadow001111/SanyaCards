using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class SplitBulletGun : MonoBehaviour
    {
        private Gun gun;

        public void Fire(Vector3 position, Vector3 direction)
        {
            if (!(bool)typeof(Gun).InvokeMember("CheckIsMine", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, gun, new object[] { }))
            {
                return;
            }

            int currentNumberOfProjectiles = gun.numberOfProjectiles;
            for (int i = 0; i < gun.projectiles.Length; i++)
            {
                for (int j = 0; j < currentNumberOfProjectiles; j++)
                {
                    GameObject gameObject = PhotonNetwork.Instantiate(this.gun.projectiles[i].objectToSpawn.gameObject.name, position, getShootRotation(direction, j, currentNumberOfProjectiles));
                    if (PhotonNetwork.OfflineMode)
                    {
                        gameObject.GetComponent<ProjectileInit>().OFFLINE_Init(this.holdable.holder.player.playerID, currentNumberOfProjectiles, 1.0f, Random.Range(0f, 1f));
                    }
                    else
                    {
                        gun.gameObject.GetComponent<PhotonView>().RPC("RPCA_Init", RpcTarget.All, new object[]
                        {
                            this.holdable.holder.view.OwnerActorNr,
                            currentNumberOfProjectiles,
                            damageM,
                            Random.Range(0f, 1f)
                        });
                    }
                }
            }
        }

        private Quaternion getShootRotation(Vector3 direction, int bulletID, int numOfProj)
        {
            Vector3 vector = direction;
            float spread = UnityEngine.Random.Range(-gun.spread, gun.spread);
            spread /= (1f + gun.projectileSpeed * 0.5f) * 0.5f;
            float evenSpread = (numOfProj <= 1) ? 0f : ((float)bulletID * (gun.spread * 2f / (float)(numOfProj - 1)) - gun.spread);
            evenSpread /= (1f + gun.projectileSpeed * 0.5f) * 0.5f;
            spread = evenSpread + (1f - Mathf.Clamp(gun.evenSpread, 0f, 1f)) * (spread - evenSpread);
            vector += Vector3.Cross(vector, Vector3.forward) * spread * gun.multiplySpread;
            return Quaternion.LookRotation(vector);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards.Monos
{
    class DestroyCheck : MonoBehaviour
    {
        void OnDestroy()
        {
            UnityEngine.Debug.Log($"Object destroyed: {gameObject.name} {gameObject.GetInstanceID()}");
        }
    }
}

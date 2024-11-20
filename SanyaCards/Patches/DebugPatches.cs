using HarmonyLib;
using InControl;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace SanyaCards.Patches
{
    [HarmonyPatch(typeof(PlayerAssigner))]
    [HarmonyPatch("LateUpdate")]
    class PlayerAssignerPatch
    {
        static bool Prefix(PlayerAssigner __instance)
        {
            // The original LateUpdate logic goes here, replacing all original code
            var playersCanJoin = Traverse.Create(__instance).Field("playersCanJoin").GetValue<bool>();
            if (!playersCanJoin || __instance.players.Count >= __instance.maxPlayers || DevConsole.isTyping)
            {
                return false;  // Prevent the original method from running
            }

            if (Input.GetKeyDown(KeyCode.B) && !GameManager.lockInput)
            {
                __instance.StartCoroutine(__instance.CreatePlayer(null, isAI: true));
            }

            if (Input.GetKey(KeyCode.Space))
            {
                bool flag = true;
                for (int i = 0; i < __instance.players.Count; i++)
                {
                    var actions = __instance.players[i].playerActions;
                    if (actions == null || actions.Device == null)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    __instance.StartCoroutine(__instance.CreatePlayer(null));
                }
            }

            for (int j = 0; j < InputManager.ActiveDevices.Count; j++)
            {
                InputDevice inputDevice = InputManager.ActiveDevices[j];

                var joinButtonPressedMethod = typeof(PlayerAssigner).GetMethod("JoinButtonWasPressedOnDevice", BindingFlags.NonPublic | BindingFlags.Instance);
                var noPlayerUsingDeviceMethod = typeof(PlayerAssigner).GetMethod("ThereIsNoPlayerUsingDevice", BindingFlags.NonPublic | BindingFlags.Instance);

                bool joinButtonPressed = (bool)joinButtonPressedMethod.Invoke(__instance, new object[] { inputDevice });
                bool noPlayerUsingDevice = (bool)noPlayerUsingDeviceMethod.Invoke(__instance, new object[] { inputDevice });

                if (joinButtonPressed && noPlayerUsingDevice)
                {
                    __instance.StartCoroutine(__instance.CreatePlayer(inputDevice));
                }
            }

            return false;
        }
    }
}

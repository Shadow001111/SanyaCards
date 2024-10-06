using HarmonyLib;
using InControl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SanyaCards.Patches
{
    [HarmonyPatch(typeof(PlayerAssigner), "LateUpdate")]
    public static class PlayerAssignerPatch
    {
        static bool Prefix(PlayerAssigner __instance)
        {
            FieldInfo playersCanJoinField = typeof(PlayerAssigner).GetField("playersCanJoin", BindingFlags.NonPublic | BindingFlags.Instance);
            bool playersCanJoin = (bool)playersCanJoinField.GetValue(__instance);
            if (!playersCanJoin)
            {
                return false;
            }
            if (__instance.players.Count >= __instance.maxPlayers)
            {
                return false;
            }
            if (DevConsole.isTyping)
            {
                return false;
            }
            if (Input.GetKeyDown(KeyCode.B) && !GameManager.lockInput)
            {
                __instance.StartCoroutine(__instance.CreatePlayer(null, true));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                bool flag = true;
                for (int i = 0; i < __instance.players.Count; i++)
                {
                    if (__instance.players[i].playerActions == null)
                    {
                        flag = false;
                        break;
                    }
                    if (__instance.players[i].playerActions.Device == null)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    __instance.StartCoroutine(__instance.CreatePlayer(null, false));
                }
            }

            MethodInfo joinButtonMethod = typeof(PlayerAssigner).GetMethod("JoinButtonWasPressedOnDevice", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo noPlayerUsingDeviceMethod = typeof(PlayerAssigner).GetMethod("ThereIsNoPlayerUsingDevice", BindingFlags.NonPublic | BindingFlags.Instance);
            for (int j = 0; j < InputManager.ActiveDevices.Count; j++)
            {
                InputDevice inputDevice = InputManager.ActiveDevices[j];
                bool buttonPressed = (bool)joinButtonMethod.Invoke(__instance, new object[] { inputDevice });
                bool noPlayerUsingDevice = (bool)noPlayerUsingDeviceMethod.Invoke(__instance, new object[] { inputDevice });
                if (buttonPressed && noPlayerUsingDevice)
                {
                    __instance.StartCoroutine(__instance.CreatePlayer(inputDevice, false));
                }
            }
            return false;
        }
    }
}

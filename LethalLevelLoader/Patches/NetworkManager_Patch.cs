﻿using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using Unity.Netcode;

namespace LethalLevelLoader
{
    public class NetworkManager_Patch
    {
        private static List<GameObject> queuedNetworkPrefabs = new List<GameObject>();
        public static bool networkHasStarted;

        public static void RegisterNetworkPrefab(GameObject prefab)
        {
            if (networkHasStarted == false)
                queuedNetworkPrefabs.Add(prefab);
            else
                DebugHelper.Log("Attempted To Register NetworkPrefab: " + prefab + " After GameNetworkManager Has Started!");
        }

        internal static List<string> loggedObjectNames = new List<string>();
        public static void TryRestoreVanillaSpawnSyncPrefab(SpawnSyncedObject spawnSyncedObject)
        {
            NetworkManager networkManager = UnityEngine.Object.FindObjectOfType<NetworkManager>();

            if (loggedObjectNames == null)
                loggedObjectNames = new List<string>();

            if (networkManager != null && spawnSyncedObject != null && spawnSyncedObject.spawnPrefab != null)
                    foreach (NetworkPrefab networkPrefab in networkManager.NetworkConfig.Prefabs.m_Prefabs)
                        if (networkPrefab.Prefab.name == spawnSyncedObject.spawnPrefab.name)
                        {
                            if (!loggedObjectNames.Contains(networkPrefab.Prefab.name))
                            {
                                DebugHelper.Log("Succesfully Restored " + spawnSyncedObject.name + " NetworkPrefab From " + spawnSyncedObject.spawnPrefab.name + " To " + networkPrefab.Prefab.name);
                                loggedObjectNames.Add(networkPrefab.Prefab.name);
                            }
                            spawnSyncedObject.spawnPrefab = networkPrefab.Prefab;
                            break;
                        }
        }

        [HarmonyPatch(typeof(GameNetworkManager), "Start")]
        [HarmonyPrefix]
        [HarmonyPriority(350)]
        internal static void GameNetworkManager_Start(GameNetworkManager __instance)
        {
            //DebugHelper.Log("Game NetworkManager Start");

            NetworkManager networkManager = __instance.GetComponent<NetworkManager>();

            List<GameObject> addedNetworkPrefabs = new List<GameObject>();

            foreach (NetworkPrefab networkPrefab in networkManager.NetworkConfig.Prefabs.m_Prefabs)
                addedNetworkPrefabs.Add(networkPrefab.Prefab);

            int debugCounter = 0;

            foreach (GameObject queuedNetworkPrefab in queuedNetworkPrefabs)
            {
                if (!addedNetworkPrefabs.Contains(queuedNetworkPrefab))
                {
                    DebugHelper.Log("Trying To Register Prefab: " + queuedNetworkPrefab);
                    networkManager.AddNetworkPrefab(queuedNetworkPrefab);
                    addedNetworkPrefabs.Add(queuedNetworkPrefab);
                }
                else
                    debugCounter++;
            }

            DebugHelper.Log("Skipped Registering " + debugCounter + " NetworkObjects As They Were Already Registered.");

            networkHasStarted = true;
            
        }
    }
}

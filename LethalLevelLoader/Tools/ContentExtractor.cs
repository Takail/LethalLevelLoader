﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace LethalLevelLoader
{
    public class ContentExtractor
    {
        public static List<Item> vanillaItemsList = new List<Item>();
        public static List<EnemyType> vanillaEnemiesList = new List<EnemyType>();
        public static List<SpawnableOutsideObject> vanillaSpawnableOutsideMapObjectsList = new List<SpawnableOutsideObject>();
        public static List<GameObject> vanillaSpawnableInsideMapObjectsList = new List<GameObject>();
        public static List<LevelAmbienceLibrary> vanillaAmbienceLibrariesList = new List<LevelAmbienceLibrary>();
        public static List<AudioMixerGroup> vanillaAudioMixerGroupsList = new List<AudioMixerGroup>();


        [HarmonyPatch(typeof(RoundManager), "Awake")]
        [HarmonyPrefix]
        [HarmonyPriority(350)]
        internal static void TryScrapeVanillaContent()
        {
            if (LethalLevelLoaderPlugin.hasVanillaBeenPatched == false)
            {
                StartOfRound startOfRound = StartOfRound.Instance;
                if (startOfRound != null)
                {
                    foreach (Item item in startOfRound.allItemsList.itemsList)
                    {
                        if (!vanillaItemsList.Contains(item))
                            vanillaItemsList.Add(item);

                        if (item.spawnPrefab != null)
                            TryExtractAudioMixerGroups(item.spawnPrefab.GetComponentsInChildren<AudioSource>());
                    }

                    foreach (SelectableLevel selectableLevel in startOfRound.levels)
                    {
                        foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.Enemies)
                            if (!vanillaEnemiesList.Contains(enemyWithRarity.enemyType))
                                vanillaEnemiesList.Add(enemyWithRarity.enemyType);

                        foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.OutsideEnemies)
                            if (!vanillaEnemiesList.Contains(enemyWithRarity.enemyType))
                                vanillaEnemiesList.Add(enemyWithRarity.enemyType);

                        foreach (SpawnableEnemyWithRarity enemyWithRarity in selectableLevel.DaytimeEnemies)
                            if (!vanillaEnemiesList.Contains(enemyWithRarity.enemyType))
                                vanillaEnemiesList.Add(enemyWithRarity.enemyType);

                        foreach (SpawnableMapObject spawnableInsideObject in selectableLevel.spawnableMapObjects)
                            if (!vanillaSpawnableInsideMapObjectsList.Contains(spawnableInsideObject.prefabToSpawn))
                                vanillaSpawnableInsideMapObjectsList.Add(spawnableInsideObject.prefabToSpawn);


                        foreach (SpawnableOutsideObjectWithRarity spawnableOutsideObject in selectableLevel.spawnableOutsideObjects)
                            if (!vanillaSpawnableOutsideMapObjectsList.Contains(spawnableOutsideObject.spawnableObject))
                                vanillaSpawnableOutsideMapObjectsList.Add(spawnableOutsideObject.spawnableObject);

                        if (!vanillaAmbienceLibrariesList.Contains(selectableLevel.levelAmbienceClips))
                            vanillaAmbienceLibrariesList.Add(selectableLevel.levelAmbienceClips);
                    }
                }

            }
            DebugHelper.DebugScrapedVanillaContent();
        }

        internal static void TryExtractAudioMixerGroups(AudioSource[] audioSources)
        {
            foreach (AudioSource audioSource in audioSources)
                if (audioSource.outputAudioMixerGroup != null && !vanillaAudioMixerGroupsList.Contains(audioSource.outputAudioMixerGroup))
                {
                    vanillaAudioMixerGroupsList.Add(audioSource.outputAudioMixerGroup);
                    DebugHelper.Log("Adding AudioMixerGroup: " + audioSource.outputAudioMixerGroup.name + " To Vanilla Reference List!");
                }
        }
    }
}

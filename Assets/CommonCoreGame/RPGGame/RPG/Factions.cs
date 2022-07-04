﻿using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore.DebugLog;

namespace CommonCore.RpgGame.Rpg
{
    public enum PredefinedFaction
    {
        None = 0, Chaotic = 1, Player, Neutral, Monster
    }

    public enum FactionRelationStatus
    {
        Neutral = 0, Friendly, Hostile
    }

    public class FactionModel
    {
        //relationships are defined as self->target
        private static Dictionary<string, Dictionary<string, FactionRelationStatus>> FactionTable;

        internal static void Load()
        {
            FactionTable = new Dictionary<string, Dictionary<string, FactionRelationStatus>>();

            //load predefined factions
            //difference between None and Neutral is that Monsters will attack Neutral
            //None is a true non-alignment, whereas Neutral is meant for non-hostile NPCs and such
            FactionTable.Add("None", new Dictionary<string, FactionRelationStatus>());
            FactionTable.Add("Player", new Dictionary<string, FactionRelationStatus>() {
                { "Monster", FactionRelationStatus.Hostile}
            });
            FactionTable.Add("Neutral", new Dictionary<string, FactionRelationStatus>());
            FactionTable.Add("Monster", new Dictionary<string, FactionRelationStatus>() {
                { "Neutral", FactionRelationStatus.Hostile },
                { "Player", FactionRelationStatus.Hostile }
            });

            //load factions from defs
            try
            {
                TextAsset ta = CoreUtils.LoadResource<TextAsset>("Data/RPGDefs/factions");
                LoadFactionDataFromAsset(ta);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load faction defs!");
                Debug.LogError(e);
            }
        }

        internal static void LoadFromAddon(AddonLoadData data)
        {
            if(data.LoadedResources != null && data.LoadedResources.Count > 0)
            {
                if(data.LoadedResources.TryGetValue("Data/RPGDefs/factions", out var rh))
                {
                    if(rh.Resource is TextAsset ta)
                    {
                        LoadFactionDataFromAsset(ta);
                        Debug.Log("Loaded faction data from addon!");
                    }
                }
            }
        }

        private static void LoadFactionDataFromAsset(TextAsset ta)
        {
            var newFactions = CoreUtils.LoadJson<Dictionary<string, Dictionary<string, FactionRelationStatus>>>(ta.text);

            foreach (var row in newFactions)
            {
                if (FactionTable.ContainsKey(row.Key))
                {
                    var destRow = FactionTable[row.Key];
                    foreach (var entry in row.Value)
                    {
                        destRow[entry.Key] = entry.Value;
                    }
                }
                else
                {
                    FactionTable.Add(row.Key, row.Value);
                }
            }
        }        

        public static FactionRelationStatus GetRelation(string self, string target)
        {
            //a few simple rules:
            // -factions are always friendly with themselvs
            // -factions are always neutral toward factions they have no entry for
            // -factions are always neutral toward "None"
            // -factions are always hostile toward "Chaotic"
            // -otherwise, it's a lookup

            if (self == target)
                return FactionRelationStatus.Friendly;

            if (self == "None" || target == "None")
                return FactionRelationStatus.Neutral;

            if (self == "Chaotic" || target == "Chaotic")
                return FactionRelationStatus.Hostile;

            var selfEntry = FactionTable.GetOrDefault(self);
            if(selfEntry != null)
            {
                return selfEntry.GetOrDefault(target, FactionRelationStatus.Neutral);
            }
            else
            {
                //no entry in table for our faction
                return FactionRelationStatus.Neutral;
            }
        }

        public static string GetFactionsList()
        {
            StringBuilder sb = new StringBuilder(FactionTable.Count * 100);

            foreach(var row in FactionTable)
            {
                sb.AppendLine(FactionTableRowToString(row));
            }

            return sb.ToString();
        }

        private static string FactionTableRowToString(KeyValuePair<string, Dictionary<string, FactionRelationStatus>> row)
        {
            StringBuilder sb = new StringBuilder(row.Key.Length * 32);

            sb.AppendFormat("{0} : ", row.Key);

            foreach(var entry in row.Value)
            {
                sb.AppendFormat("[{0}: {1}] ", entry.Key, entry.Value.ToString());
            }

            return sb.ToString();
        }
    }

}
using CommonCore.State;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vail
{

    public class VailScore
    {
        [JsonIgnore]
        public static readonly string ObjectName = "VailScore";

        [JsonProperty] //unneeded?
        public int Kills { get; set; }
        [JsonProperty]
        public int Mercies { get; set; }

    }

    public static class VailScoreExtensions
    {
        public static VailScore GetScoreObject(this GameState gameState)
        {
            if (gameState.GlobalDataState.TryGetValue(VailScore.ObjectName, out var obj) && obj is VailScore vs)
            {
                return vs;
            }

            VailScore vailScore = new VailScore();
            gameState.GlobalDataState[VailScore.ObjectName] = vailScore;
            return vailScore;
        }
    }
}
using CommonCore.State;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Vail
{

    [JsonConverter(typeof(MutatorBlockConverter)), JsonObject]
    public class MutatorBlock : IReadOnlyList<float>
    {
        [JsonIgnore]
        public static readonly string ObjectName = "VailMutatorBlock";
        private const int BlockSize = 256;

        [JsonProperty]
        private float[] BlockValues;               

        public MutatorBlock()
        {
            BlockValues = new float[BlockSize];
        }

        public void Regenerate()
        {
            System.Random random = new System.Random();
            for(int i = 0; i < BlockValues.Length; i++)
            {
                BlockValues[i] = (float)random.NextDouble();
            }
        }

        public int Count => BlockValues.Length;        

        [JsonIgnore]
        public float this[int i] => BlockValues[i];

        public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)BlockValues).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

    public static class MutatorBlockExtensions
    {
        public static MutatorBlock GetMutatorBlock(this GameState gameState)
        {
            if(gameState == null || gameState.GlobalDataState == null)
            {
                Debug.LogWarning("GetMutatorBlock called without valid GameState, returning dummy MutatorBlock");
                return new MutatorBlock();
            }

            if (gameState.GlobalDataState.TryGetValue(MutatorBlock.ObjectName, out var obj) && obj is MutatorBlock mb)
            {
                return mb;
            }

            Debug.LogWarning("Regenerating MutatorBlock in GetMutatorBlock, should this really be happening?");

            var mutatorBlock = new MutatorBlock();

            mutatorBlock.Regenerate();
            gameState.GlobalDataState[MutatorBlock.ObjectName] = mutatorBlock;

            return mutatorBlock;
        }
    }

    public class RegenerateMutatorBlockIntent : Intent
    {
        public override void LoadingExecute()
        {
            if (!Valid)
                return;

            MutatorBlock mutatorBlock;
            if(GameState.Instance.GlobalDataState.TryGetValue(MutatorBlock.ObjectName, out var obj) && obj is MutatorBlock mb)
            {
                mutatorBlock = mb;
            }
            else
            {
                mutatorBlock = new MutatorBlock();
                GameState.Instance.GlobalDataState[MutatorBlock.ObjectName] = mutatorBlock;
            }

            mutatorBlock.Regenerate();

            Valid = false;
        }
    }

    //yeah it doesn't work by default, probably shitting the bed on IReadOnlyList
    public class MutatorBlockConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MutatorBlock).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            MutatorBlock mutatorBlock;
            if (serializer.ObjectCreationHandling != ObjectCreationHandling.Replace && existingValue != null && existingValue is MutatorBlock existingMutatorBlock)
                mutatorBlock = existingMutatorBlock;
            else
                mutatorBlock = new MutatorBlock();

            JObject jsonObject = JObject.Load(reader);
            JArray blockValuesJsonArray = jsonObject["BlockValues"] as JArray;

            float[] blockValues = new float[blockValuesJsonArray.Count];
            for(int i = 0; i < blockValues.Length; i++)
            {
                blockValues[i] = blockValuesJsonArray[i].Value<float>();
            }

            var field = mutatorBlock.GetType().GetField("BlockValues", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(mutatorBlock, blockValues);

            return mutatorBlock;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var mutatorBlock = value as MutatorBlock;
            writer.WriteStartObject();
            if (serializer.TypeNameHandling != TypeNameHandling.None)
            {
                writer.WritePropertyName("$type");
                writer.WriteValue(string.Format("{0}, {1}", value.GetType().ToString(), value.GetType().Assembly.GetName().Name));
            }
            writer.WritePropertyName("BlockValues");
            writer.WriteStartArray();
            foreach (float blockValue in mutatorBlock)
                writer.WriteValue(blockValue);
            writer.WriteEndArray();
            writer.WriteEndObject();
            
        }
    }


}

using CommonCore;
using CommonCore.Config;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vail
{

    public class VailParams
    {

        //world randomization options
        [JsonProperty]
        public float MinWorldScale { get; private set; } = 0.5f;
        [JsonProperty]
        public float MaxWorldScale { get; private set; } = 2.0f;
        [JsonProperty]
        public float MaxSpawnDisplacement { get; private set; } = 0.5f;
        [JsonProperty]
        public float MinRandoHealthFactor { get; private set; } = 0.5f;
        [JsonProperty]
        public float MaxRandoHealthFactor { get; private set; } = 2.0f;
        [JsonProperty]
        public float MinRandoScale { get; private set; } = 0.8f;
        [JsonProperty]
        public float MaxRandoScale { get; private set; } = 1.2f;

        //intro options
        [JsonProperty]
        public float IntroSlide1Time { get; private set; } = 14f;
        [JsonProperty]
        public float IntroSlide2Time { get; private set; } = 5f;
        [JsonProperty]
        public string IntroMusic { get; private set; } = "nitemare";

        //misc game options
        [JsonProperty]
        public string MainMusic { get; private set; } = "mutaland";
        [JsonProperty]
        public bool MutateMusicSpeed { get; private set; } = true;
        [JsonProperty]
        public float MutateMusicSpeedMin { get; private set; } = 0.25f;
        [JsonProperty]
        public float MutateMusicSpeedMax { get; private set; } = 3f;

        //ending options
        [JsonProperty]
        public float FadeoutTime { get; private set; } = 2f;
        [JsonProperty]
        public string SecretEndingScene { get; private set; } = "VailEndingScene";
        [JsonProperty]
        public float SecretEndingTypeonRate { get; private set; } = 0.1f;
        [JsonProperty]
        public float SecretEndingTypeonDelay { get; private set; } = 0.5f;
        [JsonProperty]
        public string SecretEndingMusic { get; private set; } = "oblivion";
        [JsonProperty]
        public string ChessGamePath { get; private set; } = "<%StreamingAssets%>/chess/DreamChess.exe";

        //splash/menu options
        [JsonProperty]
        public bool EnableMenuSplash { get; private set; } = true;
        [JsonProperty]
        public bool EnableMenuSplashAnimation { get; private set; } = true;
        [JsonProperty]
        public float MenuSplashAnimationSpeed { get; private set; } = 0.1f;
        [JsonProperty]
        public float MenuSplashAnimationScale { get; private set; } = 1.1f;
        [JsonProperty]
        public StringCase MenuSplashCase { get; private set; } = StringCase.UpperCase;

        //secret/bonus options
        [JsonProperty]
        public bool EnableMenuBonus { get; private set; } = true;
        [JsonProperty]
        public float MenuBonusDelay { get; private set; } = 1000 * 60 * 60; //in ms //1000 * 60 * 60
        [JsonProperty]
        public string MenuBonusString { get; private set; } = "yuo forget licens RENEWEAL";

        [JsonIgnore]
        private const string ResourcePath = "Data/Vail/VailParams";

        [JsonIgnore]
        private static VailParams _Instance;

        [JsonIgnore]
        public static VailParams Instance
        {
            get
            {
                if(_Instance == null)
                {
                    Debug.Log("[VailParams] Loading params");

                    _Instance = new VailParams();

                    try
                    {
                        if (CoreUtils.CheckResource<TextAsset>(ResourcePath))
                        {

                            var resource = CoreUtils.LoadResource<TextAsset>(ResourcePath);
                            //should we use PopulateObject instead?
                            JsonConvert.PopulateObject(resource.text, _Instance, new JsonSerializerSettings
                            {
                                Converters = CCJsonConverters.Defaults.Converters,
                                TypeNameHandling = TypeNameHandling.Auto,
                                NullValueHandling = NullValueHandling.Ignore
                            });
                            //_Instance = CoreUtils.LoadJson<VailParams>(resource.text);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogError($"[VailParams] Attempted to load params and got a {e.GetType().Name}");
                        if(ConfigState.Instance.UseVerboseLogging)
                            Debug.LogException(e);
                    }

                }

                return _Instance;
            }
        }

        public static void ClearInstance()
        {
            _Instance = null;
        }

        private VailParams()
        {

        }


    }
}
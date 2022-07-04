using CommonCore;
using CommonCore.Audio;
using CommonCore.Scripting;
using CommonCore.State;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Vail
{
    //are we just going to modify a property on an audiosource we've dragged in in the inspector? fuck no!
    public static class MusicSpeedMutator
    {
        [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.AfterSceneLoad)]
        public static void OnSceneLoaded(ScriptExecutionContext context)
        {
            var vParams = VailParams.Instance;
            if (!vParams.MutateMusicSpeed)
                return;

            if (!(context.Caller is WorldSceneController)) //we should probably check the scene name but eh...
                return;

            Debug.Log("[MusicSpeedMutator] Mutating music speed!");

            var mutatorBlock = GameState.Instance.GetMutatorBlock();
            float rawSpeed = mutatorBlock[31];
            float pitch = MathUtils.ScaleRange(rawSpeed, 0, 1, vParams.MutateMusicSpeedMin, vParams.MutateMusicSpeedMax);

            var audioSource = GetAudioSource();
            audioSource.pitch = pitch;
            Debug.Log("[MusicSpeedMutator] Music pitch set to " + pitch);
        }

        [CCScript, CCScriptHook(AllowExplicitCalls = false, Hook = ScriptHook.OnSceneUnload)]
        public static void OnSceneUnloaded(ScriptExecutionContext context)
        {
            Debug.Log("[MusicSpeedMutator] Unmutating music speed!");

            var audioSource = GetAudioSource();
            audioSource.pitch = 1;
        }

        private static AudioSource GetAudioSource()
        {
            var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;
            object mpField = audioPlayer.GetType().GetField("MusicPlayers", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(audioPlayer);
            var musicPlayers = mpField as Dictionary<MusicSlot, AudioSource>;
            return musicPlayers[MusicSlot.Ambient];
        }
    }
}
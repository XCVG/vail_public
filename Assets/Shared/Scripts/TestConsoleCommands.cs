﻿using CommonCore;
using CommonCore.Async;
using CommonCore.Audio;
using CommonCore.DebugLog;
using CommonCore.LockPause;
using CommonCore.State;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Vail;

/// <summary>
/// Temporary "test" console commands for trying out various things
/// </summary>
public static class TestConsoleCommands
{
    [Command]
    public static void CCTestCommand()
    {
        Debug.Log("hello world");
    }

    [Command]
    public static void CCTestArgCommand(string mystring)
    {
        Debug.Log(mystring);
    }

    [Command]
    public static void DumpQualitySettings()
    {
        Dictionary<string, Dictionary<string, object>> allQSettings = new Dictionary<string, Dictionary<string, object>>();

        for(int i = 0; i < QualitySettings.names.Length; i++)
        {
            QualitySettings.SetQualityLevel(i, true);
            Dictionary<string, object> qualitySettings = new Dictionary<string, object>();
            var props = typeof(QualitySettings).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach(var prop in props)
            {
                qualitySettings.Add(prop.Name, prop.GetValue(null));
            }
            allQSettings.Add(i.ToString(), qualitySettings);
        }

        DebugUtils.JsonWrite(allQSettings, "qualitysettings");
    }

    [Command]
    public static void TestWeakReferencePauseLock()
    {
        AsyncUtils.RunWithExceptionHandling(async () =>
        {
            object lockObject = new object();
            WeakReference untypedWeakReference = new WeakReference(lockObject);
            LockPauseModule.PauseGame(PauseLockType.AllowMenu, untypedWeakReference);
            Debug.Log("Untyped lock added!");

            await Task.Delay(3000);
            AsyncUtils.ThrowIfEditorStopped();

            lockObject = null;
            GC.Collect();

            Debug.Log("Untyped lock released!");

            await Task.Yield();
            AsyncUtils.ThrowIfEditorStopped();

            LockPauseModule.ForceCleanLocks();

            Debug.Log("Locks cleaned!");

            await Task.Delay(5000);
            AsyncUtils.ThrowIfEditorStopped();

            IEnumerable typedLockObject = new string[] { "lol", "hi" };
            WeakReference<IEnumerable> typedWeakReference = new WeakReference<IEnumerable>(typedLockObject);

            LockPauseModule.PauseGame(PauseLockType.AllowMenu, typedWeakReference);
            Debug.Log("Typed lock added!");

            await Task.Delay(3000);
            AsyncUtils.ThrowIfEditorStopped();

            typedLockObject = null;
            GC.Collect();

            Debug.Log("Typed lock released!");

            await Task.Yield();
            AsyncUtils.ThrowIfEditorStopped();

            LockPauseModule.ForceCleanLocks();

            Debug.Log("Locks cleaned!");
        });
    }

    [Command]
    public static void TestPlayMusic()
    {
        var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;
        audioPlayer.PlayMusic("menu", MusicSlot.Override, 1.0f, true, false);
    }

    [Command]
    public static void DumpVailParams()
    {
        DebugUtils.JsonWrite(VailParams.Instance, "VailParams");
    }

    [Command]
    public static void InvalidateVailParams()
    {
        VailParams.ClearInstance();
    }

    [Command]
    public static void LoadVailSave(string saveName)
    {
        string path = Path.Combine(CoreParams.SavePath, saveName + ".json");
        string data = File.ReadAllText(path);
        var instance = JsonConvert.DeserializeObject<GameState>(data,
            new JsonSerializerSettings
            {
                Converters = CCJsonConverters.Defaults.Converters.Concat(new JsonConverter[] { new MutatorBlockConverter() }).ToList(),
                TypeNameHandling = TypeNameHandling.Auto
            });
        DebugUtils.JsonWrite(instance, "saveloadtest");
    }

}

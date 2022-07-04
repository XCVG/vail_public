﻿using CommonCore;
using CommonCore.StringSub;
using System;

/// <summary>
/// Module for setting the title of the game window
/// </summary>
/// <remarks>
/// <para>Currently only works for Windows, via win32 API</para>
/// </remarks>
public class WindowTitleModule : CCModule
{
    /// <summary>
    /// If set, will automatically set the window title to the default (specified below) on startup
    /// </summary>
    private static readonly bool AutosetWindowTitle = true;
    /// <summary>
    /// The "default" window title for autoset
    /// </summary>
    private static readonly string DefaultWindowTitle = "Return To Reality";
    /// <summary>
    /// If set, will use IGUI->WindowTitle from the string lookup and will ignore DefaultWindowTitle if it exists
    /// </summary>
    private static readonly bool LookupWindowTitle = true;

    public WindowTitleModule()
    {
        
    }

    public override void OnAllModulesLoaded()
    {
        if (AutosetWindowTitle)
        {
            try
            {                
                SetWindowTitle((LookupWindowTitle && Sub.Exists("WindowTitle", "IGUI")) ? Sub.Replace("WindowTitle", "IGUI") : DefaultWindowTitle);
            }
            catch
            {

            }
        }
    }

    public void SetWindowTitle(string windowTitle)
    {
        try
        {
#if UNITY_STANDALONE_WIN
            WindowsWindowTitleSetter.SetWindowTitle(windowTitle);
#elif UNITY_WSA && ENABLE_WINMD_SUPPORT
            UWPWindowTitleSetter.SetWindowTitle(windowTitle);
#else
        throw new NotSupportedException("WindowTitleModule does not support setting the window title on this platform!");
#endif
        }
        catch (Exception e)
        {
            LogError($"Failed to set the window title ({e.GetType().Name})");
            LogException(e);
            throw e;
        }

        Log($"Set window title to {windowTitle}");
    }
}

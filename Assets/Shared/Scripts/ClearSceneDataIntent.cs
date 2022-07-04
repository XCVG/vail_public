using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//will likely move this into mainline
namespace Vail
{

    public class ClearSceneDataIntent : Intent
    {
        private string SceneName { get; set; }

        public ClearSceneDataIntent(string sceneName)
        {
            SceneName = sceneName;
        }

        public override void LoadingExecute()
        {
            if (!Valid)
                return;

            GameState.Instance.LocalDataState.Remove(SceneName);
            GameState.Instance.LocalObjectState.Remove(SceneName);
            //reset player location state
            GameState.Instance.PlayerWorldState = null;

            Valid = false;
        }
    }
}
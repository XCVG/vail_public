using CommonCore;
using CommonCore.Audio;
using CommonCore.State;
using CommonCore.StringSub;
using CommonCore.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vail
{

    public class VailEndingSequenceScript : MonoBehaviour
    {
        [SerializeField, Header("UI Elements")]
        private Text[] TextFields = null;
        [SerializeField]
        private Text StatsTextField = null;

        private Coroutine EndingCoroutine = null;
        
        private void Start()
        {
            GameState.Instance.ManualSaveLocked = true;

            //play sequence etc
            EndingCoroutine = StartCoroutine(CoEnding());
        }

        private IEnumerator CoEnding()
        {
            //get and clear text
            string[] textStrings = new string[TextFields.Length];
            for (int i = 0; i < TextFields.Length; i++)
            {
                textStrings[i] = TextFields[i].text;
                TextFields[i].text = string.Empty;
            }

            var vParams = VailParams.Instance;
            var vScore = GameState.Instance.GetScoreObject();

            CCBase.GetModule<AudioModule>().AudioPlayer.PlayMusic(vParams.SecretEndingMusic, MusicSlot.Ambient, 1.0f, true, false);

            //write kill count etc
            string statsText = $"ROUNDS: {(vScore.Kills + vScore.Mercies + 1)}\tKILLS: {vScore.Kills}\tMERCIES: {vScore.Mercies}";
            float statsTextTime = statsText.Length * vParams.SecretEndingTypeonRate;
            TextAnimation.TypeOn(StatsTextField, statsText, statsTextTime);
            yield return new WaitForSecondsRealtime(statsTextTime);

            yield return new WaitForSecondsRealtime(vParams.SecretEndingTypeonDelay);
            yield return null;

            for (int i = 0; i < TextFields.Length; i++)
            {
                string str = textStrings[i];
                float time = str.Length * vParams.SecretEndingTypeonRate; //not 100% because of code points but close enough for golf
                TextAnimation.TypeOn(TextFields[i], str, time);
                yield return new WaitForSecondsRealtime(time + vParams.SecretEndingTypeonDelay); //realtime?
            }

            yield return null;

            EndingCoroutine = null;

        }

        public void HandleChessTextClicked()
        {
            if (EndingCoroutine != null)
                return; //type on isn't finished!

            //Debug.Log("Clicked chess text!");

            //launch chess!
            string processPath = Sub.Macro(VailParams.Instance.ChessGamePath);
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = processPath;
            process.StartInfo.UseShellExecute = true;
            bool worked = process.Start();

            if (worked)
                MetaState.Instance.SessionFlags.Add("ChessStarted");
        }
    }
}
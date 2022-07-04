using CommonCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vail
{

    public class MainMenuBonusScript : MonoBehaviour
    {
        [SerializeField]
        private int MaxCharacters = 256;

        [SerializeField]
        private GameObject RootObject = null;
        [SerializeField]
        private Text TextField = null;

        private float Delay = 0;

        private System.Random Generator = new System.Random();
        private float Elapsed = 0;
        private bool Triggered = false;
        private bool Aborted = false;

        private void Start()
        {
            if (!VailParams.Instance.EnableMenuBonus)
            {
                enabled = false;
                return;
            }

            Delay = VailParams.Instance.MenuBonusDelay / 1000f;
        }

        private void Update()
        {
            if (Aborted || Delay == 0)
                return;

            if(Triggered)
            {
                StepSequence();
            }
            else
            {
                Elapsed += Time.deltaTime;
                if (Elapsed >= Delay)
                    StartSequence();
            }
        }

        private void StartSequence()
        {
            Triggered = true;
            RootObject.SetActive(true);
            TextField.text = string.Empty;
        }

        private void StepSequence()
        {
            //bugged AF and iunno why

            if (TextField.text.Length >= MaxCharacters)
                TextField.text = string.Empty;

            //char nextChar = (char)(byte)Generator.Next(); //icky but eh
            //string nextChunk = nextChar.ToString();
            string nextChunk = GetRandomStringChunk(1); //less buggy believe it or not
            TextField.text = TextField.text + nextChunk; //will generate shedloads of garbage

            int halfPoint = System.Math.Max(1, MaxCharacters / 2);
            if(TextField.text.Length >= halfPoint && TextField.text.Length < halfPoint + 1)
            {
                TextField.text = TextField.text + VailParams.Instance.MenuBonusString;
            }
        }

        private string GetRandomStringChunk(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] outChars = new char[length];
            for (int i = 0; i < outChars.Length; i++)
            {
                outChars[i] = chars[Generator.Next(chars.Length)];
            }
            return new string(outChars);
        }

        [Command(alias = "ForceSecret", className = "VailMenu", useClassName = true)]
        private static void ForceMainMenuSecret()
        {
            MainMenuBonusScript script = CoreUtils.GetUIRoot().GetComponentInChildren<MainMenuBonusScript>(true);
            script.Aborted = false;
            script.StartSequence();
            //System.GC.Collect();
            Debug.Log("Forced main menu secret!");
        }

        [Command(alias = "ClearSecret", className = "VailMenu", useClassName = true)]
        private static void ClearMainMenuSecret()
        {
            MainMenuBonusScript script = CoreUtils.GetUIRoot().GetComponentInChildren<MainMenuBonusScript>(true);
            script.Aborted = true;
            script.RootObject.SetActive(false);
            //System.GC.Collect();
            Debug.Log("Aborted main menu secret!");
        }
    }
}
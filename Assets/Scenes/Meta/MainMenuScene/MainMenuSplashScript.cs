using CommonCore;
using CommonCore.DebugLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Vail
{

    /// <summary>
    /// Controls the Minecraft-style splash text on the main menu
    /// </summary>
    public class MainMenuSplashScript : MonoBehaviour
    {
        [SerializeField]
        private Text SplashText = null;



        private float AnimateSpeed = 1f; //in scale units per second
        private float AnimateMaxScale = 1f;

        private RectTransform SplashTransform;
        private VailParams Params;

        private bool ForceEnabled = false;
        private float CurrentScale = 1;
        private int AnimationDirection = 1;

        private void Start()
        {
            Params = VailParams.Instance;

            if(!Params.EnableMenuSplash && !ForceEnabled)
            {
                enabled = false;
                return;
            }


            SplashTransform = (RectTransform)SplashText.transform;
            CurrentScale = SplashTransform.localScale.x;
            PaintSplash();
        }

        private void PaintSplash()
        {
            string chosenText = PickSplash(); // = "Lorem ipsum dolor sit amet. 123456789ABCDEF";
            switch (Params.MenuSplashCase)
            {
                case StringCase.Unspecified:
                    break;
                case StringCase.LowerCase:
                    chosenText = chosenText.ToLower(CultureInfo.InvariantCulture);
                    break;
                case StringCase.UpperCase:
                    chosenText = chosenText.ToUpper(CultureInfo.InvariantCulture);
                    break;
                default:
                    Debug.LogWarning("[MainMenuSplashScript] Unsupported case " + Params.MenuSplashCase);
                    break;
            }
            SplashText.text = chosenText;
            SplashText.gameObject.SetActive(true);

        }

        private void Update()
        {
            if (!Params.EnableMenuSplashAnimation && !ForceEnabled)
                return;

            float animateSpeed = Params.MenuSplashAnimationSpeed * AnimateSpeed;
            float animateScale = Params.MenuSplashAnimationScale * AnimateMaxScale;

            if(AnimationDirection > 0)
            {
                //animate toward max scale
                CurrentScale += animateSpeed * Time.deltaTime;

                if (CurrentScale >= animateScale)
                {
                    //reverse animation direction
                    CurrentScale = animateScale; //clamp
                    AnimationDirection = -1;
                }
            }
            else if(AnimationDirection < 0)
            {
                //animate away from max scale
                CurrentScale -= animateSpeed * Time.deltaTime;

                if (CurrentScale <= 1)
                {
                    //reverse animation direction
                    CurrentScale = 1; //clamp
                    AnimationDirection = 1;
                }
            }

            SplashTransform.localScale = new Vector3(CurrentScale, CurrentScale, 1);
        }

        private string PickSplash()
        {
            var listOfSplashes = LoadSplashModel();
            int index = CoreUtils.Random.Next(0, listOfSplashes.Count);
            return listOfSplashes[index];
        }

        private static IList<string> LoadSplashModel()
        {
            //TODO should improve this and move it elsewhere

            //var resources = CoreUtils.LoadResourceVariants<TextAsset>("Data/splash");
            //only load one list, if you want to merge lists, do it yourself
            var resource = CoreUtils.LoadResource<TextAsset>("Data/Vail/splash");
            return resource.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        //command handling
        private static MainMenuSplashScript GetInstance() => CoreUtils.GetUIRoot().GetComponentInChildren<MainMenuSplashScript>(true);

        [Command(className = "VailMenu", useClassName = true)]
        private static void EnableSplash()
        {
            var instance = GetInstance();

            instance.enabled = true;
            instance.ForceEnabled = true;
            instance.Start();
        }

        [Command(className = "VailMenu", useClassName = true)]
        private static void DisableSplash()
        {
            var instance = GetInstance();
            instance.enabled = false;
            instance.ForceEnabled = false;
            instance.SplashText.gameObject.SetActive(false);
        }

        [Command(className = "VailMenu", useClassName = true)]
        private static void SetSplashText(string text)
        {
            var instance = GetInstance();
            instance.SplashText.text = text;
        }

        [Command(className = "VailMenu", useClassName = true)]
        private static void SetSplashIndex(int index)
        {
            var instance = GetInstance();
            instance.SplashText.text = LoadSplashModel()[index];
        }

        [Command(className = "VailMenu", useClassName = true)]
        private static void PrintSplashList()
        {
            Debug.Log(string.Join("\n", LoadSplashModel()));
        }

        [Command(className = "VailMenu", useClassName = true)]
        private static void DumpSplashList()
        {
            var splashes = string.Join("\n", LoadSplashModel());
            DebugUtils.TextWrite(splashes, "splashes");
        }
    }
}
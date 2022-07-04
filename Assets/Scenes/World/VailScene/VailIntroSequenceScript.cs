using CommonCore;
using CommonCore.Audio;
using CommonCore.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vail
{

    /// <summary>
    /// Handles the actual intro sequence
    /// </summary>
    public class VailIntroSequenceScript : MonoBehaviour
    {
        [SerializeField, Header("UI Elements")]
        private RawImage Background = null;
        [SerializeField]
        private RawImage Foreground = null;
        [SerializeField]
        private CanvasGroup Container = null;

        private Action IntroFinishedCallback;
        private Coroutine IntroCoroutine;

        private VailParams Params => VailParams.Instance; //should probably cache this but fuck it

        public void StartIntroSequence(Action introFinishedCallback)
        {
            //enable this
            gameObject.SetActive(true);

            IntroFinishedCallback = introFinishedCallback;
            IntroCoroutine = StartCoroutine(CoIntro());
        }

        public void CancelIntroSequence()
        {
            if (IntroCoroutine != null)
                StopCoroutine(IntroCoroutine);

            gameObject.SetActive(false);
            IntroFinishedCallback?.Invoke();
        }

        private IEnumerator CoIntro()
        {
            //get colors
            var mutatorBlock = GameState.Instance.GetMutatorBlock();
            Color goodColor = new Color(mutatorBlock[111], mutatorBlock[112], mutatorBlock[113]);
            Color badColor = new Color(mutatorBlock[77], mutatorBlock[78], mutatorBlock[79]);
            Color groundColor = new Color(mutatorBlock[21], mutatorBlock[22], mutatorBlock[23]);
            Color textColor = new Color(mutatorBlock[130], mutatorBlock[131], mutatorBlock[132]);

            var audioPlayer = CCBase.GetModule<AudioModule>().AudioPlayer;

            //start fade and music
            audioPlayer.PlayMusic(Params.IntroMusic, MusicSlot.Ambient, 1.0f, true, false);
            ScreenFader.FadeFrom(Color.black, 1.0f, false, true, false);

            yield return null;

            //set background and foreground
            Background.texture = CoreUtils.LoadResource<Texture2D>("DynamicTexture/introbg");
            Foreground.texture = CoreUtils.LoadResource<Texture2D>("DynamicTexture/introgood");
            Container.alpha = 1;

            //set good colors
            Background.color = goodColor;
            Foreground.color = goodColor;

            //wait for fadein
            yield return new WaitForSeconds(1.0f);
            ScreenFader.ClearFade();
            yield return null;

            //because I'm an idiot and "slide 1" is actually two slides
            float slideTime = Params.IntroSlide1Time / 2f;
            yield return SkippableWait.WaitForSeconds(slideTime);
            yield return null;

            Foreground.texture = CoreUtils.LoadResource<Texture2D>("DynamicTexture/introbad");
            Background.color = badColor;
            Foreground.color = badColor;
            yield return SkippableWait.WaitForSeconds(slideTime);
            yield return null;

            Foreground.texture = CoreUtils.LoadResource<Texture2D>("DynamicTexture/intromessage");
            Background.color = groundColor;
            Foreground.color = textColor;
            yield return SkippableWait.WaitForSeconds(slideTime);
            yield return null;

            //final fadeout
            MusicFader.FadeOut(MusicSlot.Ambient, 1f, false, false);
            yield return FadeoutCanvasGroup();
            MusicFader.ClearFade(MusicSlot.Ambient);
            yield return null;

            gameObject.SetActive(false);
            IntroFinishedCallback?.Invoke();
            IntroCoroutine = null;
        }

        private IEnumerator FadeoutCanvasGroup()
        {
            yield return null;

            float fadeoutTime = 1f;
            for (float elapsed = 0; elapsed <= fadeoutTime; elapsed += Time.deltaTime)
            {
                float alpha = Mathf.Clamp(1 - (elapsed / fadeoutTime), 0, 1);
                Container.alpha = alpha;
                yield return null;
            }

            Container.alpha = 0;
        }

    }
}
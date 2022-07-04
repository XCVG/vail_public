using CommonCore;
using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vail
{

    //mutates the pistol appearance
    public class PistolMutator : MonoBehaviour
    {
        [SerializeField]
        private Transform PistolObjectRoot = null;

        private void Start()
        {
            MutateColors();
            //we don't rescale (yet) because I'm worried it'll break shit
        }

        private void MutateColors()
        {
            var mutatorBlock = GameState.Instance.GetMutatorBlock();

            var frameRenderer = PistolObjectRoot.Find("Frame").GetComponent<Renderer>();
            var frameMaterials = frameRenderer.materials;
            for(int i = 0; i < frameMaterials.Length; i++)
            {
                frameMaterials[i].color = new Color(mutatorBlock[100+i], mutatorBlock[102 + i], mutatorBlock[103 + i]); //skip one so we can never get the same color as others if they overlap
            }
            frameRenderer.materials = frameMaterials; //not sure if necessary

            var slideObject = PistolObjectRoot.Find("Slide");
            var slideRenderer = slideObject.GetComponent<Renderer>();
            slideRenderer.material.color = new Color(mutatorBlock[111], mutatorBlock[112], mutatorBlock[113]);

            var sightRenderer = slideObject.Find("Sight").GetComponent<Renderer>();
            sightRenderer.material.color = new Color(mutatorBlock[114], mutatorBlock[115], mutatorBlock[116]);

            var muzzleRoot = PistolObjectRoot.Find("MuzzleTransform");

            var particleSystem = muzzleRoot.GetComponentInChildren<ParticleSystem>(true);
            var particleMain = particleSystem.main;
            particleMain.startColor = new Color(mutatorBlock[117], mutatorBlock[118], mutatorBlock[119]);

            var light = muzzleRoot.GetComponentInChildren<Light>(true);
            light.color = new Color(mutatorBlock[118], mutatorBlock[119], mutatorBlock[120]);
        }
    }
}
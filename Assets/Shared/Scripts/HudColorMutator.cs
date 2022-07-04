using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vail
{

    public class HudColorMutator : MonoBehaviour
    {
        [SerializeField]
        private Graphic[] BackgroundGraphics = null;
        [SerializeField]
        private Graphic[] GunColorGraphics = null;

        [SerializeField]
        private Graphic[] TextColorGraphics = null;

        [SerializeField]
        private Graphic[] TertiaryColorGraphics = null;

        private void Start()
        {
            var mutatorBlock = GameState.Instance.GetMutatorBlock();

            if (GunColorGraphics != null && GunColorGraphics.Length > 0)
            {
                Color gunColor = new Color(mutatorBlock[111], mutatorBlock[112], mutatorBlock[113]);
                foreach (var gunColorGraphic in GunColorGraphics)
                {
                    SetColorRetainAlpha(gunColorGraphic, gunColor);
                }
            }

            if (BackgroundGraphics != null && BackgroundGraphics.Length > 0)
            {
                Color backgroundColor = new Color(mutatorBlock[192], mutatorBlock[193], mutatorBlock[194]);
                foreach (var backgroundColorGraphic in BackgroundGraphics)
                {
                    SetColorRetainAlpha(backgroundColorGraphic, backgroundColor);
                }
            }

            if (TextColorGraphics != null && TextColorGraphics.Length > 0)
            {
                Color textColor = new Color(mutatorBlock[195], mutatorBlock[196], mutatorBlock[197]);
                foreach (var textColorGraphic in TextColorGraphics)
                {
                    SetColorRetainAlpha(textColorGraphic, textColor);
                }
            }

            if (TertiaryColorGraphics != null && TertiaryColorGraphics.Length > 0)
            {
                Color tertiaryColor = new Color(mutatorBlock[198], mutatorBlock[199], mutatorBlock[200]);
                foreach (var tertiaryColorGraphic in TertiaryColorGraphics)
                {
                    SetColorRetainAlpha(tertiaryColorGraphic, tertiaryColor);
                }
            }
        }

        private void SetColorRetainAlpha(Graphic graphic, Color color) => graphic.color = new Color(color.r, color.g, color.b, graphic.color.a);
    }
}
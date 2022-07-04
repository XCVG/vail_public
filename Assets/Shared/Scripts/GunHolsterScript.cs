using CommonCore.Input;
using CommonCore.RpgGame.Rpg;
using CommonCore.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vail
{

    public class GunHolsterScript : MonoBehaviour
    {
        [SerializeField]
        private float DelayTime = 3f;

        private float Elapsed;

        private void Update()
        {
            if (!GameState.Instance.PlayerRpgState.IsEquipped(EquipSlot.RightWeapon))
                return;

            if (GameState.Instance.PlayerFlags.Contains(PlayerFlags.Frozen) || GameState.Instance.PlayerFlags.Contains(PlayerFlags.TotallyFrozen) || GameState.Instance.PlayerFlags.Contains(PlayerFlags.NoWeapons) || GameState.Instance.PlayerFlags.Contains("VailSceneOver"))
                return;

            if (MappedInput.GetButton(DefaultControls.Reload))
            {
                Elapsed += Time.deltaTime;


                if (Elapsed > DelayTime)
                {
                    GameState.Instance.PlayerRpgState.UnequipItem(EquipSlot.RightWeapon);
                    Elapsed = 0;                    
                }
            }
            else
            {
                Elapsed = 0;
            }

        }
    }
}
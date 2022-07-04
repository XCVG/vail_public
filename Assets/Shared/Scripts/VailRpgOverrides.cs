using CommonCore.RpgGame.Rpg;
using CommonCore.Scripting;
using CommonCore.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vail
{

    public static class VailRpgOverrides
    {
        private static MutatorBlock MutatorBlock => GameState.Instance.GetMutatorBlock();

        [CCScript, CCScriptHook(Hook = ScriptHook.AfterModulesLoaded)]
        private static void InjectVailOverrides()
        {
            RpgValues.SetOverride<Func<CharacterModel, float>>(nameof(MaxHealth), MaxHealth);
            RpgValues.SetOverride<Func<CharacterModel, float>>(nameof(MaxEnergy), MaxEnergy);
            RpgValues.SetOverride<Func<CharacterModel, float>>(nameof(GetMoveSpeedMultiplier), GetMoveSpeedMultiplier);
            RpgValues.SetOverride<Func<CharacterModel, float>>(nameof(GetRunSpeedMultiplier), GetRunSpeedMultiplier);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponRateFactor), GetWeaponRateFactor);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponReloadFactor), GetWeaponReloadFactor);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponSpreadFactor), GetWeaponSpreadFactor);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponInstabilityFactor), GetWeaponInstabilityFactor);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponRecoveryFactor), GetWeaponRecoveryFactor);
            RpgValues.SetOverride<Func<CharacterModel, WeaponItemModel, float>>(nameof(GetWeaponDamageFactor), GetWeaponDamageFactor);

            Debug.Log("[VailRpgOverrides] Injected Vail RPG Overrides");
        }

        public static float MaxHealth(CharacterModel characterModel)
        {
            return 50f + (100f * MutatorBlock[160]);
        }

        public static float MaxEnergy(CharacterModel characterModel)
        {
            return 50f + (100f * MutatorBlock[161]);
        }

        public static float GetMoveSpeedMultiplier(CharacterModel character)
        {
            return 1.1f + ((MutatorBlock[162] - 0.5f) / 2f);
        }

        public static float GetRunSpeedMultiplier(CharacterModel character)
        {
            return 1.1f + ((MutatorBlock[162] - 0.5f) / 2f) + ((MutatorBlock[163] - 0.5f) / 4f);
        }

        public static float GetWeaponRateFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[170] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.25f, 2.0f);
        }

        public static float GetWeaponReloadFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[171] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.5f, 2.0f);
        }

        public static float GetWeaponSpreadFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[172] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.5f, 2.0f);
        }

        public static float GetWeaponInstabilityFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[173] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.5f, 2.0f);
        }

        public static float GetWeaponRecoveryFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[174] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.5f, 2.0f);
        }

        public static float GetWeaponDamageFactor(CharacterModel character, WeaponItemModel itemModel)
        {
            float factor = 1f + ((MutatorBlock[175] - 0.5f) / 0.5f);
            return Mathf.Clamp(factor, 0.5f, 2.0f);
        }
    }
}
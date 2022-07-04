using CommonCore;
using CommonCore.RpgGame.Rpg;
using CommonCore.State;
using Newtonsoft.Json;
using UnityEngine;

//resets player RPG state
public class ResetPlayerIntent : Intent
{
    public override void LoadingExecute()
    {
        if (!Valid)
            return;

        /*
        JsonConvert.PopulateObject(CoreUtils.LoadResource<TextAsset>("Data/RPGDefs/init_player").text, GameState.Instance.PlayerRpgState, new JsonSerializerSettings
        {
            Converters = CCJsonConverters.Defaults.Converters,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });
        GameState.Instance.PlayerRpgState.UpdateStats();

        //disgusting hacks because ObjectCreationHandling breaks our reference links
        GameState.Instance.PlayerRpgState.Equipped.Clear();
        var armorItem = GameState.Instance.PlayerRpgState.Inventory.FindItem("lightarmor")[0];
        armorItem.Equipped = false;
        GameState.Instance.PlayerRpgState.EquipItem(armorItem);
        var pistolItem = GameState.Instance.PlayerRpgState.Inventory.FindItem("vailpistol")[0];
        pistolItem.Equipped = false;
        GameState.Instance.PlayerRpgState.EquipItem(pistolItem);
        */

        //doing it "properly" is hopelessly broken, let's hack only what we need
        var referenceModel = CoreUtils.LoadJson<CharacterModel>(CoreUtils.LoadResource<TextAsset>("Data/RPGDefs/init_player").text);
        
        GameState.Instance.PlayerRpgState.HealthFraction = 1.0f;
        GameState.Instance.PlayerRpgState.EnergyFraction = 1.0f;

        if (!GameState.Instance.PlayerRpgState.IsEquipped(EquipSlot.RightWeapon))
            GameState.Instance.PlayerRpgState.EquipItem(GameState.Instance.PlayerRpgState.Inventory.FindItem("vailpistol")[0]);

        int numBullets = referenceModel.Inventory.CountItem("Para9");
        GameState.Instance.PlayerRpgState.Inventory.FindItem("Para9")[0].Quantity = numBullets;

        int numBulletsInMagazine = referenceModel.AmmoInMagazine[EquipSlot.RightWeapon];
        GameState.Instance.PlayerRpgState.AmmoInMagazine[EquipSlot.RightWeapon] = numBulletsInMagazine;

        Valid = false;
    }
}
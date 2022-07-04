using CommonCore;
using CommonCore.Audio;
using CommonCore.LockPause;
using CommonCore.RpgGame.Rpg;
using CommonCore.RpgGame.World;
using CommonCore.State;
using CommonCore.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vail
{

    /// <summary>
    /// Handles start, randomization, and ending conditions
    /// </summary>
    public class VailSceneSequenceScript : MonoBehaviour
    {
        [SerializeField, Header("Scene Elements")]
        private Light MainDirectionalLight = null;
        [SerializeField]
        private GameObject MainPlane = null;
        [SerializeField]
        private GameObject InvisiblePlane = null;
        [SerializeField]
        private GameObject RandoSpawnPoint = null;
        [SerializeField]
        private VailIntroSequenceScript IntroSequence = null;

        private GameObject RandoObject = null;
        private ActorController RandoController = null;

        private float EndingElasped = 0;
        private bool EndingStarted = false;
        private bool TriggeredSecretEnding = false;

        private VailParams Params => VailParams.Instance; //should probably cache this but fuck it

        //handles randomization and sequencing

        private void Start()
        {
            //poke!
            GameState.Instance.GetMutatorBlock();
            GameState.Instance.PlayerRpgState.UpdateStats(); //needed?

            MutateWorld();

            SpawnRando();

            StartIntro();
        }

        private void Update()
        {
            if (LockPauseModule.IsPaused())
                return;

            HandleEndingConditions();
            HandleEnding();
        }

        private void MutateWorld()
        {
            var mutatorBlock = GameState.Instance.GetMutatorBlock();

            //mutate the planes size
            float worldScale = MathUtils.ScaleRange(mutatorBlock[20], 0, 1, Params.MinWorldScale, Params.MaxWorldScale);
            MainPlane.transform.localScale = new Vector3(worldScale, 1, worldScale);
            InvisiblePlane.transform.localScale = new Vector3(worldScale * InvisiblePlane.transform.localScale.x, 1, worldScale * InvisiblePlane.transform.localScale.z);

            //mutate the plane color
            var planeMaterial = MainPlane.GetComponent<Renderer>().material;
            var groundColor = new Color(mutatorBlock[21], mutatorBlock[22], mutatorBlock[23]);
            planeMaterial.color = groundColor;

            //mutate lighting
            MainDirectionalLight.transform.rotation = Quaternion.Euler(
                MathUtils.ScaleRange(mutatorBlock[1], 0, 1, 0, 360),
                MathUtils.ScaleRange(mutatorBlock[2], 0, 1, 0, 360),
                MathUtils.ScaleRange(mutatorBlock[3], 0, 1, 0, 360));
            MainDirectionalLight.color = new Color(mutatorBlock[4], mutatorBlock[5], mutatorBlock[6]);
            MainDirectionalLight.intensity = mutatorBlock[7];

            RenderSettings.ambientLight = new Color(mutatorBlock[8], mutatorBlock[9], mutatorBlock[10]);

            var skyColor = (Color)(((Vector4)MainDirectionalLight.color + (Vector4)RenderSettings.ambientLight) / 2);
            var skyboxMaterial = new Material(RenderSettings.skybox);
            skyboxMaterial.SetFloat("_Exposure", ((Vector3)(Vector4)skyColor).magnitude);
            skyboxMaterial.SetColor("_SkyTint", skyColor);
            skyboxMaterial.SetColor("_GroundColor", groundColor);
            RenderSettings.skybox = skyboxMaterial;
        }

        private void SpawnRando()
        {
            var mutatorBlock = GameState.Instance.GetMutatorBlock();

            bool useRando2 = mutatorBlock[64] > 0.5f;
            string formID = useRando2 ? "npc_rand2" : "npc_rando";
            //string formID = "npc_rando"; //temporary

            Vector3 offsetSpawnPosition = RandoSpawnPoint.transform.position + new Vector3(mutatorBlock[65] + 0.5f, 0, mutatorBlock[66] + 0.5f) * Params.MaxSpawnDisplacement; //same logic as ScaleRange but a simpler special case
            //Quaternion offsetSpawnRotation = RandoSpawnPoint.transform.rotation * Quaternion.Euler(0, MathUtils.ScaleRange(mutatorBlock[67], 0, 1, -30f, 30f), 0); //no point since doom-style sprites
            Quaternion offsetSpawnRotation = RandoSpawnPoint.transform.rotation;

            //spawn rando!
            RandoObject = WorldUtils.SpawnEntity(formID, "Rando", offsetSpawnPosition, offsetSpawnRotation, null);
            RandoController = RandoObject.GetComponent<ActorController>();

            //mutate rando
            RandoController.Health *= MathUtils.ScaleRange(mutatorBlock[70], 0, 1, Params.MinRandoHealthFactor, Params.MaxRandoHealthFactor); //TODO change to a "set directly" instead of a factor
            RandoObject.transform.localScale *= MathUtils.ScaleRange(mutatorBlock[71], 0, 1, Params.MinRandoScale, Params.MaxRandoScale);

            var randoFSAAC = RandoController.AnimationComponent as FacingSpriteActorAnimationComponent;
            var randoRenderer = randoFSAAC.GetComponentInChildren<Renderer>(true);
            var randoMaterial = randoRenderer.material;
            randoMaterial.color = new Color(mutatorBlock[77], mutatorBlock[78], mutatorBlock[79]);
            randoMaterial.SetColor("_EmissionColor", randoMaterial.color * 0.5f);
        }

        private void HandleEndingConditions()
        {
            if (EndingStarted) //ending has already started, don't check conditions
                return;

            //see if the player has left and/or if the rando is dead and/or if the gun has been holstered

            if(!CheckPlayerInBounds())
            {
                Debug.Log("Ending: Player out of bounds");
                //goto good ending
                
                TriggeredSecretEnding = true;
                StartEnding();
            }
            else if(!GameState.Instance.PlayerRpgState.IsEquipped(EquipSlot.RightWeapon))
            {
                Debug.Log("Ending: Gun holstered");
                //holstered gun, goto that ending
                
                GameState.Instance.GetScoreObject().Mercies++;
                StartEnding();
            }
            else if(CheckRandoIsDead())
            {
                Debug.Log("Ending: Rando killed");
                //rando is dead, goto that ending

                GameState.Instance.GetScoreObject().Kills++;
                StartEnding();
            }

        }

        private bool CheckPlayerInBounds()
        {
            var player = WorldUtils.GetPlayerObject();
            if (player == null)
                return true; //"player doesn't exist" means they're in bounds for our purposes

            Vector2 halfExtents = MainPlane.transform.localScale.GetFlatVector() * 5f * 2f;

            Vector2 playerPosition = player.transform.position.GetFlatVector();

            return !(playerPosition.x < -halfExtents.x || playerPosition.x > halfExtents.x || playerPosition.y < -halfExtents.y || playerPosition.y > halfExtents.y);
        }

        private bool CheckRandoIsDead()
        {
            return (RandoController != null && RandoController.CurrentAiState == ActorAiState.Dead);
        }

        private void StartEnding()
        {
            EndingStarted = true;
            ScreenFader.FadeTo(Color.black, Params.FadeoutTime, false, true, false);
            MusicFader.FadeOut(MusicSlot.Ambient, Params.FadeoutTime, false, false);

            var pflags = (SharedUtils.GetSceneController() as WorldSceneController).TempPlayerFlags;
            pflags.Add("VailSceneOver");
            pflags.Add(PlayerFlags.TotallyFrozen); //do we want this?
        }

        private void HandleEnding()
        {
            if (!EndingStarted)
                return;

            EndingElasped += Time.deltaTime;
            if(EndingElasped >= Params.FadeoutTime)
            {
                //execute transition
                string thisSceneName = SceneManager.GetActiveScene().name;
                MetaState.Instance.Intents.Add(new RegenerateMutatorBlockIntent()); //regenerate the mutator block on scene transition
                MetaState.Instance.Intents.Add(new ClearSceneDataIntent(thisSceneName)); //clear our own scene data
                MetaState.Instance.Intents.Add(new ResetPlayerIntent()); //reset player RPG state
                string nextScene = TriggeredSecretEnding ? Params.SecretEndingScene : thisSceneName;
                WorldUtils.ChangeScene(nextScene, string.Empty, Vector3.zero, Vector3.zero);
            }
        }

        private void StartIntro()
        {
            //lock player and start intro sequence
            var pflags = (SharedUtils.GetSceneController() as WorldSceneController).TempPlayerFlags;
            pflags.Add(PlayerFlags.TotallyFrozen);
            IntroSequence.StartIntroSequence(HandleIntroDone);
        }

        private void HandleIntroDone()
        {
            //unlock player, start music
            var pflags = (SharedUtils.GetSceneController() as WorldSceneController).TempPlayerFlags;
            pflags.Remove(PlayerFlags.TotallyFrozen.ToString());
            CCBase.GetModule<AudioModule>().AudioPlayer.PlayMusic(Params.MainMusic, MusicSlot.Ambient, 1.0f, true, false);
        }


    }
}
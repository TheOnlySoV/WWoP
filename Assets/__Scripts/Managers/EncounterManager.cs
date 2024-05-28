using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class EncounterManager : MonoBehaviour
{
    public GameObject cinemachinePrefab;

    public EncounterState currentState = EncounterState.Waiting;
    [HideInInspector]
    public int currentTurn = 0;
    [HideInInspector]
    public int fleeAttempts = 0;

    [Header("Battle Setup")]
    [SerializeField]
    private List<AudioClip> battleMusic;
    public System.Random battleSeed;

    [Header("Catch Attempt Camera Setup")]

    [Header("Visuals")]
    public GameObject encounterShinyPS;
    [Range(.1f, 50f)] public float UISpeed = 2f;
    [Range(1f, 50f)] public float sliderSpeed = 2f;

    InventoryManager im;
    GameManager gm;
    AudioManager am;

    GameObject activeCam;

    #region Singleton
    public static EncounterManager instance;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (instance)
            Destroy(instance);
        instance = this;
    }
    #endregion

    private void Start()
    {
        gm = GameManager.instance;
        im = InventoryManager.instance;
        am = AudioManager.instance;
    }

    #region Set up Encounter
    public void BeginEncounter(PokemonMotor targetMotor)
    {
        gm = GameManager.instance;

        battleSeed = new System.Random();

        //gm.trainer.GetComponent<AudioSource>().clip = am.GetWildTheme();
        //gm.trainer.GetComponent<AudioSource>().Play();

        currentState = EncounterState.Waiting;
        fleeAttempts = 0;
        currentTurn = 0;

        //encounteredPokemonTransform = target.transform;
        //myPokemon = FirstHealthyPokemon(im.ownedCritters);

        //rotate the encountered pokemon to face the player, also stop the pokemon from moving. zoom the camera in to see the pokemon and move the player during this time. maybe just make the player disappear?
        //UpdateUI();

        CreateCam(targetMotor);
    }

    void CreateCam(PokemonMotor targetMotor)
    {
        GameObject newPrefab = Instantiate(cinemachinePrefab);
        CinemachineVirtualCamera vCam = newPrefab.GetComponent<CinemachineVirtualCamera>();

        vCam.m_Follow = targetMotor.battleCameraRoot;

        activeCam = newPrefab;

        TrainerCallPokemon();
    }

    public void DestroyCam()
    {
        activeCam.SetActive(false);
        Destroy(activeCam, 1f);
        activeCam = null;
    }

    void TrainerCallPokemon()
    {
        //trainer turns around
        //throws pokemon to set location X distance away from encountered pokemon
        //called pokemon faces encountered pokemon
        //encountered pokemon gets targetPokemonLocation set
    }

    AudioClip GetRandomBattleMusic()
    {
        return battleMusic[battleSeed.Next(0, battleMusic.Count)];
    }

    Owned ConvertToOwned(Wild target)
    {
        Owned newCritter = new Owned();
        newCritter.pokemon = target.pokemon;
        newCritter.IV = target.IV;
        newCritter.EV = new Stats();
        newCritter.stats = target.stats;
        newCritter.level = target.level;
        newCritter.nature = target.nature;
        newCritter.shiny = target.shiny;
        newCritter.moveset = target.moveset;
        newCritter.currentHealth = target.stats.HP;
        newCritter.gender = target.gender;
        newCritter.ability = target.ability;
        switch (target.gender)
        {
            case Gender.Male:
                if (newCritter.shiny)
                    newCritter.myModel = target.pokemon.info.profile.models.myShinyMaleModel;
                else
                    newCritter.myModel = target.pokemon.info.profile.models.myMaleModel;
                break;

            case Gender.Female:
                if (newCritter.shiny)
                    newCritter.myModel = target.pokemon.info.profile.models.myShinyFemaleModel;
                else
                    newCritter.myModel = target.pokemon.info.profile.models.myFemaleModel;
                break;

            case Gender.None:
                break;
        }
        newCritter.tID = NetworkManager.instance.clientTrainer.tID.ToString().PadLeft(6, '0');
        return newCritter;
    }

    Owned FirstHealthyPokemon(List<Owned> myParty)
    {
        Owned newHealthy = myParty[0];
        for (int i = 0; i <= myParty.Count - 1; i++)
        {
            if (myParty[i].status != ConditionStatus.Fainted)
                return newHealthy;
        }
        return newHealthy;
    }

    void UpdateUI()
    {
        #region My Critter UI
        //SetMyUI();
        #endregion

        #region Moveset UI
        UpdateMovesetUI();
        #endregion

        #region Target Critter UI

        #endregion
    }

    public void UpdateMovesetUI()
    {

    }

    public void SwitchPokemon(int index)
    {
        //myPokemon = im.party[index];//set our new pokemon
        //Destroy(myPokemonGameobject);
        //ChoosePokemon(encounteredPokemonTransform);

        EnemyRetaliate();
    }

    #endregion

    #region Bag Display

    void EnemyRetaliate()
    {
        currentTurn++;
        //Move enemySelectedMove = EnemyAttackChoice();
        //movesToUse.Add(ConvertToPokemonAttack(encounteredPokemon, myPokemon, enemySelectedMove));
        //StartCoroutine(IAttack());
    }

    IEnumerator UsingItemTextUI()
    {
        yield return new WaitForSecondsRealtime(sliderSpeed);
        //encounterDisplayTextUI.SetActive(false);

        EnemyRetaliate();
    }

    public void PokemonPartyButton(int pokemonIndex)
    {
        //pokemonPartyParent.SetActive(false);
        switch (currentState)
        {
            case EncounterState.Waiting://default state, used only before battles to reset the state
                //should NOT end up here
                Debug.LogWarning("State is set incorrectly!");
                break;
            case EncounterState.Attacking://button press on attack
                //should NOT end up here
                Debug.LogWarning("State is set incorrectly!");
                break;
            //case EncounterState.UsingBagItem://button press on bag
            //    ExecuteUseItem(pokemonIndex);
            //    break;
            case EncounterState.Catching://button press on bag -> pokeball
                //should NOT end up here
                Debug.LogWarning("State is set incorrectly!");
                break;
            case EncounterState.Switching://button press on attack
                SwitchPokemon(pokemonIndex);
                break;
            case EncounterState.Ending://button press on run
                //should NOT end up here
                Debug.LogWarning("State is set incorrectly!");
                break;
        }
    }
    #endregion

    #region Attack Management
    public void SelectAttack(int index)
    {
        //look for the selected move
        //Move selectedMove = myPokemon.moveset[index].move;
        currentTurn++;

        //execute attack strategy
        //PriorityCheck(selectedMove);
    }

    PokemonAttack ConvertToPokemonAttack(Owned attacker, Owned target, Move move)
    {
        PokemonAttack newAttackToAdd = new PokemonAttack();

        newAttackToAdd.move = move;
        newAttackToAdd.user = attacker;
        newAttackToAdd.target = target;

        return newAttackToAdd;
    }

    //IEnumerator DisplayHealing()
    //{
    //    int healingAmount = im.ownedCritters[0].currentHealth + im.healingBag[currentPocketIndex].item.healAmount > im.ownedCritters[0].stats.HP
    //                      ? im.ownedCritters[0].stats.HP - im.ownedCritters[0].currentHealth
    //                      : im.healingBag[currentPocketIndex].item.healAmount;

    //    float change = healingAmount / sliderSpeed;

    //    int result = im.ownedCritters[0].currentHealth + healingAmount;

    //    Slider targetSlider = encounterUIParent.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Slider>();

    //    while (targetSlider.value < result)
    //    {
    //        targetSlider.value += change * Time.deltaTime;
    //        yield return 0;
    //    }
    //    encounterDisplayTextUI.SetActive(false);

    //    targetSlider.value = im.ownedCritters[0].currentHealth;

    //    EnemyRetaliate();
    //}



    /* LET'S TALK ABOUT BATTLE FLOW
        * 1. Move has been selected. - DONE
        * - START HERE - This can be a function that returns the move?
        * 3. Check if either move is a priority move, or if they both are. - DONE
        * 2. Check if the attacker is paralyzed - ADDED
        * 4. Check if the attacker is asleep - ADDED
        * 4.1 Perform a wake check if needed - ADDED
        * 5. Perform a speed evaluation if needed at this point - DONE
        * - END OF CONDENSED PART
        * 
        * ##### IMPORTANT STEP #####
        * 6. USE THE FIRST MOVE
        * - MAKE THIS A COROUTINE TO BE USED
        * 7. Apply damage to the target (Also find out if the status condition would be applied if the target does not faint) - Coroutine for UI - CONDENSED!!!
        * 8. Check if the Pokemon fainted - NEW COROUTINE!!! - DONE
        * - END OF NEW COROUTINE
        * 
        * 9. Apply status condition if needed - Coroutine for UI
        * 9.1 Apply stat modifiers if needed - Coroutine for UI - DONE!
        * 10. Check if the move has recoil - Coroutine for UI - ADDED
        * 
        * ##### END OF IMPORTANT STEPS #####
        * 11. USE THE SECOND MOVE
        * 12. Repeat steps 7 - 10
        * 
        * - MAKE THIS A COROUTINE TO BE USED
        * 13. If no more moves are in the list, apply status condition effects for Burn/Poison - Coroutine for UI - DONE
        * 14. Check if that fainted the pokemon again - ADDED!
        */

    string EffectivenessUI(float effectivenessMultiplier)
    {
        switch (effectivenessMultiplier)
        {
            case 0:
                return "It had no effect.";
            case .125f:
                return "Not very effective!";
            case .25f:
                return "Not very effective!";
            case .5f:
                return "Not very effective!";
            case 1f:
                return string.Empty;
            case 2f:
                return "Super effective!";
            case 4f:
                return "Super effective!";
            case 8f:
                return "Super effective!";

            default:
                return string.Empty;
        }
    }

    string DetailText(int value)
    {
        if (value > 0)
            return "won't go any higher";
        else
            return "won't go any lower";
    }

    string StatModifierText(int valueToCheck)
    {
        string returnString = string.Empty;

        if (valueToCheck == 0)
            return string.Empty;
        switch (valueToCheck)
        {
            case -3:
                returnString = "severely fell";
                break;
            case -2:
                returnString = "harshly fell";
                break;
            case -1:
                returnString = "fell";
                break;
            case 0:
                returnString = "won't go any higher";
                break;
            case 1:
                returnString = "rose";
                break;
            case 2:
                returnString = "rose sharply";
                break;
            case 3:
                returnString = "rose drastically";
                break;
        }
        return returnString;
    }

    void UpdateFaintedPokemonUI()
    {
        //Transform partyParent = encounterUIParent.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0);

        //for (int i = 0; i <= 5; i++)
        //    partyParent.GetChild(i).gameObject.SetActive(false); //reset the UI

        //for (int i = 0; i <= im.ownedCritters.Count - 1; i++)
        //{
        //    partyParent.GetChild(i).gameObject.SetActive(true);

        //    //set sprite
        //    partyParent.GetChild(i).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = im.ownedCritters[i].critter.sprite;

        //    //set HP slider
        //    partyParent.GetChild(i).GetChild(0).GetChild(1).GetComponent<Slider>().maxValue = im.ownedCritters[i].stats.HP;
        //    partyParent.GetChild(i).GetChild(0).GetChild(1).GetComponent<Slider>().value = im.ownedCritters[i].currentHealth;

        //    //set EXP slider
        //    partyParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<Slider>().maxValue = im.ownedCritters[i].nextLevelEXP;
        //    partyParent.GetChild(i).GetChild(0).GetChild(2).GetComponent<Slider>().value = im.ownedCritters[i].currentEXP;

        //    //set name
        //    partyParent.GetChild(i).GetChild(0).GetChild(3).GetChild(0).GetComponent<Text>().text = im.ownedCritters[i].critter.critterName;
        //}
    }

    //void DeductPP(Move moveToCheck, int partySlot)
    //{
    //    for (int i = 0; i <= im.ownedCritters[partySlot].moveset.Count - 1; i++)
    //    {
    //        if (im.ownedCritters[partySlot].moveset[i].move == moveToCheck)
    //        {
    //            im.ownedCritters[partySlot].moveset[i].timesUsed++;
    //        }
    //    }
    //}

    ConditionStatus CheckHealthStatus(Owned pokemon)
    {
        if (pokemon.status == ConditionStatus.Burned || pokemon.status == ConditionStatus.Poisoned)
            return pokemon.status;
        else
            return ConditionStatus.Healthy;
    }
    #endregion

    #region UI Management



    #endregion

    public void RunAttempt()
    {
        string textToDisplay = string.Empty;
        bool escaped = false;

        //encounterUIParent.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);

        //if (myPokemon.stats.speed >= encounteredPokemon.stats.speed)
        //{
        //    textToDisplay = "Ran away successfully!";
        //    escaped = true;
        //    StartCoroutine(RunCoroutine(textToDisplay, escaped));
        //    return;
        //}

        fleeAttempts++;
        //float oddsOfEscape = ((Mathf.Floor((myPokemon.stats.speed) * 32f) / (encounteredPokemon.stats.speed / 4f)) + 30f * fleeAttempts) / 256f;
        //int valueCheck = (int)(256 * oddsOfEscape);
        //int RNG = Random.Range(0, 256);

        //if (RNG < valueCheck)
        //    escaped = true;

        if (escaped)
            textToDisplay = "Ran away successfully!";
        else
            textToDisplay = "Couldn't get away.";

        StartCoroutine(RunCoroutine(textToDisplay, escaped));
    }

    public void RunFromFaintedAttempt()
    {
        string textToDisplay = string.Empty;
        bool escaped = false;

        //encounterUIParent.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);

        //if (myPokemon.stats.speed >= encounteredPokemon.stats.speed)
        //    textToDisplay = "Ran away successfully!";

        fleeAttempts++;
        //float oddsOfEscape = ((Mathf.Floor((myPokemon.stats.speed) * 32f) / (encounteredPokemon.stats.speed / 4f)) + 30f * fleeAttempts) / 256f;
        //int valueCheck = (int)(256 * oddsOfEscape);
        //int RNG = Random.Range(0, 256);

        //if (RNG < valueCheck)
        //    escaped = true;

        if (escaped)
            textToDisplay = "Ran away successfully!";
        else
            textToDisplay = "Could not get away.";

        StartCoroutine(RunFromFaintedCoroutine(textToDisplay, escaped));
    }

    IEnumerator RunCoroutine(string text, bool escaped)
    {
        //encounterDisplayTextUI.transform.GetChild(0).GetComponent<Text>().text = text;
        //encounterDisplayTextUI.SetActive(true);

        yield return new WaitForSeconds(sliderSpeed);

        //encounterDisplayTextUI.SetActive(false);
        if (escaped)
            EndBattle();
        else
            EnemyRetaliate();
    }

    IEnumerator RunFromFaintedCoroutine(string text, bool escaped)
    {
        //encounterDisplayTextUI.transform.GetChild(0).GetComponent<Text>().text = text;
        //encounterDisplayTextUI.SetActive(true);

        yield return new WaitForSeconds(sliderSpeed);

        //encounterDisplayTextUI.SetActive(false);
        if (escaped)
            EndBattle();
        //else
        //    encounterUIParent.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(2).GetComponent<Button>().interactable = false;

    }

    public void EndBattle()
    {
        ManagePlayer(false);
        //gm.inBattle = false;

        //evolve any pokemon
        //foreach (OwnedCritter partyPokemon in im.ownedCritters)
        //{
        //    if (partyPokemon.leveledUp)
        //    {
        //        partyPokemon.critter = partyPokemon.critter.evolution;

        //        if (partyPokemon.shiny)
        //        {
        //            switch (partyPokemon.gender)
        //            {
        //                case Gender.None:
        //                    partyPokemon.myModel = partyPokemon.critter.myShinyMaleModel;
        //                    break;
        //                case Gender.Male:
        //                    partyPokemon.myModel = partyPokemon.critter.myShinyMaleModel;
        //                    break;
        //                case Gender.Female:
        //                    partyPokemon.myModel = partyPokemon.critter.myShinyFemaleModel;
        //                    break;
        //                case Gender.Legendary:
        //                    partyPokemon.myModel = partyPokemon.critter.myShinyFemaleModel;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (partyPokemon.gender)
        //            {
        //                case Gender.None:
        //                    partyPokemon.myModel = partyPokemon.critter.myMaleModel;
        //                    break;
        //                case Gender.Male:
        //                    partyPokemon.myModel = partyPokemon.critter.myMaleModel;
        //                    break;
        //                case Gender.Female:
        //                    partyPokemon.myModel = partyPokemon.critter.myFemaleModel;
        //                    break;
        //                case Gender.Legendary:
        //                    partyPokemon.myModel = partyPokemon.critter.myFemaleModel;
        //                    break;
        //            }
        //        }

        //        partyPokemon.leveledUp = false;
        //    }
        //}

        currentState = EncounterState.Waiting;

        //cinemachineCamera.Follow = gm.player.GetChild(0);
    }

    void ReactivateNavMeshAgent(GameObject target)
    {
        target.GetComponent<NavMeshAgent>().enabled = true;
        target.GetComponent<NavMeshAgent>().isStopped = false;
        //target.GetComponent<NPCMotor>().encountered = false;

        //target.GetComponent<NPCMotor>().RunHook();

        //encounteredPokemonTransform.GetChild(1).gameObject.SetActive(false);
    }

    void ManagePlayer(bool freeCursor)
    {
        Cursor.lockState = freeCursor ? CursorLockMode.None : CursorLockMode.Locked;

        //gm.player.GetChild(1).gameObject.SetActive(!freeCursor);
        //gm.player.GetComponent<StarterAssetsInputs>().cursorLocked = !freeCursor;
        //gm.player.GetComponent<StarterAssetsInputs>().cursorInputForLook = !freeCursor;
    }

    public void CatchAttempt()
    {
        //Transform catchAttemptCameraLocation = Instantiate(new GameObject(), battleCenter.transform.position, Quaternion.identity).transform;

        //catchAttemptCameraLocation.LookAt(encounteredPokemonTransform);
        //catchAttemptCameraLocation.position = encounteredPokemonTransform.position;
        //encounterCinemachineCamera.m_Follow = catchAttemptCameraLocation;
        //StartCoroutine(BallShake());
    }

    IEnumerator BallShake()
    {
        //int result = im.ballBag[currentPocketIndex].item.ThrowPokeball(encounteredPokemon, myPokemon);
        //int result = 0;
        //if (result == -1)
        //{
            //TODO add a catch that will not let anything happen and instead display a message for 3 - 5 seconds depending on text speed
            //yield break;
        //}
        //int shakeIndex = 0;
        //while (shakeIndex < result)
        //{
        //    //a check is performed, if false, no shakes happen. number of shakes = 0, shake checks = 1;
        //    //if it passes the shake check, 1 shake happens. if false, no more shakes happen. number of shakes = 1, shake checks = 2;
        //    //if it passes the shake check, 1 shake happens. if false, no more shakes happen. number of shakes = 2, shake checks = 3;
        //    //if it passes the shake check, 1 shake happens. if false, no more shakes happen. number of shakes = 3, shake checks = 4;

        //    //every time there is a check, the camera needs to zoom in - DONE
        //    SetBattleCam(catchAttemptCameraSettings[shakeIndex + 1]);//Zoom camera in - CHECK

        //    //if the check fails, wait for animation, then zoom the camera back out to default settings and set the follow back to the battle center - DONE
        //    if (shakeIndex + 1 == result && result < 4)
        //    {
        //        yield return new WaitForSecondsRealtime(shakeWaitTime / 2f);
        //        SetBattleCam(catchAttemptCameraSettings[0], true);//reset the camera to default settings and reset the follow target - CHECK
        //        EnemyRetaliate();
        //        yield return null;
        //    }
        //    else
        //    {
        //        yield return new WaitForSecondsRealtime(shakeWaitTime);
        //        SetBattleCam(catchAttemptCameraSettings[shakeIndex]);//reset the camera to default settings and reset the follow target - CHECK
        //    }
        //    shakeIndex++;//shake checks - CHECK
        //    if (shakeIndex >= 4)
        //    {
        //        //im.Catch(encounteredPokemon, encounteredPokemon.EvaluateEXPGain(encounteredPokemon, myPokemon.level));

        //        encounterDisplayTextUI.SetActive(true);
        //        encounterDisplayTextUI.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0} was caught!", encounteredPokemon.pokemon.info.pokemonName);
                yield return new WaitForSeconds(2f);
        //        encounterDisplayTextUI.SetActive(false);

        //        Destroy(encounteredPokemonTransform.gameObject);
        //        EndBattle();
        //    }
        //}
    }

    void SetBattleCam(ShakeAttemptCameraSettings settings, bool resetFollow = false)
    {
        //encounterCinemachineCamera.m_Lens.FieldOfView = settings.shakeAttemptFOV;
        //encounterCinemachineCamera.GetComponent<CinemachineRecomposer>().m_ZoomScale = settings.shakeZoom;
        //encounterCinemachineCamera.GetComponent<CinemachineRecomposer>().m_Tilt = settings.shakeTilt;
        //if (resetFollow)
        //{
        //    GameObject cleanup = encounterCinemachineCamera.Follow.gameObject;
        //    encounterCinemachineCamera.Follow = battleCenter.transform;
        //    Destroy(cleanup);
        //}
    }

    public void SetState(int state)
    {
        currentState = (EncounterState)state;
    }
}
public enum EncounterState { Waiting, Attacking, UsingBagItem, Catching, Switching, Ending }

[System.Serializable]
public class PokemonAttack
{
    public Move move;
    public Owned user;
    public Owned target;
}

[System.Serializable]
public class ShakeAttemptCameraSettings
{
    [Tooltip("Name for the list")]
    public string name;
    [Range(10, 50)]
    public float shakeTilt = 20;
    [Range(.1f, 1f)]
    public float shakeZoom = 1f;
    [Range(50, 110)]
    public int shakeAttemptFOV = 60;

    public string shakeFailText = string.Empty;
}
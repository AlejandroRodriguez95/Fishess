using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Fish types")]
    [Tooltip("Add all the different type of fishes here! " +
        "You can find them in the fishTypes folder." +
        "\nTo create new fishes: \n" +
        "1) right click on the project view -> Create -> ScriptableObjects -> new fish\n" +
        "2) fill the values on the new file\n" +
        "3) drag this new file to this collection in the inspector.\n" +
        "note: it's important that every fish gets an ID, and the IDs must follow a numerical" +
        "order, without skipping numbers: (0, 1, 2, 3, 4, 5 ...)")]
    [SerializeField]
    List<Fish> fishCollection;
    [SerializeField]
    List<Fish> availableFishes;
    [SerializeField]
    GameObject pullSystem;    
    [SerializeField]
    GameObject reelSystem;
    [SerializeField]
    Image capturedFish;

    [Header("Script references")]
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    CharacterAnimationController characterAnimation;
    [SerializeField]
    PullController pullController;    
    [SerializeField]
    ReelController reelController;

    //game loop variables: 
    [SerializeField]
    GameStages currentStage;

    bool waitingCoroutineIsActive;
    private Coroutine waitAndChangeStage;
    [SerializeField]
    Fish currentFish;
    bool wasFishCaptured;
    int reelSpins;
    int reelSpinsToCatch;
    int fishesCaptured;
    [SerializeField]
    int currentFishMisses;

    private void Awake()
    {
        uiManager.UIFishCollection = fishCollection;
        availableFishes = new List<Fish>(fishCollection);
        fishesCaptured = 0;

        currentFish = availableFishes[Random.Range(0, availableFishes.Count)];
        currentFishMisses = 0;
        reelSpins = 0;
        reelSpinsToCatch = Random.Range((int)currentFish.ReelSpinsToCatch.x,(int)currentFish.ReelSpinsToCatch.y);

        ReelController.SpinCountUpdated += OnSpinCountUpdated;

        currentStage = GameStages.Idle;
    }

    private void Update()
    {
        HandleStage();
    }

    /// <summary>
    /// Here we will manage input for the different types of stages
    /// </summary>
    private void HandleStage()
    {
        switch (currentStage)
        {
            //player can cast rod
            //  possible input -> space
            case GameStages.Idle:
                if (fishesCaptured == 1)
                    Fade.fadeSky.Invoke();

                capturedFish.gameObject.SetActive(false);
                capturedFish.sprite = null;
                reelSpinsToCatch = Random.Range((int)currentFish.ReelSpinsToCatch.x, (int)currentFish.ReelSpinsToCatch.y);

                characterAnimation.UpdateAnimation(currentStage);
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    pullController.SweetSpotMinAndMaxSize = new Vector2(currentFish.SweetSpotSizeRange.x, currentFish.SweetSpotSizeRange.y);
                    currentStage = GameStages.CastingRod;
                }
                break;
                
            //player isn't able to do anything until a fish is caught
            case GameStages.CastingRod:
                characterAnimation.UpdateAnimation(currentStage);
                currentStage = GameStages.WaitingForFish;
                uiManager.ToggleOffButton.GetComponent<Button>().onClick.Invoke();
                uiManager.ToggleOnButton.SetActive(false);

                break;
            case GameStages.WaitingForFish:
                if(!waitingCoroutineIsActive)
                {
                    float waitingTime = Random.Range(currentFish.FishBitesBaitTime.x, currentFish.FishBitesBaitTime.y);
                    StartCoroutine(WaitForSecondsAndMoveToStage(Random.Range(currentFish.FishBitesBaitTime.x, currentFish.FishBitesBaitTime.y), GameStages.FishBitRod));
                }
                break;

            //after a few seconds, player transitions to the pull animation
            //  possible input -> nothing?
            case GameStages.FishBitRod:
                if (!waitingCoroutineIsActive)
                {
                    characterAnimation.UpdateAnimation(currentStage);

                    StartCoroutine(WaitForSecondsAndMoveToStage(Random.Range(currentFish.FishBitesBaitTime.x, currentFish.FishBitesBaitTime.y), Random.Range(0, 2) == 0 ? GameStages.Reel : GameStages.Pull));

                }
                break;
            //player pulls for X amount of times, defined in the fish data
            //  possible input -> space
            case GameStages.Pull:
                if (!waitingCoroutineIsActive)
                {
                    pullController.LerpSpeed = Random.Range(currentFish.PullIndicatorSpeedRange.x, currentFish.PullIndicatorSpeedRange.y);
                    reelSystem.SetActive(false);
                    characterAnimation.UpdateAnimation(currentStage);
                    pullSystem.SetActive(true);
                    waitAndChangeStage = StartCoroutine(WaitForSecondsAndMoveToStage(10f, GameStages.Reel));
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PullResult result = pullController.CheckIfIndicatorIsInContactWithSweetSpot();
                    switch (result)
                    {
                        case PullResult.Hit:
                            reelSpins++;
                            break;
                        case PullResult.Miss:
                            currentFishMisses++;
                            break;
                        case PullResult.Bitter:
                            currentFishMisses += 2;
                            break;
                    }
                }
                if (DidFishEscape())
                {
                    pullSystem.SetActive(false);
                    reelSystem.SetActive(false);
                    StopCoroutine(waitAndChangeStage);
                    waitingCoroutineIsActive = false;
                    currentStage = GameStages.FishRanAway;
                }


                break;

            //player reels until he reaches the necessary amount of spins around the reel.
            //-amount spins is defined per fish
            //-friction increases, defined per fish
            //  possible input -> mouse movement
            case GameStages.Reel:
                if (!waitingCoroutineIsActive)
                {
                    reelController.Friction = Random.Range(currentFish.ReelFrictionRange.x, currentFish.ReelFrictionRange.y);
                    reelSystem.SetActive(true);
                    characterAnimation.UpdateAnimation(currentStage);
                    pullSystem.SetActive(false);
                    waitAndChangeStage = StartCoroutine(WaitForSecondsAndMoveToStage(10f, GameStages.Pull));
                }
                if (wasFishCaptured)
                {
                    pullSystem.SetActive(false);
                    reelSystem.SetActive(false);
                    uiManager.ToggleOnButton.GetComponent<Button>().onClick.Invoke();
                    StopCoroutine(waitAndChangeStage);
                    waitingCoroutineIsActive = false;
                    currentStage = GameStages.FishCaught;
                }
                break;

            //player unlocks fish in journal
            //current fish variables are reseted
            //  possible input -> mouse, opening journal
            case GameStages.FishCaught:
                if (!waitingCoroutineIsActive)
                {
                    capturedFish.sprite = currentFish.FishImage;
                    capturedFish.gameObject.SetActive(true);

                    characterAnimation.UpdateAnimation(currentStage);
                    uiManager.UnlockFish(currentFish.FishId);
                    availableFishes.Remove(currentFish);

                    fishesCaptured++;

                    ResetCurrentFish();
                    StartCoroutine(WaitForSecondsAndMoveToStage(8f, GameStages.Idle));
                }

                break;

            case GameStages.FishRanAway:
                if (!waitingCoroutineIsActive)
                {
                    uiManager.ToggleOnButton.SetActive(true);
                    characterAnimation.UpdateAnimation(currentStage);
                    ResetCurrentFish();
                    StartCoroutine(WaitForSecondsAndMoveToStage(5f, GameStages.Idle));
                }
                break;
        }
    }

    IEnumerator WaitForSecondsAndMoveToStage(float seconds, GameStages nextStage)
    {
        waitingCoroutineIsActive = true;
        yield return new WaitForSeconds(seconds);
        currentStage = nextStage;
        waitingCoroutineIsActive = false;
    }

    private bool DidFishEscape()
    {
        if (currentFishMisses > currentFish.FishMaxFails)
            return true;
        
        return false;
    }

    private void ResetCurrentFish()
    {
        currentFish = availableFishes[Random.Range(0, availableFishes.Count)];
        currentFishMisses = 0;
        reelSpins = 0;
        wasFishCaptured = false;
    }

    private void UpdateStage(GameStages newStage)
    {
        currentStage = newStage;
        characterAnimation.UpdateAnimation(currentStage);
    }
    void OnSpinCountUpdated()
    {
        reelSpins++;
        if (reelSpins >= reelSpinsToCatch) // was fish captured?
            wasFishCaptured = true;

    }

}

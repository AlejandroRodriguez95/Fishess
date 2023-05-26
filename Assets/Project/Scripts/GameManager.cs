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

    [Header("Script references")]
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    CharacterAnimationController characterAnimation;
    [SerializeField]
    PullController pullController;

    //game loop variables: 
    [SerializeField]
    GameStages currentStage;

    bool waitingCoroutineIsActive;
    private Coroutine waitAndChangeStage;
    [SerializeField]
    Fish currentFish;
    [SerializeField]
    int currentFishHealth;
    [SerializeField]
    int currentFishMisses;

    private void Awake()
    {
        uiManager.UIFishCollection = fishCollection;
        availableFishes = new List<Fish>(fishCollection);

        currentFish = availableFishes[Random.Range(0, availableFishes.Count)];
        currentFishHealth = currentFish.FishMaxHealth;
        currentFishMisses = 0;

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
                characterAnimation.UpdateAnimation(currentStage);
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    currentStage = GameStages.CastingRod;
                }
                break;
                
            //player isn't able to do anything until a fish is caught
            case GameStages.CastingRod:
                characterAnimation.UpdateAnimation(currentStage);
                currentStage = GameStages.WaitingForFish;
                break;
            case GameStages.WaitingForFish:
                if(!waitingCoroutineIsActive)
                {
                    float waitingTime = Random.Range(currentFish.FishBitesBaitTime.x, currentFish.FishBitesBaitTime.y);
                    StartCoroutine(WaitForSecondsAndMoveToStage(waitingTime, GameStages.FishBitRod));
                }
                break;

            //after a few seconds, player transitions to the pull animation
            //  possible input -> nothing?
            case GameStages.FishBitRod:
                if (!waitingCoroutineIsActive)
                {
                    characterAnimation.UpdateAnimation(currentStage);
                    StartCoroutine(WaitForSecondsAndMoveToStage(Random.Range(2, 5), GameStages.Reel));
                }
                break;
            //player pulls for X amount of times, defined in the fish data
            //  possible input -> space
            case GameStages.Pull:
                if (!waitingCoroutineIsActive)
                {
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
                            currentFishHealth--;
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
                    StopCoroutine(waitAndChangeStage);
                    waitingCoroutineIsActive = false;
                    currentStage = GameStages.FishRanAway;
                }

                if (WasFishCaptured())
                {
                    pullSystem.SetActive(false);
                    StopCoroutine(waitAndChangeStage);
                    waitingCoroutineIsActive = false;
                    currentStage = GameStages.FishCaught;
                }
                break;

            //player reels until he reaches the necessary amount of spins around the reel.
            //-amount spins is defined per fish
            //-friction increases, defined per fish
            //  possible input -> mouse movement
            case GameStages.Reel:
                if (!waitingCoroutineIsActive)
                {
                    characterAnimation.UpdateAnimation(currentStage);
                    pullSystem.SetActive(false);
                    waitAndChangeStage = StartCoroutine(WaitForSecondsAndMoveToStage(3f, GameStages.Pull));
                }
                break;

            //player unlocks fish in journal
            //current fish variables are reseted
            //  possible input -> mouse, opening journal
            case GameStages.FishCaught:
                if (!waitingCoroutineIsActive)
                {
                    characterAnimation.UpdateAnimation(currentStage);
                    uiManager.UnlockFish(currentFish.FishId);
                    availableFishes.Remove(currentFish);
                    ResetCurrentFish();
                    StartCoroutine(WaitForSecondsAndMoveToStage(5f, GameStages.Idle));
                }

                break;

            case GameStages.FishRanAway:
                if (!waitingCoroutineIsActive)
                {
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

    private bool WasFishCaptured()
    {
        if (currentFishHealth <= 0)
            return true;
        
        return false;
    }

    private void ResetCurrentFish()
    {
        currentFish = availableFishes[Random.Range(0, availableFishes.Count)];
        currentFishHealth = currentFish.FishMaxHealth;
        currentFishMisses = 0;
    }

    private void UpdateStage(GameStages newStage)
    {
        currentStage = newStage;
        characterAnimation.UpdateAnimation(currentStage);
    }

}

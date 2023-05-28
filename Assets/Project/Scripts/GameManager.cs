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
    SpriteRenderer gameOverOverlay;    
    [SerializeField]
    SpriteRenderer gameOverText;
    [SerializeField]
    float gameOverFadeOutDuration;


    [Header("Audio")]
    [SerializeField]
    List<AudioClip> audioClips;
    [SerializeField]
    List<AudioClip> audioClipsHitAndPull;

    // Audios indexes:
    // 0.   Swing rod
    // 1.   Reel slow
    // 2.   Reel medium
    // 3.   Reel fast
    // 4.   Fish Bite Bait
    // 5.   Fish Splash
    // 6.   Fish Caught
    // 7.   Radio guy speaking
    // 8.   Radio guy speaking short
    // 9.   Sea sounds
    // 10.  Creepy radio noise



    [Header("Script references")]
    [SerializeField]
    AudioManager audioManager;
    [SerializeField]
    UIManager uiManager;
    [SerializeField]
    CharacterAnimationController characterAnimation;
    [SerializeField]
    PullController pullController;    
    [SerializeField]
    ReelController reelController;
    [SerializeField]
    TextScript radioTextScript;

    //game loop variables: 
    [SerializeField]
    GameStages currentStage;
    [SerializeField]
    Image capturedFish;
    [SerializeField]
    int pullsToReelAgain; // how many times the player must succeed the pull mini game to advance

    bool waitingCoroutineIsActive;
    private Coroutine waitAndChangeStage;
    [SerializeField]
    Fish currentFish;
    bool wasFishCaptured;
    int reelSpins;
    int reelSpinsToCatch;
    [SerializeField]
    int pullCount;
    int fishesCaptured;
    [SerializeField]
    int currentFishMisses;

    private void Awake()
    {
        uiManager.UIFishCollection = fishCollection;
        availableFishes = new List<Fish>(fishCollection);
        fishesCaptured = 0;

        if (availableFishes.Count > 0)
            currentFish = availableFishes[0];

        currentFishMisses = 0;
        reelSpins = 0;
        reelSpinsToCatch = Random.Range((int)currentFish.ReelSpinsToCatch.x,(int)currentFish.ReelSpinsToCatch.y);

        ReelController.SpinCountUpdated += OnSpinCountUpdated;

        currentStage = GameStages.Idle;
    }

    private void Start()
    {
        audioManager.PlayAudio(audioClips[7]);
        TextScript.displayRadioText.Invoke(0);
        uiManager.ToggleOnButton.GetComponent<Button>().onClick.Invoke();
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

                pullCount = 0;
                capturedFish.gameObject.SetActive(false);
                capturedFish.sprite = null;
                reelSpinsToCatch = Random.Range((int)currentFish.ReelSpinsToCatch.x, (int)currentFish.ReelSpinsToCatch.y);

                characterAnimation.UpdateAnimation(currentStage);
                if (fishCollection.Count == 0)
                    return;

                if(Input.GetKeyDown(KeyCode.Space) && !TextScript.textIsBeingDisplayed)
                {
                    audioManager.PlayAudio(audioClips[0], 0.15f);

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
                    audioManager.PlayAudioLoop(audioClips[4]);

                    characterAnimation.UpdateAnimation(currentStage);

                    StartCoroutine(WaitForSecondsAndMoveToStage(Random.Range(currentFish.FishBitesBaitTime.x, currentFish.FishBitesBaitTime.y), Random.Range(0, 2) == 0 ? GameStages.Reel : GameStages.Pull));

                }
                break;
            //player pulls for X amount of times, defined in the fish data
            //  possible input -> space
            case GameStages.Pull:
                if (!waitingCoroutineIsActive)
                {
                    pullCount = 0;
                    audioManager.StopAudio();
                    audioManager.PlayAudioLoop(audioClips[5]);

                    pullController.LerpSpeed = Random.Range(currentFish.PullIndicatorSpeedRange.x, currentFish.PullIndicatorSpeedRange.y);
                    reelSystem.SetActive(false);
                    characterAnimation.UpdateAnimation(currentStage);
                    pullSystem.SetActive(true);
                    waitingCoroutineIsActive = true;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PullResult result = pullController.CheckIfIndicatorIsInContactWithSweetSpot();
                    switch (result)
                    {
                        case PullResult.Hit:
                            pullCount++;
                            if(pullCount == 1)
                                audioManager.PlayHitAndPull(audioClipsHitAndPull[1]);
                            if (pullCount == 2)
                                audioManager.PlayHitAndPull(audioClipsHitAndPull[2]);
                            if (pullCount >= 3)
                                audioManager.PlayHitAndPull(audioClipsHitAndPull[3]);

                            if(pullCount == pullsToReelAgain)
                            {
                                waitingCoroutineIsActive = false;   
                                waitAndChangeStage = StartCoroutine(WaitForSecondsAndMoveToStage(0f, GameStages.Reel));
                            }
                            break;
                        case PullResult.Miss:
                            audioManager.PlayAudio(audioClipsHitAndPull[0]);
                            currentFishMisses++;
                            break;
                        case PullResult.Bitter:
                            audioManager.PlayAudio(audioClipsHitAndPull[0]);
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
                    audioManager.StopAudio();
                    if(reelController.Friction >=0 && reelController.Friction <= 1)
                        audioManager.PlayAudioLoop(audioClips[3]);
                    if(reelController.Friction >=1 && reelController.Friction <= 4)
                        audioManager.PlayAudioLoop(audioClips[2]);
                    if(reelController.Friction >= 4 && reelController.Friction <= 10)
                        audioManager.PlayAudioLoop(audioClips[1]);


                    reelController.Friction = Random.Range(currentFish.ReelFrictionRange.x, currentFish.ReelFrictionRange.y);
                    reelSystem.SetActive(true);
                    characterAnimation.UpdateAnimation(currentStage);
                    pullSystem.SetActive(false);
                    waitAndChangeStage = StartCoroutine(WaitForSecondsAndMoveToStage(10f, GameStages.Pull));
                }
                if (wasFishCaptured)
                {
                    audioManager.StopAudio();
                    audioManager.PlayAudio(audioClips[6]);

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
                    int random = Random.Range(7, 9);
                    if (fishesCaptured >= 4)
                    {
                        if (fishesCaptured == 4)
                            Fade.fadeSky.Invoke();

                        random = 10;
                    }


                    capturedFish.sprite = currentFish.FishImage;
                    capturedFish.gameObject.SetActive(true);

                    characterAnimation.UpdateAnimation(currentStage);
                    uiManager.UnlockFish(currentFish.FishId);
                    availableFishes.Remove(currentFish);

                    if(availableFishes.Count == 0)
                    {
                        // finish game
                        var rt = capturedFish.GetComponent<RectTransform>();
                        rt.localScale = Vector3.one * 1.5f;
                        rt.anchoredPosition += new Vector2(30f, 0);
                        StartCoroutine(WaitForSecondsAndMoveToStage(3f, GameStages.GameOver));
                        break;
                    }

                    TextScript.displayRadioText.Invoke(currentFish.FishId + 1);
                    audioManager.PlayRadio(audioClips[random]);

                    fishesCaptured++;

                    ResetCurrentFish();
                    StartCoroutine(WaitForSecondsAndMoveToStage(8f, GameStages.Idle));
                }

                break;

            case GameStages.FishRanAway:
                if (!waitingCoroutineIsActive)
                {
                    audioManager.StopAudio();

                    uiManager.ToggleOnButton.SetActive(true);
                    characterAnimation.UpdateAnimation(currentStage);
                    ResetCurrentFish();
                    StartCoroutine(WaitForSecondsAndMoveToStage(5f, GameStages.Idle));
                }
                break;

            case GameStages.GameOver:

                if (!waitingCoroutineIsActive)
                {
                    waitingCoroutineIsActive = true;
                    audioManager.StopAllAudio();
                    uiManager.TurnOffCanvas();

                    StartCoroutine(GameOverFadeInBG());
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
        currentFish = availableFishes[0];
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
        if (reelSpins == 1)
            audioManager.PlayHitAndPull(audioClipsHitAndPull[4]);
        if (reelSpins == 2)
            audioManager.PlayHitAndPull(audioClipsHitAndPull[5]);
        if (reelSpins == 3)
            audioManager.PlayHitAndPull(audioClipsHitAndPull[6]);
        if (reelSpins >= 3)
            audioManager.PlayHitAndPull(audioClipsHitAndPull[7]);


        if (reelSpins >= reelSpinsToCatch) // was fish captured?
            wasFishCaptured = true;

    }

    IEnumerator GameOverFadeInBG()
    {
        gameOverOverlay.gameObject.SetActive(true);
        float elapsedTime = 0.0f;
        Color startColor = gameOverOverlay.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < gameOverFadeOutDuration)
        {
            float t = elapsedTime / gameOverFadeOutDuration;
            gameOverOverlay.color = Color.Lerp(startColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameOverOverlay.color = targetColor;
        audioManager.EndOfTheWorld();
        StartCoroutine(GameOverFadeInText());
    }
        IEnumerator GameOverFadeInText()
    {
        yield return new WaitForSeconds(5);
        gameOverText.gameObject.SetActive(true);
        float elapsedTime = 0.0f;
        Color startColor = gameOverText.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsedTime < gameOverFadeOutDuration)
        {
            float t = elapsedTime / gameOverFadeOutDuration;
            gameOverText.color = Color.Lerp(startColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameOverText.color = targetColor;

        yield return new WaitForSeconds(2);
        if(Input.anyKeyDown)
            Application.Quit();
    }

}

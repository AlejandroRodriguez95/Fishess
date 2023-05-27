using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    // Animation list:
    // Idle
    // CastRod
    // Fishing
    // AlertFishing
    // PullingRod
    // ReelingRod
    // HoldingFish

    Animator animator;

    int idle;
    int castRod;
    int fishing;
    int alertFishing;
    int pullingRod;
    int reelingRod;
    int holdingFish;

    GameStages currentStage;

    private void Awake()
    {
        idle = Animator.StringToHash("Idle");
        castRod = Animator.StringToHash("CastRod");
        fishing = Animator.StringToHash("Fishing");
        alertFishing = Animator.StringToHash("AlertFishing");
        pullingRod = Animator.StringToHash("PullingRod");
        reelingRod = Animator.StringToHash("ReelingRod");
        holdingFish = Animator.StringToHash("HoldingFish");
        animator = GetComponent<Animator>();
    }

    public void UpdateAnimation(GameStages currentStage)
    {
        switch (currentStage)
        {
            case GameStages.Idle:
                animator.Play(idle);
                break;
            case GameStages.CastingRod:
                animator.Play(castRod);
                break;
            case GameStages.WaitingForFish:
                animator.Play(fishing);
                break;
            case GameStages.FishBitRod:
                animator.Play(alertFishing);
                break;
            case GameStages.Pull:
                animator.Play(pullingRod);
                break;
            case GameStages.Reel:
                animator.Play(reelingRod);
                break;
            case GameStages.FishCaught:
                animator.Play(holdingFish);
                break;
            case GameStages.FishRanAway:
                animator.Play(idle);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStages
{
    Idle = 100,
    CastingRod = 200,
    WaitingForFish = 300,
    FishBitRod = 400,
    Pull = 500,
    Reel = 600,
    FishCaught = 700,
    FishRanAway = 800,
    GameOver = 900,
}

public enum PullResult
{
    Hit,
    Miss,
    Bitter
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "ScriptableObjects/New Fish")]

public class Fish : ScriptableObject
{
    [SerializeField]
    private string fishName;

    [SerializeField]
    private int fishId;

    [SerializeField]
    [TextArea(3, 10)]
    private string fishDescription;

    [SerializeField]
    [TextArea(3, 10)]
    private string fishLore;

    [SerializeField]
    private Sprite fishImage;

    //range of how long the player will have to wait until this fish bites the bait
    [Tooltip("Min waiting time: 3 seconds, otherwise animation will look bad")]
    [SerializeField]
    private Vector2 fishBitesBaitTime;
    [SerializeField]
    private int fishMaxHealth; // must be reduced to 0 to catch the fish
    [SerializeField]
    private int fishMaxFails; // if you miss this amount of times, fish will run away
    [SerializeField]
    Vector2 pullIndicatorSpeedRange; // how fast should the indicator move while pulling
    [SerializeField]
    Vector2 reelSpinsToCatch;
    [SerializeField]
    Vector2 reelFrictionRange;


    public string FishName { get { return fishName; } }
    public string FishDescription { get { return fishDescription; } }
    public string FishLore { get { return fishLore; } }
    public Sprite FishImage{ get { return fishImage; } }
    public int FishId { get { return fishId; } }
    public Vector2 FishBitesBaitTime { get { return fishBitesBaitTime; } }
    public int FishMaxHealth { get { return fishMaxHealth; } }
    public int FishMaxFails { get { return fishMaxFails; } }
    public Vector2 PullIndicatorSpeedRange { get { return pullIndicatorSpeedRange; } }
    public Vector2 ReelSpinsToCatch { get { return reelSpinsToCatch; } }
    public Vector2 ReelFrictionRange { get { return reelFrictionRange; } }

}
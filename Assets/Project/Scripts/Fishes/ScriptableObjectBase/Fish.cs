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

    public string FishName { get { return fishName; } }
    public string FishDescription { get { return fishDescription; } }
    public string FishLore { get { return fishLore; } }
    public Sprite FishImage{ get { return fishImage; } }
    public int FishId { get { return fishId; } }
}
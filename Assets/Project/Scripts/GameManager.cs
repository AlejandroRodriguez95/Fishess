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
    UIManager uiManager;

    private void Awake()
    {
        uiManager.UIFishCollection = fishCollection;
    }



}

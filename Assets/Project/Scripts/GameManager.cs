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
    GameObject fishJournalInUI;

    [Header("Fish showcase")]
    [SerializeField]
    Image fishImageDisplay;
    [SerializeField]
    TextMeshProUGUI fishNameDisplay;
    [SerializeField]
    TextMeshProUGUI fishDescDisplay;    
    [SerializeField]
    TextMeshProUGUI fishInfoDisplay;

    [SerializeField]
    GameObject horizontalLayoutGroupPrefab;
    [SerializeField]
    Sprite mysteryFishSprite;

    List<Transform> imageList;

    bool[] fishIsUnlocked;


    private void Start()
    {
        fishIsUnlocked = new bool[fishCollection.Count]; // here we mark the locked/unlocked fishes, index is based on fishId. false = locked fish

        imageList = new List<Transform>();
        fishCollection = fishCollection.OrderBy(data => data.FishId).ToList();

        int amountOfHorizontalGroupsToSpawn = (int)Mathf.Ceil((float)fishCollection.Count / 2f);

        for (int i=0; i < amountOfHorizontalGroupsToSpawn; i++)
        {
            var temp = Instantiate(horizontalLayoutGroupPrefab, fishJournalInUI.transform);
            imageList.Add(temp.transform.GetChild(0));
            imageList.Add(temp.transform.GetChild(1));
        }

        for (int i = 0; i < fishCollection.Count; i++)
        {
            imageList[i].GetComponent<Image>().sprite = mysteryFishSprite;
            imageList[i].gameObject.SetActive(true);
        }

        UnlockFish(2);
    }

    #region Unlocking Fishes
    private void UnlockFish(int fishId)
    {
        imageList[fishId].GetComponent<Image>().sprite = fishCollection[fishId].FishImage;
        fishIsUnlocked[fishId] = true;
        DisplayFishInfoOnUI(fishId);
    }

    private void DisplayFishInfoOnUI(int fishId)
    {
        if(fishIsUnlocked[fishId])
        {
            fishImageDisplay.sprite = fishCollection[fishId].FishImage;
            fishNameDisplay.text = fishCollection[fishId].FishName;
            fishDescDisplay.text = fishCollection[fishId].FishDescription;
            fishInfoDisplay.text = fishCollection[fishId].FishLore;
        }

        else
        {
            fishImageDisplay.sprite = fishCollection[fishId].FishImage;
            fishNameDisplay.text = "You haven't gotten this one yet!";
            fishDescDisplay.text = "";
            fishInfoDisplay.text = "";
        }

    }
    #endregion
}

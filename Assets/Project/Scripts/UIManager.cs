using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    List<Fish> uIFishCollection;


    [SerializeField]
    GameObject fishJournalInUI;
    [SerializeField]
    GameObject canvas;

    [Header("Fish showcase")]
    [SerializeField]
    Image fishImageDisplay;
    [SerializeField]
    TextMeshProUGUI fishNameDisplay;
    [SerializeField]
    TextMeshProUGUI fishDescDisplay;
    [SerializeField]
    TextMeshProUGUI fishInfoDisplay;

    [Header("Journal + selected fish info")]
    [SerializeField]
    RectTransform journal;
    [SerializeField]
    RectTransform fishInfo;
    [SerializeField]
    GameObject toggleOnButton;
    public GameObject ToggleOnButton
    {
        get { return toggleOnButton; }
    }
    [SerializeField]
    GameObject toggleOffButton;

    public GameObject ToggleOffButton
    {
        get { return toggleOffButton; }
    }

    

    [SerializeField]
    GameObject horizontalLayoutGroupPrefab;
    [SerializeField]
    Sprite mysteryFishSprite;

    List<Transform> imageList;

    #region ToggleJournalVariables

    Vector2 journalDisplayPos = new Vector2(-210, 0);
    Vector2 journalHiddenPos = new Vector2(200, 0);
    Vector2 fishInfoDisplayPos = new Vector2(-10, 0);
    Vector2 fishInfoHiddenPos = new Vector2(400, 0);
    float elapsed = 0;
    [SerializeField]
    [Tooltip("How long in seconds it takes the journal to move to the new position")]
    float duration = 1;
    #endregion

    bool[] fishIsUnlocked;

    #region getters and setters
    public List<Fish> UIFishCollection
    {
        get { return uIFishCollection; }
        set { uIFishCollection = value; }
    }
    #endregion

    private void Start()
    {
        fishIsUnlocked = new bool[uIFishCollection.Count]; // here we mark the locked/unlocked fishes, index is based on fishId. false = locked fish

        imageList = new List<Transform>();
        uIFishCollection = uIFishCollection.OrderBy(data => data.FishId).ToList();

        int amountOfHorizontalGroupsToSpawn = (int)Mathf.Ceil((float)uIFishCollection.Count / 2f);
        int fishNumber = 0;

        for (int i = 0; i < amountOfHorizontalGroupsToSpawn; i++)
        {

            var temp = Instantiate(horizontalLayoutGroupPrefab, fishJournalInUI.transform);
            var child1 = temp.transform.GetChild(0);
            var child2 = temp.transform.GetChild(1);
            child1.gameObject.name = $"{fishNumber}";
            child2.gameObject.name = $"{fishNumber + 1}";
            fishNumber += 2;

            imageList.Add(child1);
            imageList.Add(child2);
        }

        for (int i = 0; i < uIFishCollection.Count; i++)
        {
            imageList[i].GetComponent<Image>().sprite = mysteryFishSprite;
            imageList[i].gameObject.SetActive(true);
        }
    }
    #region Toggle journal on/off logic
    public void ToggleJournalOn()
    {
        StartCoroutine(DisplayJournal());
    }

    IEnumerator DisplayJournal()
    {
        if (elapsed == 0)
        {
            ToggleOnButton.SetActive(false);
            toggleOffButton.SetActive(true);
            while(journal.anchoredPosition.x != journalDisplayPos.x)
            {
                yield return new WaitForEndOfFrame();
                journal.anchoredPosition = Vector2.Lerp(journalHiddenPos, journalDisplayPos, elapsed);
                fishInfo.anchoredPosition = Vector2.Lerp(fishInfoHiddenPos, fishInfoDisplayPos, elapsed);
                elapsed += Time.deltaTime/duration;

            }
            elapsed = 0;
        }
    }    
    public void ToggleJournalOff()
    {
        StartCoroutine(HideJournal());
    }

    IEnumerator HideJournal()
    {
        if(elapsed == 0)
        {
            toggleOffButton.SetActive(false);
            ToggleOnButton.SetActive(true);
            while(journal.anchoredPosition.x != journalHiddenPos.x)
            {
                yield return new WaitForEndOfFrame();
                journal.anchoredPosition = Vector2.Lerp(journalDisplayPos, journalHiddenPos, elapsed);
                fishInfo.anchoredPosition = Vector2.Lerp(fishInfoDisplayPos, fishInfoHiddenPos, elapsed);
                elapsed += Time.deltaTime/duration;
            }
            elapsed = 0;
        }
    }
    #endregion

    #region Unlocking Fishes
    public void UnlockFish(int fishId)
    {
        imageList[fishId].GetComponent<Image>().sprite = uIFishCollection[fishId].FishImage;
        fishIsUnlocked[fishId] = true;
        DisplayFishInfoOnUI(fishId);
    }

    public void DisplayFishInfoOnUI(int fishId)
    {
        if (fishIsUnlocked[fishId])
        {
            fishImageDisplay.sprite = uIFishCollection[fishId].FishImage;
            fishNameDisplay.text = uIFishCollection[fishId].FishName;
            fishDescDisplay.text = uIFishCollection[fishId].FishDescription;
            fishInfoDisplay.text = uIFishCollection[fishId].FishLore;
        }

        else
        {
            fishImageDisplay.sprite = mysteryFishSprite;
            fishNameDisplay.text = "You haven't gotten this one yet!";
            fishDescDisplay.text = "";
            fishInfoDisplay.text = "";
        }

    }

    public void TurnOffCanvas()
    {
        canvas.SetActive(false);
    }
    #endregion
}

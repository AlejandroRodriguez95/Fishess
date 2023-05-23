using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Fish types")]
    [Tooltip("Add all the different type of fishes here! " +
        "You can find them in the fishTypes folder." +
        "\nTo create new fishes: \n" +
        "1) right click on the project view -> Create -> ScriptableObjects -> new fish\n" +
        "2) fill the values on the new file\n" +
        "3) drag this new file to this collection in the inspector.")]
    [SerializeField]
    List<Fish> fishCollection;


    [SerializeField]
    GameObject fishJournalInUI;
    [SerializeField]
    GameObject horizontalLayoutGroupPrefab;

    List<Transform> imageList;


    private void Start()
    {
        imageList = new List<Transform>();

        int amountOfHorizontalGroupsToSpawn = (int)Mathf.Ceil((float)fishCollection.Count / 2f);

        for (int i=0; i < amountOfHorizontalGroupsToSpawn; i++)
        {
            var temp = Instantiate(horizontalLayoutGroupPrefab, fishJournalInUI.transform);
            imageList.Add(temp.transform.GetChild(0));
            imageList.Add(temp.transform.GetChild(1));
        }

        for (int i = 0; i < fishCollection.Count; i++)
        {
            imageList[i].GetComponent<Image>().sprite = fishCollection[i].FishImage;
            imageList[i].gameObject.SetActive(true);
        }
    }

}

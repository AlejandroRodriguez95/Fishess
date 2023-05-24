using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalEntryBehavior : MonoBehaviour
{
    [SerializeField]
    UIManager uIManager;

    private void Awake()
    {
        uIManager = FindObjectOfType<UIManager>();
    }
    public void OnClick()
    {
        uIManager.DisplayFishInfoOnUI(int.Parse(gameObject.name));
    }
}

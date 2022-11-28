using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionMenuManager : MonoBehaviour
{
    public GameObject missionsList;
    public GameObject dialogueObject;
    public TextMeshProUGUI dialogueTxt;
    public string[] messages;

    void Start() {
        dialogueTxt.text = messages[(PlayerPrefs.GetInt("Day")-1) % messages.Length];
    }

    public void CloseMessage() {
        dialogueObject.SetActive(false);
        missionsList.SetActive(true);
    }
}

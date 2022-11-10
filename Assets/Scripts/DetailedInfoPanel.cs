using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetailedInfoPanel : MonoBehaviour
{
    public TextMeshProUGUI princessTxt;
    public Transform diffParent;
    public TextMeshProUGUI daysTxt;
    public TextMeshProUGUI storyTxt;
    public TextMeshProUGUI locationTxt;
    public TextMeshProUGUI timeTxt;
    public TextMeshProUGUI rewardTxt;
    private MissionInfo info;
    public void DisplayInfo(MissionInfo info) {
        this.info = info;

        princessTxt.text = info.princess;
        for (int i=0; i< diffParent.childCount; i++) {
            diffParent.GetChild(i).gameObject.SetActive(i < info.diff);
        }
        daysTxt.text = "" + info.days;
        storyTxt.text = info.story;
        locationTxt.text = info.map;
        timeTxt.text = info.time;
        if (info.coins > 0)
            rewardTxt.text = "" + info.coins + "\n";
        foreach (string rew in info.items)
            rewardTxt.text = rew + "\n";
    }

    public void LaunchCurrentMission() {
        FindObjectOfType<GameManager>().LaunchMission(info);
    }
}

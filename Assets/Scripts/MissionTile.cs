using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionTile : MonoBehaviour
{
    public TextMeshProUGUI princessTxt;
    public Transform diffParent;
    public TextMeshProUGUI daysTxt;
    private MissionInfo info;
    public void SetInfo(MissionInfo info) {
        this.info = info;

        princessTxt.text = info.princess;
        for (int i=0; i< diffParent.childCount; i++) {
            diffParent.GetChild(i).gameObject.SetActive(i < info.diff);
        }
        daysTxt.text = "" + info.days;
    }

    public void ShowDetailedInfo() {
        FindObjectOfType<GameManager>().detailedInfoScroll.SetActive(true);
        FindObjectOfType<DetailedInfoPanel>().DisplayInfo(info);
    }
}

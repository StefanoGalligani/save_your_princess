using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class PowerupsMenu : MonoBehaviour
{
    public TextMeshProUGUI floorsTxt;
    public TextMeshProUGUI floorsPriceTxt;
    public TextMeshProUGUI assistantsTxt;
    public TextMeshProUGUI assistantsPriceTxt;
    public TextMeshProUGUI upgradesTxt;
    public TextMeshProUGUI upgradesPriceTxt;

    public int[] floorsPrices;
    public int[] assistantsPrices;
    public int[] upgradesPrices;
    public Transform buildingSprites;

    public void UpdateTexts(int floors, int assistants, int upgrades) {
        floorsTxt.text = "" + floors;
        assistantsTxt.text = "" + assistants;
        upgradesTxt.text = "" + upgrades;
        if (floors < floorsPrices.Length)
            floorsPriceTxt.text = "" + floorsPrices[floors];
        else
            floorsPriceTxt.text = "";
        if (assistants < floors*2)
            assistantsPriceTxt.text = "" + assistantsPrices[assistants];
        else
            assistantsPriceTxt.text = "";
        if (upgrades < upgradesPrices.Length)
            upgradesPriceTxt.text = "" + upgradesPrices[upgrades];
        else
            upgradesPriceTxt.text = "";
        
        for (int i=0; i<4; i++) {
            buildingSprites.GetChild(i).gameObject.SetActive(i==floors-1);
        }
    }

    public void BuyPowerup(int powerup) {
        int[] prices = null;
        if (powerup == 0)
            prices = floorsPrices;
        if (powerup == 1)
            prices = assistantsPrices;
        if (powerup == 2)
            prices = upgradesPrices;
        FindObjectOfType<GameManager>().AcceptPowerup(powerup, prices);
    }
}

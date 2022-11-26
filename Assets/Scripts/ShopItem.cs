using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    bool sell;
    string itemName;

    public void SetType(bool sell, string name) {
        this.sell = sell;
        itemName = name;
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itemName;
        int price = ItemsDictionary.GetInstance().GetItemPrice(itemName);
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "" + (sell ? price/2 : price) + " coins";
        transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text =
            sell ? "Sell" : "Buy";
    }

    public void Selected() {
        if (sell) {
            FindObjectOfType<GameManager>().SoldItem(itemName);
            Destroy(gameObject);
        } else {
            bool bought = FindObjectOfType<GameManager>().BuyItem(itemName);
            if (bought) Destroy(gameObject);
        }
    }
}

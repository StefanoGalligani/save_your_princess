using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShopManager : MonoBehaviour
{
    public GameObject shopItem;
    public RectTransform sellPanel;
    public RectTransform buyPanel;

    void Start() {
        string temp = Application.persistentDataPath;
        string inventoryPath = Path.Combine(temp, "inventory.txt");

        TextReader trInv = new StreamReader(inventoryPath); //read inventory
        List<string> inventory = new List<string>();
        string line;
        while ((line = trInv.ReadLine()) != null && line != "") {
            inventory.Add(line);
        }
        trInv.Close();

        foreach(string s in inventory) {
            GameObject instShopItem = Instantiate(shopItem, sellPanel);
            instShopItem.GetComponent<ShopItem>().SetType(true, s);
        }

        sellPanel.sizeDelta = new Vector2(sellPanel.sizeDelta.x, 200*inventory.Count);

        string[] items = new string[Random.Range(1, FindObjectOfType<GameManager>().upgrades+1)];
        for (int i=0; i<items.Length; i++) {
            items[i] = ItemsDictionary.GetInstance().GetRandomItemOfRarityUpTo(FindObjectOfType<GameManager>().assistants);
        }

        foreach(string s in items) {
            GameObject instShopItem = Instantiate(shopItem, buyPanel);
            instShopItem.GetComponent<ShopItem>().SetType(false, s);
        }

        buyPanel.sizeDelta = new Vector2(buyPanel.sizeDelta.x, 200*items.Length);
    }

    public void AddItemToSellList(string name) {
        GameObject instShopItem = Instantiate(shopItem, sellPanel);
        instShopItem.GetComponent<ShopItem>().SetType(true, name);

        sellPanel.sizeDelta = new Vector2(sellPanel.sizeDelta.x, 200 + sellPanel.sizeDelta.y);
    }

    public void AddItemToBuyList(string name) {
        GameObject instShopItem = Instantiate(shopItem, buyPanel);
        instShopItem.GetComponent<ShopItem>().SetType(false, name);

        buyPanel.sizeDelta = new Vector2(buyPanel.sizeDelta.x, 200 + buyPanel.sizeDelta.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsDictionary
{
    private static ItemsDictionary instance;
    private Dictionary<string, int> itemsPrices;
    private Dictionary<string, int> itemsStats;
    private Dictionary<string, int> itemsRarity;


    private ItemsDictionary() {
        itemsPrices = new Dictionary<string, int>();
        itemsStats = new Dictionary<string, int>();
        itemsRarity = new Dictionary<string, int>();
        AddItem("Weapon_Sword", 10, 2);
        AddItem("Weapon_Spear", 15, 4, 1);
        AddItem("Weapon_LongSword", 20, 7, 2);
        AddItem("Helmet_Leather", 5, 2);
        AddItem("Helmet_Iron", 8, 3, 2);
        AddItem("Helmet_Ancestral", 8, 5, 4);
        AddItem("Chestplate_Leather", 8, 3);
        AddItem("Chestplate_Iron", 12, 5, 2);
        AddItem("Chestplate_Ancestral", 30, 10, 6);
        AddItem("Boots_Leather", 7, 4);
        AddItem("Boots_Iron", 15, 9, 3);
        AddItem("Boots_Ancestral", 50, 13, 8);
        AddItem("Boots_HighJump", 40, 8, 5);
        AddItem("Weapon_AirSword", 30, 2, 7);
        AddItem("Weapon_VampireSword", 30, 3, 7);
        AddItem("Weapon_MagicRod", 30, 5, 7);
        AddItem("Talisman_Life", 50, 2, 8);
        AddItem("Talisman_Protection", 30, 10, 5);
        AddItem("Ring_Stun", 20, 2, 2);
        AddItem("Ring_SlowTime", 30, 2, 4);
        AddItem("Weapon_Fist", 0, 1, 10);
    }

    private void AddItem(string name, int price, int stat, int rarity=0) {
        itemsPrices.Add(name, price);
        itemsStats.Add(name, stat);
        itemsRarity.Add(name, rarity);
    }

    public string GetRandomItemOfRarity(int r) {
        List<string> viableItems = new List<string>();
        for (int i=0; i<itemsRarity.Count; i++) {
            if (itemsRarity.ElementAt(i).Value == r) viableItems.Add(itemsStats.ElementAt(i).Key);
        }
        return viableItems.ElementAt(Random.Range(0, viableItems.Count));
    }
    
    public string GetRandomItemOfRarityUpTo(int r) {
        List<string> viableItems = new List<string>();
        for (int i=0; i<itemsRarity.Count; i++) {
            if (itemsRarity.ElementAt(i).Value <= r) viableItems.Add(itemsStats.ElementAt(i).Key);
        }
        return viableItems.ElementAt(Random.Range(0, viableItems.Count));
    }

    public int GetItemPrice(string item) {
        return itemsPrices.GetValueOrDefault(item);
    }

    public int GetItemStat(string item) {
        return itemsStats.GetValueOrDefault(item);
    }

    public int GetItemRarity(string item) {
        return itemsRarity.GetValueOrDefault(item);
    }

    public static ItemsDictionary GetInstance() {
        if (instance == null) instance = new ItemsDictionary();
        return instance;
    }
}

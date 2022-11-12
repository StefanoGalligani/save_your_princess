using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsDictionary
{
    private static ItemsDictionary instance;
    private Dictionary<string, int> items;

    private ItemsDictionary() {
        items = new Dictionary<string, int>();
        items.Add("Weapon_Sword", 10);
        items.Add("Helment_Leather", 5);
        items.Add("Chestplate_Leather", 8);
        items.Add("Boots_Leather", 7);
        items.Add("Talisman_Health", 14);
        items.Add("Ring_Stun", 20);
    }

    public static ItemsDictionary GetInstance() {
        if (instance == null) instance = new ItemsDictionary();
        return instance;
    }
}

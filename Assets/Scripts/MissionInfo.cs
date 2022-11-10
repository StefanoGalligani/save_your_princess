using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionInfo
{
    public string princess;
    public int diff;
    public int days;
    public string map;
    public string time;
    public int coins;
    public string[] items;
    public string story;

    public override string ToString() {
        string str =
            "Princess:" + princess + ";" +
            "Diff:" + diff + ";" +
            "Days:" + days + ";" +
            "Map:" + map + ";" +
            "Time:" + time + ";" +
            "Coins:" + coins + ";" +
            "Items:";
        for (int i = 0; i < items.Length; i++) {
            str += items[i];
            if (i + 1 < items.Length) str += ",";
        }
        str += ";Story:" + story;
        return str;
    }

    public static MissionInfo FromString(string str) {
        MissionInfo m = new MissionInfo();
        string[] infos = str.Split(";");
        m.princess = infos[0].Split(":")[1];
        m.diff = int.Parse(infos[1].Split(":")[1]);
        m.days = int.Parse(infos[2].Split(":")[1]);
        m.map = infos[3].Split(":")[1];
        m.time = infos[4].Split(":")[1];
        m.coins = int.Parse(infos[5].Split(":")[1]);
        m.items = infos[6].Split(":")[1].Split(",");
        m.story = infos[7].Split(":")[1];
        return m;
    }
}

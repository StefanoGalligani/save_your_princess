using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
    public RectTransform missionContainer;
    public GameObject missionTile;
    public GameObject detailedInfoScroll;
    private int missionsPerDay = 2;
    private List<MissionInfo> missions;
    private string listPath;
    private string infoPath;
    private string coinsPath;
    private string inventoryPath;
    private string equipmentPath;
    private string buildingPowerupsPath;

    void Start() {
        string temp = Application.persistentDataPath;
        listPath = Path.Combine(temp, "missionsList.txt");
        infoPath = Path.Combine(temp, "missionInfo.txt");
        coinsPath = Path.Combine(temp, "coins.txt");
        inventoryPath = Path.Combine(temp, "inventory.txt");
        equipmentPath = Path.Combine(temp, "equipment.txt");
        buildingPowerupsPath = Path.Combine(temp, "building.txt");

        detailedInfoScroll.SetActive(false);
        missions = new List<MissionInfo>();

        WriteNewMissions();
        ShowMissions();
        InitInventory();
    }

    private void InitInventory() {
        if (!File.Exists(coinsPath)) {
            File.Create(coinsPath).Close();

            TextWriter tw = new StreamWriter(coinsPath);
            tw.WriteLine("10");
            tw.Close();
        }
        if (!File.Exists(inventoryPath)) {
            File.Create(inventoryPath).Close();

            TextWriter tw = new StreamWriter(inventoryPath);
            string line = "Weapon_Sword\nChestplate_Leather";
            tw.WriteLine(line);
            tw.Close();
        }
        if (!File.Exists(equipmentPath)) {
            File.Create(equipmentPath).Close();

            TextWriter tw = new StreamWriter(equipmentPath);
            string line = "Weapon_Sword\nChestplate_Leather";
            tw.WriteLine(line);
            tw.Close();
        }
        if (!File.Exists(buildingPowerupsPath)) {
            File.Create(buildingPowerupsPath).Close();

            TextWriter tw = new StreamWriter(buildingPowerupsPath);
            tw.WriteLine("Floors:1;Assistants:1;Upgrades:0");
            tw.Close();
        }
    }

    private void ShowMissions() {
        if (File.Exists(listPath)) {
            TextReader tw = new StreamReader(listPath);
            string str;
            int i=0;
            while ((str = tw.ReadLine())!= null && str.Length > 2) { //hacky
                MissionInfo m = MissionInfo.FromString(str);
                missions.Add(m);
                GameObject tile = Instantiate(missionTile, missionContainer);
                tile.GetComponent<MissionTile>().SetInfo(m);
                i++;
            }

            missionContainer.sizeDelta = new Vector2(0, 350 * i + 50);
        }
    }

    private void WriteNewMissions() {
        
        if (!File.Exists(listPath))
        {
            FileStream stream = File.Create(listPath);
            stream.Close();

            TextWriter tw = new StreamWriter(listPath);
            string line = "Name:Jasmine;Diff:1;Days:5;Map:Forest;Time:Noon;Coins:100;Items:Boots,Sword;Story:prova";
            tw.WriteLine(line);
            tw.Close();
        }
        else if (File.Exists(listPath))
        {
            if (File.Exists(infoPath)) {
                File.Delete(infoPath); //prima di eliminarlo va letta la missione per le ricompense
                TextWriter tw = new StreamWriter(listPath, true);
                for (int i=0; i<missionsPerDay; i++) {
                    string line = GenerateMission();
                    tw.WriteLine(line);
                }
                tw.Close();
            }
        }
    }

    private string GenerateMission() {
        string m = "Name:Eliza;Diff:2;Days:3;Map:Tower;Time:Dusk;Coins:20;Items:;Story:abcde";
        return m;
    }

    public void LaunchMission(MissionInfo mission) {
        missions.Remove(mission);
        foreach(MissionInfo m in missions.ToArray()) { //remove missions that have expired
            m.days--;
            if (m.days == 0) missions.Remove(m);
        }

        FileStream stream = File.Create(listPath);
        stream.Close();
        TextWriter tw = new StreamWriter(listPath);
        foreach(MissionInfo m in missions) { //rewrite missions
            tw.WriteLine(m.ToString());
        }
        tw.Close();

        stream = File.Create(infoPath);
        stream.Close();
        tw = new StreamWriter(infoPath);
        tw.WriteLine(mission.ToString()); //write current mission info
        tw.Close();
        
        SceneManager.LoadScene("Mission");
    }
}

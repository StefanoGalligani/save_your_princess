using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] menus;
    public RectTransform missionContainer;
    public RectTransform equipmentContainer;
    public GameObject missionTile;
    public GameObject detailedInfoScroll;
    private int missionsPerDay = 1;
    private int floor = 1;
    private int assistants = 1;
    private int upgrades = 0;
    private List<MissionInfo> missions;
    private string listPath;
    private string infoPath;
    private string coinsPath;
    private string inventoryPath;
    private string equipmentPath;
    private string buildingPowerupsPath;
    private string resultPath;

    void Start() {
        string temp = Application.persistentDataPath;
        listPath = Path.Combine(temp, "missionsList.txt");
        infoPath = Path.Combine(temp, "missionInfo.txt");
        coinsPath = Path.Combine(temp, "coins.txt");
        inventoryPath = Path.Combine(temp, "inventory.txt");
        equipmentPath = Path.Combine(temp, "equipment.txt");
        buildingPowerupsPath = Path.Combine(temp, "building.txt");
        resultPath = Path.Combine(temp, "result.txt");

        detailedInfoScroll.SetActive(false);
        missions = new List<MissionInfo>();

        InitPowerups();
        InitInventory();
        ShowEquipment();
        WriteNewMissions();
        ShowMissions();
        ChangeMenu(0);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void InitPowerups() {
        if (!File.Exists(buildingPowerupsPath)) {
            File.Create(buildingPowerupsPath).Close();

            TextWriter tw = new StreamWriter(buildingPowerupsPath);
            tw.WriteLine("Floors:1;Assistants:1;Upgrades:0");
            tw.Close();
        } else {
            TextReader tr = new StreamReader(buildingPowerupsPath);
            string[] lines = tr.ReadLine().Split(";");

            foreach(string l in lines) {
                string[] splits = l.Split(':');
                switch (splits[0]) {
                    case "Floors":
                        floor = int.Parse(splits[1]);
                    break;
                    case "Assistants":
                        assistants = int.Parse(splits[1]);
                    break;
                    case "Upgrades":
                        upgrades = int.Parse(splits[1]);
                    break;
                }
            }
            tr.Close();
        }
        //mostrare nei dropdown i powerup dell'edificio
        int day = PlayerPrefs.GetInt("Day");
        PlayerPrefs.SetInt("Day", ++day);
        for (int i=1; i<= upgrades; i++) {
            if (day%(upgrades+1) == 0) missionsPerDay++;
        }
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
            string line = "Helmet:;Chestplate:Chestplate_Leather;Boots:;Weapon:Weapon_Sword;Talisman:;Ring:";
            tw.WriteLine(line);
            tw.Close();
        }
    }

    public void ShowEquipment() {
        foreach(Transform child in equipmentContainer) {
            if(!child.name.StartsWith("Line")) continue;

            child.GetComponentInChildren<TMP_Dropdown>().ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            options.Add(new TMP_Dropdown.OptionData("Nothing"));

            TextReader tr = new StreamReader(inventoryPath);
            string line;
            while ((line = tr.ReadLine()) != null && line != "") {
                string[] item = line.Split("_");
                if (item[0].Equals(child.GetChild(0).GetComponent<TextMeshProUGUI>().text)) {
                    options.Add(new TMP_Dropdown.OptionData(item[1]));
                }
            }
            child.GetComponentInChildren<TMP_Dropdown>().AddOptions(options);
            tr.Close();

            tr = new StreamReader(equipmentPath);
            string[] lines = tr.ReadLine().Split(";");
            foreach(string l in lines) {
                string[] splits = l.Split(':');
                if (splits[1]!="" && splits[0].Equals(child.GetChild(0).GetComponent<TextMeshProUGUI>().text)) {
                    for(int i=0; i< child.GetComponentInChildren<TMP_Dropdown>().options.Count; i++) {
                        if (splits[1].Split("_")[1].Equals(child.GetComponentInChildren<TMP_Dropdown>().options[i].text)) {
                            child.GetComponentInChildren<TMP_Dropdown>().value = i;
                            break;
                        }
                    }
                    child.GetComponentInChildren<TMP_Dropdown>().RefreshShownValue();
                }
            }
            tr.Close();
        }
    }

    private void ShowMissions() {
        if (File.Exists(listPath)) {
            TextReader tr = new StreamReader(listPath);
            string str;
            int i=0;
            while ((str = tr.ReadLine())!= null && str.Length > 2) { //hacky
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
            File.Create(listPath).Close();

            TextWriter tw = new StreamWriter(listPath);
            string line = "Name:Jasmine;Diff:1;Days:5;Map:Forest;Time:Noon;Coins:100;Items:Boots,Sword;Story:prova";
            tw.WriteLine(line);
            tw.Close();
        }
        else if (File.Exists(infoPath))
        {
            File.Delete(infoPath); //prima di eliminarlo va letta la missione per le ricompense
            TextWriter tw = new StreamWriter(listPath, true);
            for (int i=0; i<missionsPerDay; i++) {
                string line = GenerateMission();
                tw.WriteLine(line);
            }
            tw.Close();
        }
    }

    private string GenerateMission() {
        MissionInfo m = new MissionInfo();
        switch(Random.Range(0,5)) { //todo: read random name from a file
            case (0):
                m.princess = "Emily";
                break;
            case (1):
                m.princess = "Jessamine";
                break;
            case (2):
                m.princess = "Anastasia";
                break;
            case (3):
                m.princess = "Constantine";
                break;
            case (4):
                m.princess = "Beatrice";
                break;
        }
        m.diff = Mathf.Min(4, Random.Range(1, assistants/2+1));
        m.days = assistants/2+1;
        switch(Random.Range(0,4)) {
            case (0):
                m.map = "Tower";
                break;
            case (1):
                m.map = "Forest";
                break;
            case (2):
                m.map = "Castle";
                break;
            case (3):
                m.map = "Cavern";
                break;
        }
        switch(Random.Range(0,3)) {
            case (0):
                m.time = "Noon";
                break;
            case (1):
                m.time = "Dusk";
                break;
            case (2):
                m.time = "Night";
                break;
        }
        m.coins = assistants*2 + Random.Range(-assistants, +assistants);
        m.items = new string[]{""};
        m.story = "story";
        return m.ToString();
    }

    public void LaunchMission(MissionInfo mission) {
        File.Create(equipmentPath).Close();
        TextWriter tw = new StreamWriter(equipmentPath);
        string line="";
        foreach(Transform child in equipmentContainer) {
            if(!child.name.StartsWith("Line")) continue;

            line += child.GetChild(0).GetComponent<TextMeshProUGUI>().text + ":";
            string equipped = child.GetChild(1).GetComponent<TMP_Dropdown>().options[child.GetChild(1).GetComponent<TMP_Dropdown>().value].text;
            if (equipped != "Nothing") line += child.GetChild(0).GetComponent<TextMeshProUGUI>().text + "_" + equipped;
            if (child.GetSiblingIndex() != equipmentContainer.childCount-1) line += ";";
        }
        tw.Write(line);
        tw.Close();

        missions.Remove(mission);
        for (int i=0; i<missions.Count; i++) { //remove missions that have expired
            missions[i].days -= 1;
            if (missions[i].days == 0) {
                missions.RemoveAt(i);
                i--;
            }
        }

        File.Create(listPath).Close();
        tw = new StreamWriter(listPath);
        foreach(MissionInfo m in missions) { //rewrite missions
            tw.WriteLine(m.ToString());
        }
        tw.Close();

        File.Create(infoPath).Close();
        tw = new StreamWriter(infoPath);
        tw.WriteLine(mission.ToString()); //write current mission info
        tw.Close();
        
        SceneManager.LoadScene("Mission");
    }

    public void ChangeMenu(int m) {
        for (int i=0; i<menus.Length; i++) {
            menus[i].SetActive(i==m);
        }
    }
}

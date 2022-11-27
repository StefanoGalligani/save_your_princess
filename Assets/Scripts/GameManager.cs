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
    public TextMeshProUGUI coinsLabel;
    public TextMeshProUGUI coinsText;
    public GameObject missionTile;
    public GameObject detailedInfoScroll;
    public GameObject powerupsMenu;
    private int missionsPerDay = 1;
    private int floor = 1;
    public int assistants {private set; get;} = 1;
    public int upgrades {private set; get;} = 0;
    private int coins = 0;
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
        WriteNewMissions();
        InitInventory();
        ShowEquipment();
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
        int day = PlayerPrefs.GetInt("Day");
        PlayerPrefs.SetInt("Day", ++day);
        for (int i=1; i<= upgrades; i++) {
            if (day%(upgrades+1) == 0) missionsPerDay++;
        }

        powerupsMenu.GetComponent<PowerupsMenu>().UpdateTexts(floor, assistants, upgrades);
    }

    public void AcceptPowerup(int powerup, int[] prices) {
        if (powerup == 0) {
            if (floor >= prices.Length || coins < prices[floor]) return;
            coins -= prices[floor];
            floor++;
        }
        
        if (powerup == 1) {
            if (assistants >= floor*2 || coins < prices[assistants]) return;
            coins -= prices[assistants];
            assistants++;
        }
        
        if (powerup == 2) {
            if (upgrades >= prices.Length || coins < prices[upgrades]) return;
            coins -= prices[upgrades];
            upgrades++;
        }
        coinsText.text=""+coins;
        UpdateCoins();
        
        File.Create(buildingPowerupsPath).Close();
        TextWriter tw = new StreamWriter(buildingPowerupsPath);
        tw.WriteLine("Floors:" + floor + ";Assistants:" + assistants + ";Upgrades:" + upgrades);
        tw.Close();

        powerupsMenu.GetComponent<PowerupsMenu>().UpdateTexts(floor, assistants, upgrades);
    }

    private void InitInventory() {
        if (!File.Exists(coinsPath)) {
            File.Create(coinsPath).Close();

            TextWriter tw = new StreamWriter(coinsPath);
            tw.WriteLine("10");
            coins = 10;
            tw.Close();
        } else {
            TextReader tr = new StreamReader(coinsPath);
            coins = int.Parse(tr.ReadLine());
            tr.Close();
        }
        coinsText.text=""+coins;

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

    private void UpdateCoins() {
        File.Create(coinsPath).Close();

        TextWriter tw = new StreamWriter(coinsPath);
        tw.WriteLine(""+coins);
        tw.Close();

        coinsText.text = ""+coins;
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
            string line = "Name:Jasmine;Diff:1;Days:5;Map:Forest;Time:Noon;Coins:100;Items:Boots_Leather,Sword;Story:prova";
            tw.WriteLine(line);
            tw.Close();
        }
        else if (File.Exists(infoPath))
        {
            WinRewards();
            File.Delete(infoPath);
            TextWriter tw = new StreamWriter(listPath, true);
            for (int i=0; i<missionsPerDay; i++) {
                string line = GenerateMission();
                tw.WriteLine(line);
            }
            tw.Close();
        }
    }

    private void WinRewards() {
        if (!File.Exists(resultPath)) return;
        TextReader trRes = new StreamReader(resultPath);
        string res = trRes.ReadLine();
        trRes.Close();
        if (res != "Win") return;

        TextReader tr = new StreamReader(infoPath);
        MissionInfo m = MissionInfo.FromString(tr.ReadLine());
        tr.Close();

        coins += m.coins;
        UpdateCoins();

        TextWriter tw = new StreamWriter(inventoryPath, true);
        for (int i=0; i<m.items.Length; i++) {
            tw.WriteLine(m.items[i]);
        }
        tw.Close();
    }

    private string GenerateMission() {
        MissionInfo m = new MissionInfo();
        string[] princesses = {"Emily", "Jessamine", "Anastasia", "Constantine", "Beatrice"};
        m.princess = princesses[Random.Range(0, princesses.Length)];
        m.diff = Mathf.Min(4, Random.Range(1, assistants/2+1));
        m.days = Random.Range(1, assistants/2+1);
        switch(Random.Range(0,3)) {
            case (0):
                m.map = "Tower";
                break;
            case (1):
                m.map = "Forest";
                break;
            case (2):
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
        m.coins = upgrades*3 + assistants*2 + Random.Range(-assistants, +assistants);
        m.items = new string[Random.Range(0, upgrades+1)];
        for (int i=0; i<m.items.Length; i++) {
            m.items[i] = ItemsDictionary.GetInstance().GetRandomItemOfRarityUpTo(assistants);
        }
        m.story = GenerateStory(m.princess);
        return m.ToString();
    }

    private string GenerateStory(string name) {
        string story = "";
        string[] activities = {"just minding her business", "having a swim", "studying game developement", "reading a book about social psychology", "explaining her butler why the earth is flat"};
        string activity = activities[Random.Range(0, activities.Length)];
        string[] beginning = {"While princess " + name + " was " + activity + ", ", "Princess " + name + " was " + activity + " when "};
        string[] kidnappers = {"a goblin gang", "an evil lord", "some guy who was passing by", "a not so friedly ogre", "an actual ghost", "unexpected consequences", "her dreams", "nobody in particular", "the decadence of society", "the wrong path", "her friend who was secretly a kidnapper", "five invisible dwarves", "a possessed pony", "a british rock band of the 70s"};
        story = beginning[Random.Range(0, beginning.Length)] + kidnappers[Random.Range(0, kidnappers.Length)] + " took her away from her castle";
        return story;
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
        coinsText.gameObject.SetActive(m==3 || m==4);
        coinsLabel.gameObject.SetActive(m==3 || m==4);
    }

    public void SoldItem(string name) {
        coins += ItemsDictionary.GetInstance().GetItemPrice(name)/2;
        UpdateCoins();

        TextReader trInv = new StreamReader(inventoryPath); //read inventory
        List<string> inventory = new List<string>();
        string line;
        while ((line = trInv.ReadLine()) != null && line != "") {
            inventory.Add(line);
        }
        trInv.Close();

        foreach(string s in inventory) {
            if (s.Equals(name)) { inventory.Remove(s); break; }
        }

        TextWriter twInv = new StreamWriter(inventoryPath); //write new inventory
        foreach(string s in inventory) {
            twInv.WriteLine(s);
        }
        twInv.Close();
        
        ShowEquipment();
        FindObjectOfType<ShopManager>().AddItemToBuyList(name);
    }

    public bool BuyItem(string name) {
        if (coins < ItemsDictionary.GetInstance().GetItemPrice(name)) return false;
        coins -= ItemsDictionary.GetInstance().GetItemPrice(name);
        UpdateCoins();
        
        TextWriter twInv = new StreamWriter(inventoryPath, true); //write new inventory
        twInv.WriteLine(name);
        twInv.Close();

        ShowEquipment();

        FindObjectOfType<ShopManager>().AddItemToSellList(name);
        
        return true;
    }
}

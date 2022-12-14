using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class MissionManager : MonoBehaviour
{
    public string[] weaponNames;
    public GameObject[] weaponPrefabs;
    public Light sun;
    public GameObject[] maps;
    public GameObject[] skyContainers;
    public GameObject pausePanel;
    public int diff {get; private set;}
    public string time {get; private set;}
    public int def {get; private set;} = 0;
    public GameObject player;
    public GameObject lifeImg;
    public AudioClip[] soundtracks;
    private string inventoryPath;
    private string equipmentPath;
    private string resultPath;
    private bool paused = false;
    private float currentTimeScale = 1;
    private bool secondLife = false;
    private int ring = 0;

    void Start() {
        string temp = Application.persistentDataPath;
        string sPath = Path.Combine(temp, "missionInfo.txt");
        inventoryPath = Path.Combine(temp, "inventory.txt");
        equipmentPath = Path.Combine(temp, "equipment.txt");
        resultPath = Path.Combine(temp, "result.txt");

        bool fileExist = File.Exists(sPath);
        if (fileExist) {
            Setup(sPath);
        }
        else {
            Debug.Log("Missing file");
            SceneManager.LoadScene("Office");
        }

        GetComponent<AudioSource>().clip = soundtracks[Random.Range(0, soundtracks.Length)];
        GetComponent<AudioSource>().Play();

        pausePanel.SetActive(false);
        SetupEquipment();
    }

    private void SetupEquipment() {
        TextReader tr = new StreamReader(equipmentPath);
        string[] lines = tr.ReadLine().Split(";");
        foreach(string l in lines) {
            string[] splits = l.Split(':');
            if (splits[1]!="" || splits[0]=="Weapon") {
                if(splits[1] == "") splits[1] = "Weapon_Fist";
                int stat = ItemsDictionary.GetInstance().GetItemStat(splits[1]);
                switch (splits[0]) {
                    case "Helmet":
                        def+=stat;
                        break;
                    case "Chestplate":
                        def+=stat;
                        break;
                    case "Boots":
                        FindObjectOfType<MyCharacterController>().speed = stat;
                        if (splits[1] == "Boots_HighJump") FindObjectOfType<MyCharacterController>().jumpForce = 8;
                        break;
                    case "Weapon":
                        GameObject w = weaponPrefabs[System.Array.IndexOf(weaponNames, splits[1])];
                        GameObject wInstance = Instantiate(w, player.transform.GetChild(0).GetChild(1));
                        wInstance.GetComponent<Weapon>().damage = stat;
                        player.GetComponent<CombatController>().SetWeapon(wInstance.GetComponent<Weapon>());
                        break;
                    case "Talisman":
                        if (splits[1] == "Talisman_Protection") def += stat;
                        if (splits[1] == "Talisman_Life") { secondLife = true; lifeImg.SetActive(true); }
                        break;
                    case "Ring":
                        if (splits[1] == "Ring_Stun") ring = 1;
                        if (splits[1] == "Ring_SlowTime") ring = 2;
                        break;
                }
            }
        }
        tr.Close();
    }

    private void Setup(string sPath) {
        TextReader tr = new StreamReader(sPath);
        string[] lines = tr.ReadLine().Split(";");

        foreach(string l in lines) {
            string[] splits = l.Split(':');
            switch (splits[0]) {
                case "Diff":
                    diff = int.Parse(splits[1]);
                    break;
                case "Time":
                    time = splits[1];
                    break;
                case "Map":
                    int mapPos = splits[1]=="Forest" ? 0 : splits[1]=="Cavern" ? 1 : 2;
                    for (int i=0; i<3; i++) {
                        if (i==mapPos) maps[i].SetActive(true);
                        else Destroy(maps[i]);
                    }
                    break;
            }
        }
        tr.Close();

        SetSunRotation();
        ActivateSpawners();
    }

    private void SetSunRotation() {
        switch(time) {
            case "Noon":
                sun.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case "Dusk":
                sun.transform.rotation = Quaternion.Euler(0, 0, 0);
                foreach (GameObject skyContainer in skyContainers)
                    if (skyContainer != null)
                    foreach(SpriteRenderer rend in skyContainer.GetComponentsInChildren<SpriteRenderer>()) {
                        rend.color = new Color(112/256f, 112/256f, 112/256f);
                    }
                break;
            case "Night":
                sun.gameObject.SetActive(false);
                foreach (GameObject skyContainer in skyContainers)
                    if (skyContainer != null)
                    foreach(SpriteRenderer rend in skyContainer.GetComponentsInChildren<SpriteRenderer>()) {
                        rend.color = new Color(42/256f, 33/256f, 58/256f);
                    }
                break;
        }
    }

    private void ActivateSpawners() {
        foreach (Spawner s in FindObjectsOfType<Spawner>()) {
            if (s.isActive)
                s.Spawn(diff);
        }
    }

    public void Death() {
        if (secondLife) {
            secondLife = false;
            FindObjectOfType<CombatController>().Respawn();
            lifeImg.SetActive(false);
            return;
        }
        TextReader trInv = new StreamReader(inventoryPath); //read inventory
        List<string> inventory = new List<string>();
        string line;
        while ((line = trInv.ReadLine()) != null && line != "") {
            inventory.Add(line);
        }
        trInv.Close();

        TextReader trEq = new StreamReader(equipmentPath); //remove equipment from inventory
        string[] lines = trEq.ReadLine().Split(";");
        foreach(string l in lines) {
            string[] splits = l.Split(':');
            if (splits[1]!="") {
                inventory.Remove(splits[1]);
            }
        }
        trEq.Close();
        
        TextWriter twEq = new StreamWriter(equipmentPath); //write empty equipment
        twEq.WriteLine("Helmet:;Chestplate:;Boots:;Weapon:;Talisman:;Ring:");
        twEq.Close();
        
        TextWriter twInv = new StreamWriter(inventoryPath); //write new inventory
        foreach(string s in inventory) {
            twInv.WriteLine(s);
        }
        twInv.Close();
        
        TextWriter twRes = new StreamWriter(resultPath);
        twRes.Write("Death");
        twRes.Close();
        
        Time.timeScale = 1;
        SceneManager.LoadScene("Office");
    }

    public void Win() {
        TextWriter twRes = new StreamWriter(resultPath);
        twRes.Write("Win");
        twRes.Close();

        Time.timeScale = 1;
        SceneManager.LoadScene("Office");
    }

    public void BackToMenu() {
        TextWriter twRes = new StreamWriter(resultPath);
        twRes.Write("Draw");
        twRes.Close();

        Time.timeScale = 1;
        SceneManager.LoadScene("Office");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetPause(!paused);
        }
    }

    public void SetPause(bool p) {
        paused = p;
        Time.timeScale = p ? 0 : currentTimeScale;
        pausePanel.SetActive(p);
        
        Cursor.lockState = p ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = p;
    }

    public void SlowTime() {
        if (currentTimeScale > 0.9f && ring == 2) {
            Time.timeScale = 0.5f;
            currentTimeScale = 0.5f;
            StartCoroutine(RestoreTime());
        }
    }

    public void StunEnemy(Vector3 pos) {
        if (ring == 1) {
            float r = 3*3;
            foreach (Enemy e in FindObjectsOfType<Enemy>()) {
                if ((e.transform.position - pos).sqrMagnitude < r) {
                    e.Stun();
                }
            }
        }
    }

    private IEnumerator RestoreTime() {
        yield return new WaitForSeconds(3);
        Time.timeScale = 1;
        currentTimeScale = 1;
    }
}

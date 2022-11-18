using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MissionManager : MonoBehaviour
{
    public Light sun;
    public GameObject skyContainer;
    public int diff {get; private set;}
    public string time {get; private set;}
    public int def {get; private set;} = 0;
    private string inventoryPath;
    private string equipmentPath;
    private string resultPath;

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
                foreach(SpriteRenderer rend in skyContainer.GetComponentsInChildren<SpriteRenderer>()) {
                    rend.color = new Color(112/256f, 112/256f, 112/256f);
                }
                break;
            case "Night":
                sun.gameObject.SetActive(false);
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
        
        SceneManager.LoadScene("Office");
    }

    public void Win() {
        TextWriter twRes = new StreamWriter(resultPath);
        twRes.Write("Win");
        twRes.Close();

        SceneManager.LoadScene("Office");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TextWriter twRes = new StreamWriter(resultPath);
            twRes.Write("Draw");
            twRes.Close();

            SceneManager.LoadScene("Office");
        }
    }
}

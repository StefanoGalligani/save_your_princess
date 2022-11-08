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
    public int time {get; private set;}

    void Start() {
        string temp = Application.persistentDataPath;
        string sPath = Path.Combine(temp, "missionInfo.txt");

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
        string[] lines = File.ReadAllLines(sPath);
        
        diff = int.Parse(lines[0].Split(':')[1]);
        time = int.Parse(lines[1].Split(':')[1]);
            
        SetSunRotation();
        ActivateSpawners();
    }

    private void SetSunRotation() {
        switch(time) {
            case 1:
                sun.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case 2:
                sun.transform.rotation = Quaternion.Euler(0, 0, 0);
                foreach(SpriteRenderer rend in skyContainer.GetComponentsInChildren<SpriteRenderer>()) {
                    rend.color = new Color(112/256f, 112/256f, 112/256f);
                }
                break;
            case 3:
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

}

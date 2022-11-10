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
        TextReader tw = new StreamReader(sPath);
        string[] lines = tw.ReadLine().Split(";");

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Office");
        }
    }
}

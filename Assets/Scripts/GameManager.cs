using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class GameManager : MonoBehaviour
{
    void Start() {
        string temp = Application.persistentDataPath;
        string sPath = Path.Combine(temp, "missionInfo.txt");
        
        FileStream str = File.Create(sPath);
        Debug.Log("Created");

        string lines = "Diff:1\nTime:1";

        byte[] info = new UTF8Encoding(true).GetBytes(lines);
        str.Write(info);
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            string current = SceneManager.GetActiveScene().name;
            if (current == "Office") {
                SceneManager.LoadScene("Mission");
            } else {
                SceneManager.LoadScene("Office");
            }
        }
    }
}

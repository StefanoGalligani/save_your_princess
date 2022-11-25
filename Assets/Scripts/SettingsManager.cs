using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider mouseSlider;
    public Toggle fullscreenToggle;
    string settingsPath;
    float musicVolume = 1;
    float sfxVolume = 1;
    float mouseSensitivity = 20;
    bool fullscreen = true;

    void Start() {
        string temp = Application.persistentDataPath;
        settingsPath = Path.Combine(temp, "settings.txt");
        ReadSettings();
        AdjustSettingsUI();
        ApplySettings();
    }

    void ReadSettings() {
        if (!File.Exists(settingsPath)) {
            File.Create(settingsPath).Close();

            TextWriter tw = new StreamWriter(settingsPath);
            tw.WriteLine("Music:1;Sfx:1;Mouse:20;Fullscreen:1");
            tw.Close();
        } else {
            TextReader tr = new StreamReader(settingsPath);
            string[] lines = tr.ReadLine().Split(";");

            foreach(string l in lines) {
                string[] splits = l.Split(':');
                switch (splits[0]) {
                    case "Music":
                        musicVolume = float.Parse(splits[1]);
                    break;
                    case "Sfx":
                        sfxVolume = float.Parse(splits[1]);
                    break;
                    case "Mouse":
                        mouseSensitivity = float.Parse(splits[1]);
                    break;
                    case "Fullscreen":
                        fullscreen = int.Parse(splits[1])==1;
                    break;
                }
            }
            tr.Close();
        }
    }

    void AdjustSettingsUI() {
        musicSlider.value = musicVolume*100;
        sfxSlider.value = sfxVolume*100;
        mouseSlider.value = mouseSensitivity;
        fullscreenToggle.isOn = fullscreen;
    }

    void WriteSettings() {
        File.Create(settingsPath).Close();

        TextWriter tw = new StreamWriter(settingsPath);
        tw.WriteLine("Music:" + musicVolume + ";Sfx:" + sfxVolume + ";Mouse:" + mouseSensitivity + ";Fullscreen:" + (fullscreen?1:0));
        tw.Close();
    }

    void ApplySettings() {
        Screen.fullScreen = fullscreen;
        if (FindObjectOfType<MyCharacterController>())
            FindObjectOfType<MyCharacterController>().rotSpeed = mouseSensitivity;
        foreach(AudioSource aud in FindObjectsOfType<AudioSource>()) {
            if (aud.gameObject.layer == LayerMask.NameToLayer("Soundtrack"))
                aud.volume = musicVolume;
            else
                aud.volume =  sfxVolume;
        }
    }

    public void SetMusicVolume() {
        musicVolume = musicSlider.value/100f;
        WriteSettings();
        ApplySettings();
    }

    public void SetSFXVolume() {
        sfxVolume = sfxSlider.value/100f;
        WriteSettings();
        ApplySettings();
    }

    public void SetMouseSensitivity() {
        mouseSensitivity = mouseSlider.value;
        WriteSettings();
        ApplySettings();
    }

    public void SetFullscreen() {
        fullscreen = fullscreenToggle.isOn;
        WriteSettings();
        ApplySettings();
    }

}

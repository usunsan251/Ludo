using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Toggle redCpu, redHuman;
    public Toggle greenCpu, greenHuman;
    public Toggle blueCpu, blueHuman;
    public Toggle yellowCpu, yellowHuman;

    void ReadToggle()
    {
        //RED 0
        if (redCpu.isOn)
        {
            SaveSettings.players[0] = "CPU";
        }
        else if (redHuman.isOn)
        {
            SaveSettings.players[0] = "HUMAN";
        }

        //GREEN 1
        if (greenCpu.isOn)
        {
            SaveSettings.players[1] = "CPU";
        }
        else if (greenHuman.isOn)
        {
            SaveSettings.players[1] = "HUMAN";
        }

        //BLUE 2
        if (blueCpu.isOn)
        {
            SaveSettings.players[2] = "CPU";
        }
        else if (blueHuman.isOn)
        {
            SaveSettings.players[2] = "HUMAN";
        }

        //YE++OW 3
        if (yellowCpu.isOn)
        {
            SaveSettings.players[3] = "CPU";
        }
        else if (yellowHuman.isOn)
        {
            SaveSettings.players[3] = "HUMAN";
        }
    }
    //THIS FUNCTION HAS TO CALLED FROM START BUTTON
    public void StartGame(string sceneName)
    {
        ReadToggle();
        SceneManager.LoadScene(sceneName);
    }
    /*
    //---------------------RED--------------------
    public void SetRedHumanType(bool on)
    {
        if (on) SaveSettings.players[0] = "HUMAN";
    }
    public void SetRedCpuType(bool on)
    {
        if (on) SaveSettings.players[0] = "CPU";
    }

    //---------------------GREEN--------------------
    public void SetGreenHumanType(bool on)
    {
        if (on) SaveSettings.players[1] = "HUMAN";
    }
    public void SetGreenCpuType(bool on)
    {
        if (on) SaveSettings.players[1] = "CPU";
    }

    //---------------------BLUE--------------------
    public void SetBlueHumanType(bool on)
    {
        if (on) SaveSettings.players[2] = "HUMAN";
    }
    public void SetBlueCpuType(bool on)
    {
        if (on) SaveSettings.players[2] = "CPU";
    }

    //---------------------YELLOW--------------------
    public void SetYellowHumanType(bool on)
    {
        if (on) SaveSettings.players[3] = "HUMAN";
    }
    public void SetYellowCpuType(bool on)
    {
        if (on) SaveSettings.players[3] = "CPU";
    }
}
    */
}
public static class SaveSettings
{
    // 0 1 2 3
    //RED GRREN BLUE YELLOW
    public static string[] players = new string[4];

    public static string[] winners = new string[3] {string .Empty, string.Empty, string.Empty};
}

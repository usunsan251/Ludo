using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < SaveSettings.players.Length; i++)
        {
            SaveSettings.players[i] = "CPU";

        }
    }
    
    
    public void StartTheGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    public static Info Instance;

    public Text infoText;

    void Awake()
    {
        Instance = this;
        infoText.text = "";
    }   
    
    public void ShowMessage(string _text)
    {
        infoText.text = _text;
    } 

}

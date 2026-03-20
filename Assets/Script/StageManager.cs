using System.Net.WebSockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public enum STAGE { FIELD, FOREST, GRAVE, OCEAN, LAVA }
    public STAGE stage;

    // UI
    public TextMeshProUGUI stageTxt;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
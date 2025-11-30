using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum UI_HandType
{
    Off,
    Default,
    Both,
    TODO
}


public class ItemHUD_UI : MonoBehaviour
{

    [Header("UI References Def")]
    public GameObject UI_Default;
    
    public GameObject Panel_Def_L;
    public TextMeshProUGUI Text_ID_L;
    public TextMeshProUGUI Text_Hint_L;
    public TextMeshProUGUI Text_Hint_L_Use;

    public GameObject Panel_Def_R;
    public TextMeshProUGUI Text_ID_R;
    public TextMeshProUGUI Text_Hint_R;
    public TextMeshProUGUI Text_Hint_R_Use;

    [Header("UI References Both")]
    public GameObject UI_Both;

    public GameObject Panel_Both;
    public TextMeshProUGUI Text_ID_Both;
    public TextMeshProUGUI Text_Hint_Both;
    //TO DO

    [Header("Extras")]
    public UI_HandType handTypeUI = UI_HandType.Default;


    void Start()
    {
        if (UI_Default != null)
        {
            UI_Default.SetActive(true);
            Text_Hint_L_Use.text = "";
            Text_Hint_R_Use.text = "";
        }

        if (UI_Both != null)
        {
            UI_Both.SetActive(false);
        }

        //Other UI
    }

    void Update()
    {
        
    }

    public void setHandType(UI_HandType handType)
    {
        handTypeUI = handType;
        switch (handTypeUI)
        { 
            case UI_HandType.Off:
                UI_Default.SetActive(false);
                UI_Both.SetActive(false);
                break;

            case UI_HandType.Default:
                UI_Default.SetActive(true);
                UI_Both.SetActive(false);
                break;

            case UI_HandType.Both:
                UI_Default.SetActive(false);
                UI_Both.SetActive(true);
                break;
        }
    }

    public void setPanelText(HandType type, string ID, string key, string key_use, ItemSize size)
    {
        switch (type)
        {
            case HandType.Left:
                Text_ID_L.text = $"{ID}";
                Text_Hint_L.text = $"[{key}]" + "to Drop Down";
                if(size == ItemSize.Tool)
                {
                    Text_Hint_L_Use.text = $"Tool:[{key_use}]" + "to Use";
                }
                break;
            case HandType.Right:
                Text_ID_R.text = $"{ID}";
                Text_Hint_R.text = $"[{key}]" + "to Drop Down";
                if (size == ItemSize.Tool)
                {
                    Text_Hint_R_Use.text = $"Tool:[{key_use}]" + "to Use";
                }
                break;
            case HandType.Both:
                Text_ID_Both.text = $"{ID}";
                Text_Hint_Both.text = $"[{key}]" + "to Drop Down";
                break;
        }
    }

    public void resetPanelText(HandType type, string key)
    {
        switch (type)
        {
            case HandType.Left:
                Text_ID_L.text = "EMPTY";
                Text_Hint_L.text = $"[{key}]" + "to Pick Up";
                Text_Hint_L_Use.text = "";
                break;
            case HandType.Right:
                Text_ID_R.text = "EMPTY";
                Text_Hint_R.text = $"[{key}]" + "to Pick Up";
                Text_Hint_R_Use.text = "";
                break;
            case HandType.Both:
                Text_ID_Both.text = "EMPTY";
                Text_Hint_Both.text = $"[{key}]" + "to Pick Up";
                break;
        }
    }
}

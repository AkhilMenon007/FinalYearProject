using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Debugger : MonoBehaviour
{
    public static Debugger instance;
    
    static Text text;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(this);
            return;
        }
        text = GetComponent<Text>();
    }

    public static void Log(string text) 
    {
        Debugger.text.text = text +"\n"+ Debugger.text.text;
    }

}

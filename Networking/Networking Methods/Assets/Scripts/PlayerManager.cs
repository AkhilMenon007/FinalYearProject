using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public static System.Action OnClick;
    public static System.Action OnClickNoRPC;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }

    }
    public void Click() 
    {
        OnClick?.Invoke();
    }
    public void NoRPCClick() 
    {
        OnClickNoRPC?.Invoke();
    }
}

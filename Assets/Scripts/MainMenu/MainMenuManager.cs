using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : utility.Singleton<MainMenuManager>
{
    private MainMenuFence _fence;


    private void Start()
    {
        _fence = MainMenuFence.Instance; 
    }

    public void ButtonDown_OpenCounter()
    {
        _fence.PlayLiftAnimation();
    }
}

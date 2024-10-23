using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHomeView : MainViewChild
{
    [SerializeField] private ScrollRect _scrollRect;

    public override void OnShow()
    {
        for (int i = _scrollRect.content.childCount - 1; i >= 0; i--)
        {
            Destroy(_scrollRect.content.GetChild(i));
        }

        var personConfigTable = ConfigLoader.Load<PersonConfigTable>();
        
    }

    public override void OnHide()
    {
        
    }
}

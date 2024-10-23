using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    [SerializeField] private List<MainViewChild> _childViews = new List<MainViewChild>();

    [SerializeField] private List<Button> childViewButtons = new List<Button>();

    private int _childIndex = -1;
    
    private void Awake()
    {
        ChangeToChild(1);
        for (int i = 0; i < _childViews.Count; i++)
        {
            int index = i;
            childViewButtons[index].onClick.AddListener(() =>
            {
                ChangeToChild(index);
            });
        }
    }

    private void ChangeToChild(int index)
    {
        if (_childIndex == index)
        {
            return;
        }
        if (index >= _childViews.Count)
        {
            return;
        }
        if (_childIndex >= 0 && _childIndex<_childViews.Count)
        {
            _childViews[_childIndex].OnHide();
        }

        _childIndex = index;
        for (int i = 0; i < _childViews.Count; i++)
        {
            _childViews[i].gameObject.SetActive(i == _childIndex);
        }
        for (int i = 0; i < childViewButtons.Count; i++)
        {
            childViewButtons[i].interactable = i != _childIndex;
        }
        _childViews[_childIndex].OnShow();
    }

    public static void Open()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/View/MainView"),Main.Instance.canvas.transform);
    }
}

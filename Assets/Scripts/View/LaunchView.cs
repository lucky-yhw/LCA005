using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LaunchView : MonoBehaviour
{
    [SerializeField] private Image _progressBar;

    private void Start()
    {
        _progressBar.fillAmount = 0;
        MainView.Open();
        _progressBar.DOFillAmount(1, 5).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}

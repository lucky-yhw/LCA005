using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LaunchView : MonoBehaviour
{
    [SerializeField] private Image _progressBar;

    private void Awake()
    {
        _progressBar.fillAmount = 0;
        //进度条走完打开主界面，关闭这个界面
        _progressBar.DOFillAmount(1, 5).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}

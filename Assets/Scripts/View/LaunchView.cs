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
        _progressBar.DOFillAmount(0.9f, 3f).SetEase(Ease.Linear);
        Sequence seq = DOTween.Sequence();
        seq.InsertCallback(1f, () =>
        {
            ServerData.Instance.Login(() =>
            {
                DOTween.Kill(_progressBar);
                _progressBar.DOFillAmount(1f, 0.5f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            });
        });
    }
}
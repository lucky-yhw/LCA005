using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Canvas _canvasPop;
    [SerializeField] private Canvas _canvasTop;
    
    public Canvas canvas => _canvas;
    public Canvas canvasPop => _canvasPop;
    public Canvas canvasTop => _canvasTop;
    
    public static Main Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}

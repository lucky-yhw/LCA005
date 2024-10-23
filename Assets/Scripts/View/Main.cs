using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    public Canvas canvas => _canvas;
    
    public static Main Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}

﻿using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    internal static InputManager Instance { get; private set; }

    internal event EventHandler OnActionButtonDown;

    internal Vector3 MouseWorldPosition { get; private set; }
    internal float HorizontalAxisValue { get; private set; }
    internal float VerticalAxisValue { get; private set; }

    [Header("Input axes")]
    [SerializeField] private string _actionButtonName = "Fire1";
    [SerializeField] private string _horizontalAxisName = "Horizontal";
    [SerializeField] private string _verticalAxisName = "Vertical";

    private Camera _mainCamera;

    private void Awake()
    {
        SingletonSetup();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        // _mainCamera.ScreenToWorldPoint did not work for some reason
        MouseWorldPosition = _mainCamera.ScreenPointToRay(Input.mousePosition).direction * 10;

        HorizontalAxisValue = Input.GetAxisRaw(_horizontalAxisName);
        VerticalAxisValue = Input.GetAxisRaw(_verticalAxisName);

        if (Input.GetButtonDown(_actionButtonName))
        {
            OnActionButtonDown?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SingletonSetup()
    {
        if (Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

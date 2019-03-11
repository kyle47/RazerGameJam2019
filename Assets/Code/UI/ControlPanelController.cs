using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelController : MonoBehaviour
{
    public static ControlPanelController Instance;

    public RectTransform Joystick;

    public float InputVertical;
    public float InputHorizontal;

    private void Awake()
    {
        if(Instance)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnJoystickPositionChanged(Vector2 newPosition)
    {
        InputHorizontal = Mathf.Round(Joystick.anchoredPosition.x);
        InputVertical = Mathf.Round(Joystick.anchoredPosition.y);
    }

}

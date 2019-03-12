using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    protected const float SYNC_INTERVAL = 1.0f / 45;

    [HideInInspector]
    public float InputVertical;
    [HideInInspector]
    public float InputHorizontal;

    [HideInInspector]
    public string Name;
    [HideInInspector]
    public PlayerRole Role;
    [HideInInspector]
    public Color Color;

    [HideInInspector]
    public TankComponent Tank;

    protected float _nextInputSyncTime;

    private void Start()
    {
        if(hasAuthority)
        {
            Cmd_SetPlayerInfo(
                GameManager.Instance.PlayerName,
                GameManager.Instance.PlayerRole,
                GameManager.Instance.PlayerColor
            );
        }
    }

    private void Update()
    {
        if (hasAuthority)
        {
            InputVertical = ControlPanelController.Instance.InputVertical;
            InputHorizontal = ControlPanelController.Instance.InputHorizontal;

            if (Time.time > _nextInputSyncTime)
            {
                Cmd_UpdateInput(InputVertical, InputHorizontal);
                _nextInputSyncTime = Time.time + SYNC_INTERVAL;
            }
        }

        if (isServer)
        {
            if(Role == PlayerRole.DRIVER)
            {
                Tank.TreadInputVertical = InputVertical;
                Tank.TreadInputHorizontal = InputHorizontal;
            }
            else if(Role == PlayerRole.GUNNER)
            {
                Tank.GunInputHorizontal = -InputHorizontal;
            }
        }
    }

    [Command]
    protected void Cmd_UpdateInput(float vertical, float horizontal)
    {
        InputVertical = vertical;
        InputHorizontal = horizontal;
    }

    [Command]
    protected void Cmd_SetPlayerInfo(string name, PlayerRole role, Color color)
    {
        Name = name;
        Role = role;
        Color = color;

        if(isServer)
        {
            ServerGameManager.Instance.OnPlayerJoined(this);
        }
    }
}

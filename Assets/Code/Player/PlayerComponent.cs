using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class PlayerComponent : NetworkBehaviour
{
    protected const float SYNC_INTERVAL = 1.0f / 30;

    public float InputVertical;
    public float InputHorizontal;

    public PlayerType PlayerType;

    public GameObject TankPrefab;

    protected TankComponent _tank;

    protected float _nextInputSyncTime;

    private void Start()
    {
        if(isServer)
        {
            var tankObject = NetworkManager.Instantiate(TankPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(tankObject);
            var tank = tankObject.GetComponent<TankComponent>();
            _tank = tank;
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
            _tank.InputVertical = InputVertical;
            _tank.InputHorizontal = InputHorizontal;
        }
    }

    [Command]
    protected void Cmd_UpdateInput(float vertical, float horizontal)
    {
        InputVertical = vertical;
        InputHorizontal = horizontal;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server started");
    }
}

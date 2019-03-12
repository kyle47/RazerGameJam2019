using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Mirror;

public class ServerGameManager : NetworkBehaviour
{
    public static ServerGameManager Instance;

    public GameObject TankPrefab;

    protected List<TankComponent> _tanks = new List<TankComponent>();
    protected List<PlayerComponent> _players = new List<PlayerComponent>();

    private void Awake()
    {
        if (Instance)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnPlayerJoined(PlayerComponent player)
    {
        _players.Add(player);
        
        if(player.Role == PlayerRole.DRIVER)
        {
            var tank = _tanks.FirstOrDefault(searchTank => searchTank.Driver == null);
            if(tank == null)
            {
                tank = SpawnTank();
            }

            player.Tank = tank;
            tank.Driver = player;
        }
        else if(player.Role == PlayerRole.GUNNER)
        {
            var tank = _tanks.FirstOrDefault(searchTank => searchTank.Gunner == null);
            if(tank == null)
            {
                tank = SpawnTank();
            }

            player.Tank = tank;
            tank.Gunner = player;
        }

        _tanks.ForEach(tank =>
        {
            if (tank.Gunner)
            {
                tank.Rpc_SetGunColor(tank.Gunner.Color);
            }

            if (tank.Driver)
            {
                tank.Rpc_SetTreadColor(tank.Driver.Color);
            }
        });
    }

    protected TankComponent SpawnTank()
    {
        var tankObject = NetworkManager.Instantiate(TankPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(tankObject);
        var tank = tankObject.GetComponent<TankComponent>();
        _tanks.Add(tank);
        return tank;
    }
}

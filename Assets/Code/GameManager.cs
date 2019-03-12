using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string PlayerName;
    public PlayerRole PlayerRole;
    public Color PlayerColor;

    private void Awake()
    {
        if(Instance)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Instance = this;
    }

}

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIColorController : MonoBehaviour
{
    public static UIColorController Instance;

    public Image[] DynamicImages;

    private void Awake()
    {
        if(Instance)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetColor(Color color)
    {
        DynamicImages.ToList().ForEach(image => image.color = color);
    }
}
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public InputField Name;
    public Dropdown Role;

    public GameObject MainMenu;

    public RectTransform ColorButtonHolder;

    protected Color[] _baseColors = { Color.red, Color.blue, Color.yellow, Color.green, Color.magenta, Color.gray };

    protected Color[] _colors;

    protected int _selectedColorIndex;

    private void Start()
    {
        OnRoleChanged(0);
    }

    public void Button_Start()
    {
        GameManager.Instance.PlayerName = Name.text;
        GameManager.Instance.PlayerRole = (PlayerRole)Role.value;
        GameManager.Instance.PlayerColor = _colors[_selectedColorIndex];

        MainMenu.SetActive(false);
    }

    public void ReCreateColorButtons()
    {
        ClearColorButtons();
        var template = ColorButtonHolder.transform.GetChild(0).gameObject;
        var colorIndex = 0;
        _colors.ToList().ForEach(color =>
        {
            var buttonObject = GameObject.Instantiate(template, ColorButtonHolder.transform);
            buttonObject.SetActive(true);
            buttonObject.transform.localScale = Vector3.one;

            var localColorIndex = colorIndex;
            var button = buttonObject.GetComponent<Button>();
            button.onClick.AddListener(() => {
                GameManager.Instance.PlayerColor = color;
                _selectedColorIndex = localColorIndex;
                UIColorController.Instance.SetColor(_colors[_selectedColorIndex]);
            });

            var image = buttonObject.GetComponent<Image>();
            image.color = color;
            colorIndex++;
        });
    }

    protected void ClearColorButtons()
    {
        foreach(Transform child in ColorButtonHolder.transform)
        {
            if(child.gameObject.activeInHierarchy)
            {
                var button = child.gameObject.GetComponent<Button>();
                if(button)
                {
                    button.onClick.RemoveAllListeners();
                }

                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public void OnRoleChanged(int value)
    {
        var roleId = Role.value;
        _colors = _baseColors
            .ToList()
            .Select(color =>
                {
                    var targetColor = roleId == (int)PlayerRole.DRIVER ? Color.white: Color.black;
                    return Color.Lerp(color, targetColor, .2f);
                }
            ).ToArray();
        ReCreateColorButtons();
        UIColorController.Instance.SetColor(_colors[_selectedColorIndex]);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SingletonDontDestroyMono<UIController>
{
    [SerializeField] private Button _placeRoadButton, _placeHouseButton, _placeSpeacialButton;

    [SerializeField] private Color _outlineColor;

    private List<Button> _buttonList;

    private void Start()
    {
        _buttonList = new List<Button> { _placeRoadButton, _placeHouseButton, _placeSpeacialButton };

        _placeRoadButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(_placeRoadButton);
            EventManager.Instance.TriggerEvent(GameEventType.OnRoadPlacement);
        });

        _placeHouseButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(_placeHouseButton);
            EventManager.Instance.TriggerEvent(GameEventType.OnHousePlacement);
        });

        _placeSpeacialButton.onClick.AddListener(() =>
        {
            ResetButtonColor();
            ModifyOutline(_placeSpeacialButton);
            EventManager.Instance.TriggerEvent(GameEventType.OnSpecialPlacement);
        });


    }

    private void ModifyOutline(Button button)
    {
        var outline = button.GetComponent<Outline>();
        outline.effectColor = _outlineColor;
        outline.enabled = true;
    }

    private void ResetButtonColor()
    {
        foreach (Button button in _buttonList)
        {
            button.GetComponent<Outline>().enabled = false;
        }
    }
}

using SVS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraMovement _cameraMovement;

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameEventType.OnRoadPlacement, RoadPlacementHandler);
        EventManager.Instance.Subscribe(GameEventType.OnHousePlacement, HousePlacementHandler);
        EventManager.Instance.Subscribe(GameEventType.OnSpecialPlacement, SpecialPlacementHandler);

    }


    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameEventType.OnRoadPlacement, RoadPlacementHandler);
        EventManager.Instance.Unsubscribe(GameEventType.OnHousePlacement, HousePlacementHandler);
        EventManager.Instance.Unsubscribe(GameEventType.OnSpecialPlacement, SpecialPlacementHandler);
    }

    private void HousePlacementHandler(object parameter)
    {
        ClearInputActions();

        EventManager.Instance.Subscribe(GameEventType.OnMouseClick, PlaceHouse);
    }


    private void SpecialPlacementHandler(object parameter)
    {
        ClearInputActions();

        EventManager.Instance.Subscribe(GameEventType.OnMouseClick, PlaceSpecial);
    }


    private void PlaceSpecial(object parameter)
    {
        if(parameter is Vector3Int position)
        {
            StructureManager.Instance.PlaceSpecial(position);
        }
    }

    private void PlaceHouse(object parameter)
    {
        if (parameter is Vector3Int position)
        {
            StructureManager.Instance.PlaceHouse(position);
        }
    }



    private void RoadPlacementHandler(object parameter)
    {
        ClearInputActions();

        EventManager.Instance.Subscribe(GameEventType.OnMouseClick, HandleMouseClick);
        // EventManager.Instance.Subscribe(GameEventType.OnMouseHold, HandleMouseClick);
        EventManager.Instance.Subscribe(GameEventType.OnMouseUp, HandleMouseUp);
    }

    private void ClearInputActions()
    {
        EventManager.Instance.Unsubscribe(GameEventType.OnMouseClick, HandleMouseClick);
        EventManager.Instance.Unsubscribe(GameEventType.OnMouseClick, PlaceHouse);
        EventManager.Instance.Unsubscribe(GameEventType.OnMouseClick, PlaceSpecial);
        // EventManager.Instance.Unsubscribe(GameEventType.OnMouseHold, HandleMouseClick);
        EventManager.Instance.Unsubscribe(GameEventType.OnMouseUp, HandleMouseUp);
    }

    private void HandleMouseUp(object parameter)
    {
        RoadManager.Instance.FinishPlacingRoad();
    }


    private void HandleMouseClick(object parameter)
    {
        if(parameter is Vector3Int position)
        {
            RoadManager.Instance.PlaceRoad(position);
        }
    }

    private void Update()
    {
        _cameraMovement.MoveCamera(new Vector3(InputManager.Instance.CameraMovementVector.x, 0, InputManager.Instance.CameraMovementVector.y));
    }
}

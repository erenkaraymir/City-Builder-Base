using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : SingletonDontDestroyMono<InputManager>
{
    //public Action<Vector3Int> OnMouseClick, OnMouseHold;
    //public Action OnMouseUp;

    private Vector2 _cameraMovementVector;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private LayerMask _groundLayerMask;

    public Vector2 CameraMovementVector
    {
        get { return _cameraMovementVector; }
    }

    private void Update()
    {
        CheckClickDownEvent();
        CheckClickUpEvent();
        CheckClickHoldEvent();
        CheckArrowInput();
    }

    private Vector3Int? RaycastGround()
    {
        RaycastHit hit;
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayerMask))
        {
            Vector3Int positionInput = Vector3Int.RoundToInt(hit.point);
            return positionInput;
        }
        return null;
    }

    private void CheckArrowInput()
    {
        _cameraMovementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void CheckClickHoldEvent()
    {
        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if(position != null)
            {
                EventManager.Instance.TriggerEvent(GameEventType.OnMouseHold, position.Value);
            }
        }
    }

    private void CheckClickUpEvent()
    {
        if (Input.GetMouseButtonUp(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if (position != null)
            {
                EventManager.Instance.TriggerEvent(GameEventType.OnMouseUp);
            }
        }
    }

    private void CheckClickDownEvent()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            var position = RaycastGround();
            if (position != null)
            {
                EventManager.Instance.TriggerEvent(GameEventType.OnMouseClick, position.Value);
            }
        }
    }

}

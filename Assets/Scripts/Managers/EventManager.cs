using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameEventType
{
   OnMouseClick,OnMouseHold,OnMouseUp,
   OnRoadPlacement,OnHousePlacement,OnSpecialPlacement
}


[DefaultExecutionOrder(-1)]
public class EventManager : SingletonDontDestroyMono<EventManager>
{
    private Dictionary<GameEventType, Action<object>> eventListeners = new Dictionary<GameEventType, Action<object>>();


    public void Subscribe(GameEventType eventType, Action<object> listener)
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = listener;
        }
        else
        {
            eventListeners[eventType] += listener;
        }
    }

    public void Unsubscribe(GameEventType eventType, Action<object> listener)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] -= listener;
        }
    }

    public void TriggerEvent(GameEventType eventType, object parameter = null)
    {
        if (eventListeners.TryGetValue(eventType, out Action<object> listener))
        {
            listener?.Invoke(parameter);
        }
    }


}

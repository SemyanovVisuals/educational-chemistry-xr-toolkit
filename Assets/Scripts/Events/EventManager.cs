using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public enum EventType
{
    LeftWristDownActivated,
    LeftWristDownDeactivated,
    PinchSelectorSelected,
    PinchSelectorUnselected,
    RightPalmUpActivated,
    RightPalmUpDeactivated,
    SwipeForwardActivated,
    SwipeForwardDeactivated
}

public class EventManager : MonoBehaviour
{
    private static EventManager _eventManager;

    private Dictionary<EventType, Action<System.Object>> _listeners;
    
    private static EventManager Instance
    {
        get
        {
            if (!_eventManager)
            {
                _eventManager = FindFirstObjectByType(typeof(EventManager)) as EventManager;
                if (!_eventManager)
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                else
                    _eventManager.Init();
            }
            return _eventManager;
        }
    }

    void Init()
    {
        if (_listeners == null)
        {
            _listeners = new Dictionary<EventType, Action<System.Object>>();
        }
    }

    public static void StartListening(EventType eventType, Action<System.Object> listener)
    {
        if (!Instance._listeners.TryAdd(eventType, listener))
        {
            Instance._listeners[eventType] += listener;
        }
    }

    public static void StopListening(EventType eventType, Action<System.Object> listener)
    {
        if (Instance._listeners.ContainsKey(eventType))
        {
            Instance._listeners[eventType] -= listener;
        }
    }
    
    public static void TriggerEvent(EventType eventType, System.Object eventParam = null)
    {
        if (Instance._listeners.TryGetValue(eventType, out Action<System.Object> action))
        {
            action.Invoke(eventParam);
        }
    }
}
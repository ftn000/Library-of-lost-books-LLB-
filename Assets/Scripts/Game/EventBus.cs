using UnityEngine;
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Delegate>();
        _subscribers[type].Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);
        if (_subscribers.ContainsKey(type))
            _subscribers[type].Remove(callback);
    }

    public static void Raise<T>(T evt)
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var listeners))
        {
            Debug.Log($"[EventBus] Raising event {type.Name}, listeners count={listeners.Count}");
            foreach (var listener in listeners)
                (listener as Action<T>)?.Invoke(evt);
        }
        else
        {
            Debug.Log($"[EventBus] Raising event {type.Name}, no listeners");
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    // Define events
    public event Action OnLevelPass;
    public event Action OnLevelFail;

    private void Awake()
    {
        // Ensure only one instance of EventManager exists (Singleton pattern)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Methods to invoke events
    public void LevelPass()
    {
        OnLevelPass?.Invoke();
    }

    public void LevelFail()
    {
        OnLevelFail?.Invoke();
    }
}

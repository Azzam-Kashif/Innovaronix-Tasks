using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject levelCompletePanel;
    public GameObject levelFailPanel;

    private void Awake()
    {
        levelCompletePanel.SetActive(false);
        levelFailPanel.SetActive(false);
        // Ensure only one instance of UIManager exists (Singleton pattern)
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

    private void OnEnable()
    {
        EventManager.Instance.OnLevelPass += ShowLevelComplete;
        EventManager.Instance.OnLevelFail += ShowLevelFail;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnLevelPass -= ShowLevelComplete;
        EventManager.Instance.OnLevelFail -= ShowLevelFail;
    }

    private void ShowLevelComplete()
    {
        levelCompletePanel.SetActive(true);
        Time.timeScale = 0f;
        // Additional code to handle level completion (e.g., load next level)
    }

    private void ShowLevelFail()
    {
        levelFailPanel.SetActive(true);
        Time.timeScale = 0f;
        // Additional code to handle level failure (e.g., restart level)
    }
    public void RestartLevel()
    {
        if (BallController.Instance != null)
        {
            BallController.Instance.Respawn();
        }
        Time.timeScale = 1f;
        levelCompletePanel.SetActive(false);
        levelFailPanel.SetActive(false);
    }
}

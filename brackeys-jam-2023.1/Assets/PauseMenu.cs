using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public static bool isPaused = false;

    [SerializeField] private string menuSceneName = "MenuScene";
    [SerializeField] private string levelSceneName = "LevelSelectionScene";
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log(isPaused);
            if(!isPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    void Pause()
    {
        Debug.Log("Pause!");
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void LoadLevelSelector()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSceneName);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}

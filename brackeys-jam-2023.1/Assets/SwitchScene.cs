using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToNextScene(string destinationScene)
    {
        SceneManager.LoadScene(destinationScene);
        GameManager.Instance.currentLevel = destinationScene;
        if (LevelSelectionController.Instance != null)
        {
            gameObject.transform.parent.parent.gameObject.SetActive(false);
        }
    }
}

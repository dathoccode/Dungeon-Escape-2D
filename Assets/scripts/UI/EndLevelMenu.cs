using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Button nextLevelButton;

    private void Start()
    {
        
        if (DataClass.hasFinishedLevel1)
        {
            nextLevelButton.interactable = true;
        }
        else
        {
            nextLevelButton.interactable = false;
        }

        if (DataClass.hasFinishedTheLevel)
        {
            winText.text = "You win!";
        }
        else
        {
            winText.text = "You lose!";
        }
        
        if (DataClass.currentLevel == 3)
        {
            nextLevelButton.interactable = false;
        }

    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(1);
    }

    public void NextLevel()
    {
        switch(DataClass.currentLevel)
        {
            case 1:
                SceneManager.LoadScene(3);
                break;
            case 2:
                SceneManager.LoadScene(4);
                break;
        }
    }

    public void RestartLevel()
    {
        switch(DataClass.currentLevel)
        {
            case 1:
                SceneManager.LoadScene(2);
                break;
            case 2:
                SceneManager.LoadScene(3);
                break;
            case 3:
                SceneManager.LoadScene(4);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChoose : MonoBehaviour
{
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;
    private AudioSource audioSource;

    public void Start()
    {
        if (DataClass.hasFinishedLevel1)
        {
            level2Button.interactable = true;
        }
        else
        {
            level2Button.interactable = false;
        }
        if (DataClass.hasFinishedLevel2)
        {
            level3Button.interactable = true;
        }
        else
        {
            level3Button.interactable = false;
        }
        audioSource = GameObject.FindGameObjectsWithTag("Music")[0].GetComponent<AudioSource>();
    }
    public void Level1()
    {
        //check if audio source exists
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Level2()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void Level3()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }


}

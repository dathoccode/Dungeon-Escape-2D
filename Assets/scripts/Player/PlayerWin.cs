using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWin : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("End"))
        {
            DataClass.hasFinishedTheLevel = true;
            // Check which level we are on
            // and set the corresponding DataClass variable to true
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            switch (currentSceneIndex)
            {
                case 2:
                    // level 1
                    DataClass.hasFinishedLevel1 = true;
                    break;
                case 3:
                    DataClass.hasFinishedLevel2 = true;
                    break;
                case 4:
                    DataClass.hasFinishedLevel3 = true;
                    break;
            }
            // Load the ending scene
            SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
        }
    }
}

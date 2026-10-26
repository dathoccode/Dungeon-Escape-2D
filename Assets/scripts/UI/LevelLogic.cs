using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataClass.currentCherries = 0;
        // Check which level we are on
        // Check which level we are on
        // and set the corresponding DataClass variable to true
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        switch (currentSceneIndex)
        {
            case 2:
                // level 1
                DataClass.currentLevel = 1;
                break;
            case 3:
                DataClass.currentLevel = 2;
                break;
            case 4:
                DataClass.currentLevel = 3;
                break;
        }

        DataClass.hasFinishedTheLevel = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

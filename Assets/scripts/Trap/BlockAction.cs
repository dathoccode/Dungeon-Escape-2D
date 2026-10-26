using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAction : MonoBehaviour
{
    [SerializeField] private int minimumCherriesToCollect = 0;
    [SerializeField] private AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        if (DataClass.currentCherries >= minimumCherriesToCollect)
        {
            // Play the sound
            audioSource.Play();
            // Destroy the block
            Destroy(gameObject);
        }
    }
}

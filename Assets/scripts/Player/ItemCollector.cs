using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] TMP_Text TextCherries;
    [SerializeField] private AudioSource audioSource;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            Destroy(collision.gameObject);
            DataClass.currentCherries++;
            // Play the sound
            audioSource.Play();
            TextCherries.text = "Cherries : " + DataClass.currentCherries.ToString();
        }
    }

    private void Start()
    {
        TextCherries.text = "Cherries : " + 0;
    }
}

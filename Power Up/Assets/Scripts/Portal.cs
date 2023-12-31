using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene(targetScene);
    }
}

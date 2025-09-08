using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Reload scene saat ini
    public void RefreshScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Pindah ke scene berikutnya
    public void NextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("Next scene index out of range!");
        }
    }

    // Pindah ke scene sebelumnya
    public void PrevScene()
    {
        int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if (prevIndex >= 0)
        {
            SceneManager.LoadScene(prevIndex);
        }
        else
        {
            Debug.LogWarning("Previous scene index out of range!");
        }
    }

    // Pindah ke scene berdasarkan build index
    public void LoadSceneByIndex(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.LogError("Invalid scene index: " + index);
        }
    }

    public void QuitAplication()
    {
        Application.Quit();
    }

    public void ActiveScene(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void NonActiveScene(GameObject panel)
    {
        panel.SetActive(false);
    }
}

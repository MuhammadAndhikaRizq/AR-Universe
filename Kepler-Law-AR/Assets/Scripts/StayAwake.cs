using UnityEngine;
using UnityEngine.SceneManagement;

public class StayAwake : MonoBehaviour
{
    private static StayAwake instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Jangan dihancurkan saat ganti scene
        }
        else
        {
            Destroy(gameObject); // Kalau ada duplikat, hapus
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Cek nama scene apakah mengandung "Law"
        if (scene.name.Contains("Law"))
        {
            gameObject.SetActive(true);   // Aktifkan ARCamera
        }
        else
        {
            gameObject.SetActive(false);  // Nonaktifkan di luar scene AR
        }
    }

}

using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool isPaused = false;

    public GameObject[] activeObjects,desactiveObjects;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f; // pausa
        isPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        foreach (var item in activeObjects) { item.SetActive(true); }
        foreach (var item in desactiveObjects) { item.SetActive(false); }
    }

    public void Resume()
    {
        Time.timeScale = 1f; // volta ao normal
        isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        foreach (var item in activeObjects) { item.SetActive(false); }
        foreach (var item in desactiveObjects) { item.SetActive(true); }
    }
}

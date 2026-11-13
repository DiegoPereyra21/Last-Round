using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//boton de pausa del HUD, tambien funciona con Escape
public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

    }

    //lo usan el boton de pausa y el de reanudar
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;//en 0 se congela todo lo que usa deltaTime (movimiento, balas, timer)
        pausePanel.SetActive(isPaused);
        audioSource.PlayOneShot(clickSound);//el audio no depende del timeScale, suena igual
    }

    public void GoToMenu()
    {
        StartCoroutine(GoToMenuWithSound());
    }

    IEnumerator GoToMenuWithSound()
    {
        audioSource.PlayOneShot(clickSound);
        //Realtime porque con timeScale en 0 el WaitForSeconds normal nunca termina
        yield return new WaitForSecondsRealtime(clickSound.length);
        Time.timeScale = 1f;//si no lo vuelvo a 1 el menu queda congelado
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartWithSound());
    }

    IEnumerator RestartWithSound()
    {
        audioSource.PlayOneShot(clickSound);
        yield return new WaitForSecondsRealtime(clickSound.length);//Realtime porque en pausa el timeScale esta en 0
        Time.timeScale = 1f;//si no lo vuelvo a 1 el nivel recargado queda congelado
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);//recarga el nivel actual
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void chamaGarimpeiroGameplay()
    {
        SceneManager.LoadScene("GarimpeiroGameplay");
    }

    public void chamaCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void chamaRegras()
    {
        SceneManager.LoadScene("Regras");
    }

    public void chamaMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void fechaJogo()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

}

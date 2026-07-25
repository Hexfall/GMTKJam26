using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static void LoadScene(int id)
    {
        SceneManager.LoadScene(id);
    }
    
    public static void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    
    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public static void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    
    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

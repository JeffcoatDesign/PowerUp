using UnityEngine;
using UnityEngine.SceneManagement;

public class TempMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void LoadPlayground()
    {
        SceneManager.LoadScene("Playground");
    }
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("Level1");
    }
}

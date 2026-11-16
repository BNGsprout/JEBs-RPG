using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject menu;

    public void PlayButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
}

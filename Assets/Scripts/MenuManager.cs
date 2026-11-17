using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject saveSelectMenu;
    [SerializeField] GameObject nameSaveMenu;
    [SerializeField] GameObject playerJoinMenu;

    public void PlayButtonClicked()
    {
        menu.SetActive(false);
        saveSelectMenu.SetActive(true);
    }

    public void Slot1Clicked()
    {
        saveSelectMenu.SetActive(false);
        nameSaveMenu.SetActive(true);
    }

    public void Slot2Clicked()
    {
        saveSelectMenu.SetActive(false);
        nameSaveMenu.SetActive(true);
    }

    public void Slot3Clicked()
    {
        saveSelectMenu.SetActive(false);
        nameSaveMenu.SetActive(true);
    }

    public void Slot4Clicked()
    {
        saveSelectMenu.SetActive(false);
        nameSaveMenu.SetActive(true);
    }

    public void Next1Clicked()
    {
        nameSaveMenu.SetActive(false);
        playerJoinMenu.SetActive(true);
    }

    public void BackButton1Clicked()
    {
        saveSelectMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void BackButton2Clicked()
    {
        nameSaveMenu.SetActive(false);
        saveSelectMenu.SetActive(true);
    }

    public void BackButton3Clicked()
    {
        playerJoinMenu.SetActive(false);
        nameSaveMenu.SetActive(true);
    }
}

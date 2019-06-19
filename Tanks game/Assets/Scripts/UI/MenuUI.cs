using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public InputField NameInput;
    public Button PlayFullSessionButton;
    public Toggle SaveToJSonToggle;

    private void Update()
    {
        if (!PlayFullSessionButton.interactable)
        {
            if (NameInput.text.Length > 0)
                PlayFullSessionButton.interactable = true;
        }
        else
        {
            if (NameInput.text.Length == 0)
                PlayFullSessionButton.interactable = false;
        }
    }

    public void LoadScene(int sceneID)
    {
        PlayerPrefs.SetString("PlayerName", NameInput.text);
        //if (SaveToJSonToggle.isOn)
        //    PlayerPrefs.SetInt("SaveToJSon", 1);
        //else
        //    PlayerPrefs.SetInt("SaveToJSon", 0);

        SceneManager.LoadScene(sceneID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
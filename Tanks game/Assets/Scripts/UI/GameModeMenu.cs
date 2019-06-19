using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeMenu : MonoBehaviour
{
    //public Text m_Tanks;
    public Text m_GameModeText;
    public Text m_ModeExplanationText;
    public Text m_GamesWonText;
    public Button m_PlayButton;
    public Button m_SaveButton;
    public Toggle m_JSonToggle;

    private void Awake()
    {
        EnableMenu(false);
        m_SaveButton.gameObject.SetActive(false);
        m_JSonToggle.gameObject.SetActive(false);
    }

    public void EnablePlayButton()
    {
        m_PlayButton.gameObject.SetActive(true);
        m_PlayButton.GetComponentInChildren<Text>().text = "Next round";
    }

    public void EnableMenu(bool enable)
    {
        //m_Tanks.gameObject.SetActive(enable);
        m_GameModeText.gameObject.SetActive(enable);
        m_ModeExplanationText.gameObject.SetActive(enable);
        m_GamesWonText.gameObject.SetActive(enable);
        m_PlayButton.gameObject.SetActive(enable);
        m_PlayButton.GetComponentInChildren<Text>().text = "Play";
    }

    public void SetGameMode(GameMode mode, int gamesWon)
    {
        EnableMenu(true);

        switch (mode)
        {
            case GameMode.Tutorial:
                m_GamesWonText.text = "Practice mode";
                m_GameModeText.text = "You will not take damage in practice mode\nPress Start session to leave practice mode";
                m_ModeExplanationText.text = "Use WASD/Arrow keys to move\nPress/hold space bar to fire, R to reload";
                break;
            case GameMode.Normal:
                m_GamesWonText.text = "You'll be playing 10 games of each game mode";
                m_GameModeText.text = "Normal game mode";
                m_ModeExplanationText.text = "Try to destroy all tanks without dying";
                break;
            case GameMode.Timer:
                m_GamesWonText.text = "You won " + gamesWon.ToString() + "/10 games";
                m_GameModeText.text = "Timer game mode";
                m_ModeExplanationText.text = "You now only have 45 seconds to destroy all tanks, hurry up!";
                break;
            case GameMode.LowHP:
                m_GamesWonText.text = "You won " + gamesWon.ToString() + "/10 games";
                m_GameModeText.text = "1HP game mode";
                m_ModeExplanationText.text = "You're health now starts at 1, be careful!";
                break;
            default:
                break;
        }
    }

    public void SetGameFinish(int gamesWon)
    {
        EnableMenu(true);
        m_PlayButton.gameObject.SetActive(false);
        m_SaveButton.gameObject.SetActive(true);
        m_JSonToggle.gameObject.SetActive(true);

        m_GamesWonText.text = "You won " + gamesWon.ToString() + "/10 games";
        m_GameModeText.text = "Thank you for playing";
        m_ModeExplanationText.text = "Press the button below to save all your data\nA folder called SavedSequences will be created in the game's directory";
    }

    public void SaveAndExit()
    {
        EnableMenu(false);
        m_SaveButton.gameObject.SetActive(false);
        bool saveJson = m_JSonToggle.isOn;
        m_JSonToggle.gameObject.SetActive(false);
        GameObject.FindObjectOfType<PictorialRepresentationSaver>().SaveAllStoredSequences(saveJson);
    }
}
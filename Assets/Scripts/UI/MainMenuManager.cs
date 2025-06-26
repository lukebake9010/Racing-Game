using UnityEngine;

/// <summary>
/// Basic Manager class containing functions for the main menu buttons.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Function for Logic for the "Play" button.
    /// </summary>
    public void OnPlayButton()
    {

    }

    /// <summary>
    /// Function for Logic for the "Highscores" button.
    /// </summary>
    public void OnHighscoresButton()
    {

    }

    /// <summary>
    /// Function for Logic for the "Credits" button.
    /// </summary>
    public void OnCreditsButton()
    {

    }

    /// <summary>
    /// Function for Logic for the "Quit" button.
    /// 
    /// <para>
    /// Simply exits the application.
    /// </para>
    /// </summary>
    public void OnQuitButton()
    {
        //Quits the application
        Application.Quit();
    }
}

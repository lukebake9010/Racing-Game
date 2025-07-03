using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Basic Manager class containing functions for the main menu buttons.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// The loading menu to activate on play
    /// </summary>
    [SerializeField]
    private LoadingMenu sceneLoadingMenu;

    /// <summary>
    /// Function for Logic for the "Play" button.
    /// </summary>
    public void OnPlayButton()
    {
        if(sceneLoadingMenu != null)
        {
            int testSceneID = Utilities.SceneManagement.SceneUtils.GetSceneBuildIndexByName("TestScene");
            if (testSceneID != -1)
            {
                sceneLoadingMenu.ShowAndLoadScene(testSceneID);//Show loading menu and load testscene
            }
            
        }
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

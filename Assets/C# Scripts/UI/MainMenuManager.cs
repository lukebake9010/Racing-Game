using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Basic Manager class containing functions for the main menu buttons.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// The loading menu to activate on play
    /// </summary>
    [SerializeField]
    private LoadingMenu sceneLoadingMenu;

    /// <summary>
    /// An internal reference to the transform the menu buttons are children to.
    /// Synonymous with the menu manager, or the parent object of this script.
    /// </summary>
    private Transform _mainMenuWrapper;
    private Transform _settingsMenuWrapper;
    private Transform _creditsWrapper;

    #endregion

    #region Logic

    private void Awake()
    {
        _mainMenuWrapper = transform.GetChild(0);
        _settingsMenuWrapper = transform.GetChild(1);
        _creditsWrapper = transform.GetChild(2);
    }

    private void Start()
    {
        foreach(Transform child in transform)
            child.gameObject.SetActive(false);
        _mainMenuWrapper.gameObject.SetActive(true);
    }


    /// <summary>
    /// Function for Logic for the "Play" button.
    /// </summary>
    public void OnPlayButton()
    {
        Debug.Log("Play button pressed.");

        if (sceneLoadingMenu != null)
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
        Debug.Log("Highscores button pressed.");
    }

    /// <summary>
    /// Function for Logic for the "Settings" button.
    /// </summary>
    public void OnSettingsButton()
    {
        Debug.Log("Settings button pressed.");

        //Disable the main menu
        _mainMenuWrapper.gameObject.SetActive(false);
      
        //Open the settings menu
        _settingsMenuWrapper.gameObject.SetActive(true);
    }

    /// <summary>
    /// Function for Logic for the "Credits" button.
    /// </summary>
    public void OnCreditsButton()
    {
        Debug.Log("Credit button pressed.");

        //Disable the main menu
        _mainMenuWrapper.gameObject.SetActive(false);

        //Open the settings menu
        _creditsWrapper.gameObject.SetActive(true);
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
        Debug.Log("Quit button pressed.");
        //Quits the application
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        _mainMenuWrapper.gameObject.SetActive(true);
    }

    #endregion

    #region Methods

    #endregion
}

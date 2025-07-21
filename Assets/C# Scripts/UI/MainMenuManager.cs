using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.UI;

/// <summary>
/// Basic Manager class containing functions for the main menu buttons.
/// </summary>
public class MainMenuManager : WindowedMenuManager
{
    #region Variables

    /// <summary>
    /// A reference to the prefab of the loading menu.
    /// Instantiated on Awake, and activated on play.
    /// Keeps the object hierarchy "tidy".
    /// </summary>
    [SerializeField]
    private GameObject loadingMenuPrefab;

    /// <summary>
    /// A reference to the Instantiated loading menu to activate on play.
    /// </summary>
    private LoadingMenu _sceneLoadingMenu;

    #endregion

    /// <summary>
    /// Instantiate the prefab on awake and store the reference in a private variable.
    /// </summary>
    private void Awake()
    {
        if(loadingMenuPrefab != null)
            _sceneLoadingMenu = Instantiate(loadingMenuPrefab, parent:transform.parent).GetComponent<LoadingMenu>();
    }

    #region Logic

    /// <summary>
    /// Function for Logic for the "Play" button.
    /// </summary>
    public void OnPlayButton()
    {
        //DEBUG
        Debug.Log("Play button pressed.");

        //If the scene loader exists, use it to switch scenes.
        if (_sceneLoadingMenu != null)
        {
            int testSceneID = Utilities.SceneManagement.SceneUtils.GetSceneBuildIndexByName("TestScene");
            if (testSceneID != -1)
            {
                _sceneLoadingMenu.ShowAndLoadScene(testSceneID);//Show loading menu and load testscene
            }
            
        }
    }

    /// <summary>
    /// Function for Logic for a simple "Quit" button.
    /// <para>
    /// Simply exits the application.
    /// </para>
    /// </summary>
    public void Quit()
    {
        //DEBUG
        Debug.Log("Quit button pressed.");
        //Quits the application
        Application.Quit();
    }

    #endregion

}

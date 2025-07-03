using UnityEngine;
using Utilities.SceneManagement;

public class LoadSceneOnStart : MonoBehaviour
{
    [SerializeField]
    private int sceneID = 0;

    [SerializeField]
    private LoadingMenu loadingMenu;

    void Start()
    {        
        if(loadingMenu != null) 
        {
            loadingMenu.ShowAndLoadScene(sceneID);
        }
        else
        {
            SceneLoader sceneLoader = gameObject.AddComponent<SceneLoader>();
            sceneLoader.StartLoadScene(sceneID);
        }
    }

}

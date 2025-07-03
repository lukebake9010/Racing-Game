using System;
using UnityEngine;
using UnityEngine.InputSystem;
using RacingGame.TestCar;
using TestCarIAC = RacingGame.TestCar.TestCarInputActionsCollection;
using UnityEditor;


public class RacingGameManager : MonoBehaviour
{
    private void Start()
    {
        TestCarIAC.Menu.performed += OnMenuPressed;
    }

    private void OnDestroy()
    {
        TestCarIAC.Menu.performed -= OnMenuPressed;
    }

    /// <summary>
    /// When 'Menu' input is pressed
    /// </summary>
    private void OnMenuPressed(InputAction.CallbackContext context)
    {
        //For now, exit the game on escape

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit(); 
    }
}

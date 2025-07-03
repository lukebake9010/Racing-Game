using UnityEngine;
using UnityEngine.InputSystem;


namespace RacingGame.TestCar
{
    public class TestCarInputActionsCollection
    {
        private static TestCarInputActions _input;

        static TestCarInputActionsCollection()
        {
            _input = new TestCarInputActions();
            _input.Enable();
        }

        //Action Maps
        public static TestCarInputActions.TestCarActions TestCarActions => _input.TestCar;

        #region Player Inputs
        public static InputAction Move => TestCarActions.Move;
        public static InputAction Menu => TestCarActions.Menu;

        #endregion
    }
}

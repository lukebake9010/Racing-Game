using RacingGame.TestCar;
using TMPro;
using UnityEngine;

namespace RacingGame.TestCar
{
    public class CarHUDController : MonoBehaviour
    {
        [SerializeField]
        private TestCarController carController;

        [Header("Speedometer")]
        [SerializeField]
        private TextMeshProUGUI speedometerText;
        [SerializeField]
        private float mpsToMPHMultiplier = 2.24f;

        void Update()
        {
            UpdateSpeedometer();
        }

        private void UpdateSpeedometer()
        {
            if (carController == null)
            {
                Debug.LogError("No car controller supplied to CarHudController");
            }
            if (speedometerText == null) return;

            int speedometerReading = (int)(carController.DrivingSpeed * mpsToMPHMultiplier);
            speedometerText.text = speedometerReading.ToString();
        }
    }
}

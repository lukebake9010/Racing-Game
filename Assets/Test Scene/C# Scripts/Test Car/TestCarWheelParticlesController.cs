using RacingGame.TestCar;
using System.Collections.Generic;
using UnityEngine;

namespace RacingGame.TestCar
{
    public class TestCarWheelParticlesController : MonoBehaviour
    {
        [SerializeField]
        private TestCarController carController;

        [SerializeField]
        private List<ParticleSystem> wheelParticles = new List<ParticleSystem>();

        /// <summary>
        /// The default color of wheelparticles when no drifting is ocurring
        /// </summary>
        [SerializeField]
        private Color drivingColor = Color.gray;
        /// <summary>
        /// The color that <see cref="driftingColor"/> will transition to depending on the drifting amount
        /// </summary>
        [SerializeField]
        private Color driftingColor = Color.black;

        /// <summary>
        /// The drifting amount necessary for <see cref="drivingColor"/> to become <see cref="driftingColor"/>
        /// </summary>
        [SerializeField]
        private float fullDriftMagnitude = 5f;

        private void LateUpdate()
        {
            UpdateWheelParticles();
        }

        /// <summary>
        /// Gets speed and drifting amount, and calculates particle states and colors for the wheel particle systems.
        /// </summary>
        private void UpdateWheelParticles()
        {
            if (carController == null) return;

            bool isDriving = carController.IsDriving;
            if (isDriving)
            {
                SetParticlesActive(true);
                float drivingSpeed = carController.DrivingSpeed;
                bool isDrifting = carController.IsDrifting;
                Color particleColor = drivingColor;

                if (isDrifting)
                {
                    float driftMagnitude = carController.DriftMagnitude;
                    float driftFraction = Mathf.Clamp01(driftMagnitude / fullDriftMagnitude);
                    particleColor = Color.Lerp(drivingColor, driftingColor, driftFraction);
                }

                SetParticleStartColor(particleColor);
            }
            else
            {
                SetParticlesActive(false);
            }
        }

        /// <summary>
        /// Sets the active state of all wheel particle systems
        /// </summary>
        /// <param name="active">Active state to set</param>
        private void SetParticlesActive(bool active)
        {
            foreach (ParticleSystem particle in wheelParticles)
            {
                if (active)
                {
                    if (!particle.isPlaying)
                        particle.Play();
                }
                else
                {
                    if (particle.isPlaying)
                        particle.Stop();
                }
            }
        }

        /// <summary>
        /// Sets the startcolor of all wheel particle systems
        /// </summary>
        /// <param name="color">The new startcolor</param>
        private void SetParticleStartColor(Color color)
        {
            foreach (ParticleSystem particle in wheelParticles)
            {
                ParticleSystem.MainModule particleMainModule = particle.main;
                particleMainModule.startColor = color;
            }
        }
    }
}

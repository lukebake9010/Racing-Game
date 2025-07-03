using RacingGame.TestCar;
using System.Collections.Generic;
using UnityEngine;

public class TestCarWheelParticlesController : MonoBehaviour
{
    [SerializeField]
    private TestCarController carController;

    [SerializeField]
    private List<ParticleSystem> wheelParticles = new List<ParticleSystem>();

    [SerializeField]
    private Color drivingColor = Color.gray;
    [SerializeField]
    private Color driftingColor = Color.black;

    [SerializeField]
    private float fullDriftMagnitude = 5f;
        
    private void LateUpdate()
    {
        if(carController == null) return;

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

    private void SetParticlesActive(bool active)
    {
        foreach(ParticleSystem particle in wheelParticles)
        {
            if (active)
            {
                if(!particle.isPlaying) 
                    particle.Play();
            }
            else
            {
                if (particle.isPlaying)
                    particle.Stop();
            }
        }
    }

    private void SetParticleStartColor(Color color)
    {
        foreach(ParticleSystem particle in wheelParticles)
        {
            ParticleSystem.MainModule particleMainModule = particle.main;
            particleMainModule.startColor = color;
        }
    }
}

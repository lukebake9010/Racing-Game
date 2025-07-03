using System;
using UnityEngine;

using TestCarIAC = RacingGame.TestCar.TestCarInputActionsCollection;

namespace RacingGame.TestCar
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TestCarController : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D rb2D;

        #region Update

        private void FixedUpdate()
        {
            HandleCarUpdate();
        }

        /// <summary>
        /// Cars fixed update
        /// <para>
        /// <list type="bullet">
        /// <item>Handles Input (WASD)</item>
        /// <item>Calculates car physics parameters</item>
        /// <item>Handles rigidbody forces</item>
        /// </list>
        /// </para>
        /// </summary>
        private void HandleCarUpdate()
        {
            HandleUserInput();
            CalculateCarPhysics();
        }

        #endregion


        #region User Input

        /// <summary>
        /// Steering of the player represented by the digital range -1,0,1
        /// </summary>
        private float digitalSteeringAxis = 0;

        /// <summary>
        /// Acceleration of the player represented by the digital range -1,0,1
        /// </summary>
        private float digitalAccelerationAxis = 0;

        /// <summary>
        /// Gets user input to calculate the following:
        /// <list type="bullet">
        /// <item>Steering Axis</item>
        /// <item>Acceleration/Deceleration</item>
        /// </list>
        /// </summary>
        private void HandleUserInput()
        {
            digitalSteeringAxis =  TestCarIAC.Move.ReadValue<Vector2>().x;
            
            digitalAccelerationAxis = TestCarIAC.Move.ReadValue<Vector2>().y;

            Debug.Log($"Steering: {digitalSteeringAxis}\n Acceleration: {digitalAccelerationAxis}");
        }
        #endregion


        #region Car Physics

        /// <summary>
        /// The magnitude of acceleration/deceleration applied to the car
        /// </summary>
        [SerializeField] float accelerationMultiplier = 1f; //This should be better named

        [SerializeField] float maxSpeed = 20f;

        [SerializeField] float steeringMultiplier = 200f;

        [SerializeField] float driftFactor = 0.95f;

        private void CalculateCarPhysics()
        {
            CalculateAcceleration();

            CalculateSteering();

            KillDrifting();
        }

        /// <summary>
        /// The speed required for the car to be considered 'Driving'
        /// </summary>
        const float drivingThreshold = 1f;

        /// <summary>
        /// Is the car driving? (Moving at speed greater than <see cref="drivingThreshold"/>)
        /// </summary>
        public bool IsDriving
        {
            get
            {
                return Mathf.Abs(rb2D.linearVelocity.magnitude) > drivingThreshold;
            }
        }

        /// <summary>
        /// The speed of the car
        /// </summary>
        public float DrivingSpeed
        {
            get
            {
                return rb2D.linearVelocity.magnitude;
            }
        }

        /// <summary>
        /// Calculates the forward force applied to the car
        /// </summary>
        private void CalculateAcceleration()
        {
            float currentSpeed = Vector2.Dot(rb2D.linearVelocity, transform.up);
            Debug.Log($"Speed: {currentSpeed}");

            Vector2 force = transform.up * digitalAccelerationAxis * accelerationMultiplier;
            rb2D.AddForce(force, ForceMode2D.Force);
        }

        /// <summary>
        /// Calculates the rotation applied to the car
        /// </summary>
        private void CalculateSteering()
        {
            float speedFactor = rb2D.linearVelocity.magnitude / maxSpeed;
            float rotationAmount = digitalSteeringAxis * steeringMultiplier * speedFactor;
            rb2D.MoveRotation(rb2D.rotation - rotationAmount * Time.fixedDeltaTime);
        }


        private const float driftThreshold = 1f;

        private bool isDrifting = false;
        public bool IsDrifting
        {
            get
            {
                return isDrifting;
            }
            private set
            {
                if (isDrifting != value)
                {
                    isDrifting = value;
                }
            }
        }

        private float driftMagnitude = 0f;
        public float DriftMagnitude
        {
            get 
            { 
                return driftMagnitude; 
            }
            private set
            {
                driftMagnitude = value;
            }
        }

        /// <summary>
        /// Stops sideways forces to avoid drifting. Alter <see cref="driftFactor"/> to change the amount of drifting
        /// </summary>
        void KillDrifting()
        {
            Vector2 forward = transform.up * Vector2.Dot(rb2D.linearVelocity, transform.up);
            float driftAmount = Vector2.Dot(rb2D.linearVelocity, transform.right);
            DriftMagnitude = Mathf.Abs(driftAmount);
            IsDrifting = DriftMagnitude > driftThreshold;
            Vector2 sideways = transform.right * driftAmount;
            rb2D.linearVelocity = forward + sideways * driftFactor;
        }

        #endregion
    }
}

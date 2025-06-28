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

            KillOrthogonalVelocity();
        }

        private void CalculateAcceleration()
        {
            float currentSpeed = Vector2.Dot(rb2D.linearVelocity, transform.up);
            Debug.Log($"Speed: {currentSpeed}");

            Vector2 force = transform.up * digitalAccelerationAxis * accelerationMultiplier;
            rb2D.AddForce(force, ForceMode2D.Force);
        }

        private void CalculateSteering()
        {
            float speedFactor = rb2D.linearVelocity.magnitude / maxSpeed;
            float rotationAmount = digitalSteeringAxis * steeringMultiplier * speedFactor;
            rb2D.MoveRotation(rb2D.rotation - rotationAmount * Time.fixedDeltaTime);
        }

        void KillOrthogonalVelocity()
        {
            Vector2 forward = transform.up * Vector2.Dot(rb2D.linearVelocity, transform.up);
            Vector2 sideways = transform.right * Vector2.Dot(rb2D.linearVelocity, transform.right);
            rb2D.linearVelocity = forward + sideways * driftFactor;
        }


        private void HandleRigidbodyForces()
        {

        }

        #endregion
    }
}

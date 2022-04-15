using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterVelocity))]
    public class CharacterHorizontalMovement : CharacterAbility
    {
        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private SpriteRenderer _sprite;

        // TODO: runAcclel and airAccel should be in appropriate states.
        [SerializeField]
        private float _runAcceleration = 100;

        [SerializeField]
        private float _airAcceleration = 50;

        private CharacterMovement _movement;
        private CharacterVelocity _velocity;

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>(); 
            _velocity = GetComponent<CharacterVelocity>(); 
        }

        private void Update()
        {
            // TODO: sometimes is gives false when character is on the ground!
            // TODO: should update after collisions had resolved.
            HandleMovement();

            RotateToMovementDirection();
        }

        private void HandleMovement()
        {
            // TODO: Should get the input from other component.
            float moveInput = Input.GetAxisRaw("Horizontal");

            float acceleration = _movement.IsGrounded ? _runAcceleration :
                _airAcceleration;

            _velocity.X = Mathf.MoveTowards(_velocity.X, _speed * moveInput,
                acceleration * Time.deltaTime);
        }

        private void RotateToMovementDirection()
        {
            if (_velocity.X > .01f)
                _sprite.flipX = false;

            if (_velocity.X < -.01)
                _sprite.flipX = true;
        }
    }
}

using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterVelocity))]
    public class CharacterJump : MonoBehaviour
    {
        [SerializeField]
        private float _jumpHeight = 9f;

        [SerializeField]
        private int _maxAirJumps = 1;

        [SerializeField]
        private Animator _animator;

        private CharacterMovement _movement;
        private CharacterVelocity _velocity;
        private int _airJumps = 0;
        private readonly int _animJump = Animator.StringToHash("Jump");

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
            _velocity = GetComponent<CharacterVelocity>();
        }

        private void Update()
        {
            // TODO: Get input from other script.
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        public virtual void Jump()
        {
            if (_movement.IsGrounded)
            {
                ApplyVelocity();
                _airJumps = 0;
            }
            else if (_airJumps < _maxAirJumps)
            {
                ApplyVelocity();
                _airJumps++;
            }

            _animator.SetTrigger(_animJump);

            void ApplyVelocity()
            {
                _velocity.Y = Mathf.Sqrt(2 * _jumpHeight *
                    Mathf.Abs(Physics2D.gravity.y));
            }
        }
    }
}

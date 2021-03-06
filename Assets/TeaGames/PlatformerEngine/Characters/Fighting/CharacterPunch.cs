using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(CharacterMovement))]
    public class CharacterPunch : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private float _cooldown = .5f;
        
        [SerializeField]
        private float _movementCooldown = .1f;

        private CharacterMovement _movement;
        private float _lastTimePunch = float.MinValue;

        private readonly int _animPunch =
            Animator.StringToHash("Punch");

        private void Awake()
        {
            _movement = GetComponent<CharacterMovement>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
                TryPunch(1f);
            
            if (Time.time - _lastTimePunch > _movementCooldown)
            {
                _movement.SetMovementEnabled(true);
                _movement.SetJumpingEnabled(true);
            }
        }

        private void TryPunch(float dir)
        {
            if (Time.time - _lastTimePunch > _cooldown)
                Punch(dir);
        }

        private void Punch(float dir)
        {
            _movement.StopHorizontalMovement();
            _movement.SetMovementEnabled(false);
            _movement.SetJumpingEnabled(false);

            _animator.SetTrigger(_animPunch);

            _lastTimePunch = Time.time;
        }
    }
}

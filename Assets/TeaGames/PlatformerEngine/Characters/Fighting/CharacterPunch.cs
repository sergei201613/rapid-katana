using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class CharacterPunch : MonoBehaviour
    {
        [SerializeField]
        private float _cooldown = .5f;

        [SerializeField]
        private Animator _animator;

        // TODO: Maybe i don't need CharacterVelocity component?
        private float _lastTimePunch = float.MinValue;
        private readonly int _animPunch = Animator.StringToHash("Punch");

        private void Update()
        {
            // TODO: Use new InputSystem?
            if (Input.GetKeyDown(KeyCode.J))
                TryPunch(1f);
        }

        private void TryPunch(float dir)
        {
            if (Time.time - _lastTimePunch > _cooldown)
                Punch(dir);
        }

        private void Punch(float dir)
        {
            // TODO: While is dashing i need to stop movement.
            _animator.SetTrigger(_animPunch);
            _lastTimePunch = Time.time;
        }
    }
}

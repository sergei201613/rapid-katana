using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class CharacterDash : MonoBehaviour
    {
        [SerializeField]
        private float _doubleClickMaxTime = .2f;

        [SerializeField]
        private float _speed = 100f;

        [SerializeField]
        private float _duration = .5f;

        [SerializeField]
        private float _cooldown = 1f;

        private float _lastTimeLeftPressed = float.MinValue;
        private float _lastTimeRightPressed = float.MinValue;
        // TODO: Maybe i don't need CharacterVelocity component?
        private Vector2 _velocity;
        private float _lastTimeDash = float.MinValue;

        private void Update()
        {
            if (Time.time - _lastTimeDash > _duration)
            {
                _velocity = Vector2.zero;
            }

            // TODO: Use new InputSystem?
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - _lastTimeRightPressed < _doubleClickMaxTime)
                {
                    TryDash(1f);
                }

                _lastTimeRightPressed = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - _lastTimeLeftPressed < _doubleClickMaxTime)
                {
                    TryDash(-1f);
                }

                _lastTimeLeftPressed = Time.time;
            }

            transform.Translate(_velocity * Time.deltaTime);
        }

        private void TryDash(float dir)
        {
            if (Time.time - _lastTimeDash > _cooldown)
                Dash(dir);
        }

        private void Dash(float dir)
        {
            // TODO: While is dashing i need to stop movement.
            _velocity.x += _speed * dir;
            _lastTimeDash = Time.time;
        }
    }
}

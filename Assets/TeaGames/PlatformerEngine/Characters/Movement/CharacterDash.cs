using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(CharacterVelocity))]
    public class CharacterDash : CharacterAbility
    {
        [SerializeField]
        private float _doubleClickMaxTime = .2f;

        [SerializeField]
        private float _speed = 100f;

        [SerializeField]
        private float _duration = .5f;

        [SerializeField]
        private float _cooldown = 1f;

        [SerializeField]
        private CharacterAbility[] _abilitiesToDisableWhenDashing;

        /// <summary>
        /// Delay before "abilities to disable when dashing" will enabled when dash over.
        /// </summary>
        [SerializeField]
        private float _abilitiesCooldown;

        private CharacterVelocity _velocity;
        private float _lastTimeLeftPressed = float.MinValue;
        private float _lastTimeRightPressed = float.MinValue;
        private float _lastTimeDashed = float.MinValue;
        private bool _isDashing = false;

        private void Awake()
        {
            _velocity = GetComponent<CharacterVelocity>();
        }

        private void Update()
        {
            if (Time.time - _lastTimeDashed > _duration)
            {
                if (_isDashing)
                    EndDash();
            }

            // TODO: Use new InputSystem?
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - _lastTimeRightPressed < _doubleClickMaxTime)
                {
                    TryStartDash(1f);
                }

                _lastTimeRightPressed = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - _lastTimeLeftPressed < _doubleClickMaxTime)
                {
                    TryStartDash(-1f);
                }

                _lastTimeLeftPressed = Time.time;
            }
        }

        private void TryStartDash(float dir)
        {
            if (Time.time - _lastTimeDashed > _cooldown)
                StartDash(dir);
        }

        private void StartDash(float dir)
        {
            SetMovementEnabled(false);

            _velocity.X += _speed * dir;
            _lastTimeDashed = Time.time;
            _isDashing = true;
        }

        private void EndDash()
        {
            _isDashing = false;

            SetMovementEnabled(true);
        }

        private void SetMovementEnabled(bool val)
        {
            foreach (var component in _abilitiesToDisableWhenDashing)
                component.enabled = val; 
        }
    }
}

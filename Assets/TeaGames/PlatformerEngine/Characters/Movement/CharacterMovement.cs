using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterMovement : MonoBehaviour
    {
        public Vector2 Velocity => _velocity;
        public Vector2 Delta => _delta;
        public bool IsGrounded => _isGrounded;
        public bool IsDashing => _isDashing;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private LayerMask _collisionLayers;

        [SerializeField]
        private LayerMask _platformLayers;

        [SerializeField]
        private SpriteRenderer _sprite;

        [SerializeField]
        private float _gravityMultiplier = 3f;

        [Header("Jump")]

        [SerializeField]
        private float _jumpHeight = 9f;

        [SerializeField]
        private int _maxAirJumps = 1;

        [Header("Horizontal movement")]

        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private float _runAcceleration = 100;

        [SerializeField]
        private float _airAcceleration = 50;

        [Header("Dash")]

        [SerializeField]
        private float _dashDblClickMaxTime = .2f;

        [SerializeField]
        private float _dashSpeed = 100f;

        [SerializeField]
        private float _dashDuration = .5f;

        [SerializeField]
        private float _dashCooldown = 1f;

        private float _lastTimeLeftDashPressed = float.MinValue;
        private float _lastTimeRightDashPressed = float.MinValue;
        private float _lastTimeDashed = float.MinValue;
        private bool _isDashing = false;

        private MovementState _state;
        private BoxCollider2D _boxCollider;
        private Vector2 _delta;
        private Vector2 _prevPos;
        private Vector2 _velocity;
        private float _boxCollPrevBoundsMinY;
        private int _airJumps = 0;
        private bool _canHorizontalMovement = true;
        private bool _canJump = true;
        private bool _isGravityActive = true;
        private bool _isGrounded;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>(); 

            _prevPos = transform.position;

            _state = new MovementStateIdle(this);
        }

        private void Update()
        {
            AddGravityVel();
            HandleJumpVel();
            HandleMovementVel();
            HandleDashVel();

            ApplyVelocity();

            UpdateIsGrounded();

            // TODO: Bad hardcoding.
            for (int i = 0; i < 4; i++)
                ResolveCollisions();

            ResolvePlatformCollisions();
            FixVelocity();
            RotateToMovementDirection();

            _state.Update();

            UpdateMovementDelta();
        }

        public void SetState(MovementState stateToTransition)
        {
            _state = stateToTransition;
        }

        public void SetMovementEnabled(bool value)
        {
            _canHorizontalMovement = value;
        }

        public void SetJumpingEnabled(bool value)
        {
            _canJump = value;
        }

        public void StopMovement()
        {
            _velocity.x = 0;
        }

        public void PlayAnimation(int anim)
        {
            _animator.Play(anim, -1, 0);
        }

        private void HandleJumpVel()
        {
            if (!_canJump)
                return;

            // TODO: Get input from other script.
            if (Input.GetButtonDown("Jump"))
            {
                AddJumpVelocity();
            }
        }

        private void UpdateMovementDelta()
        {
            _delta = ((Vector2)transform.position - _prevPos) /
                Time.deltaTime;

            _prevPos = transform.position;
        }

        private void FixVelocity()
        {
            _velocity = ((Vector2)transform.position - _prevPos) / Time.deltaTime;
        }

        private void ApplyVelocity()
        {
            transform.Translate(_velocity * Time.deltaTime);
        }

        private void AddGravityVel()
        {
            if (!_isGravityActive)
                return;

            _velocity.y += Physics2D.gravity.y * _gravityMultiplier * 
                Time.deltaTime;
        }

        private void AddJumpVelocity()
        {
            if (_isGrounded)
            {
                ApplyVelocity();
                _airJumps = 0;
            }
            else if (_airJumps < _maxAirJumps)
            {
                ApplyVelocity();
                _airJumps++;
            }

            SetState(new MovementStateJump(this));

            void ApplyVelocity()
            {
                _velocity.y = Mathf.Sqrt(2 * _jumpHeight *
                    Mathf.Abs(Physics2D.gravity.y));
            }
        }

        private void HandleMovementVel()
        {
            if (!_canHorizontalMovement)
                return;

            // TODO: Should get the input from other component.
            float moveInput = Input.GetAxisRaw("Horizontal");

            float acceleration = _isGrounded ? _runAcceleration :
                _airAcceleration;

            _velocity.x = Mathf.MoveTowards(_velocity.x, _speed * moveInput,
                acceleration * Time.deltaTime);
        }

        private void HandleDashVel()
        {
            if (Time.time - _lastTimeDashed > _dashDuration)
            {
                if (_isDashing)
                    EndDash();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Time.time - _lastTimeRightDashPressed < _dashDblClickMaxTime)
                {
                    TryStartDash(1f);
                }

                _lastTimeRightDashPressed = Time.time;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - _lastTimeLeftDashPressed < _dashDblClickMaxTime)
                {
                    TryStartDash(-1f);
                }

                _lastTimeLeftDashPressed = Time.time;
            }
        }

        private void TryStartDash(float dir)
        {
            if (Time.time - _lastTimeDashed > _dashCooldown)
                StartDash(dir);
        }

        private void StartDash(float dir)
        {
            _canJump = false;
            _canHorizontalMovement = false;
            _isGravityActive = false;

            _velocity.x += _dashSpeed * dir;
            _velocity.y = 0;

            _lastTimeDashed = Time.time;
            _isDashing = true;
        }

        private void EndDash()
        {
            _isDashing = false;

            _canJump = true;
            _canHorizontalMovement = true;
            _isGravityActive = true;

            _velocity.x = 0;
        }

        private void RotateToMovementDirection()
        {
            if (_velocity.x > .01f)
                _sprite.flipX = false;

            if (_velocity.x < -.01)
                _sprite.flipX = true;
        }

        private void UpdateIsGrounded()
        {
            var hits = Physics2D.OverlapBoxAll(_boxCollider.transform.position,
                _boxCollider.size, 0, _collisionLayers);

            _isGrounded = false;

            foreach (var hit in hits)
            {
                if (hit == _boxCollider)
                    continue;

                var colliderDistance = hit.Distance(_boxCollider);

                if (colliderDistance.isOverlapped)
                {
                    float angle = Vector2.Angle(colliderDistance.normal, 
                        Vector2.up);

                    if (angle < 45)
                        _isGrounded = true;
                }
            }
        }

        private void ResolveCollisions()
        {
            var hits = Physics2D.OverlapBoxAll(transform.position,
                _boxCollider.size, 0, _collisionLayers);

            foreach (var hit in hits)
            {
                if (hit == _boxCollider)
                    continue;

                var colliderDistance = hit.Distance(_boxCollider);

                if (colliderDistance.isOverlapped)
                {
                    transform.Translate(colliderDistance.pointA -
                        colliderDistance.pointB);
                }
            }
        }

        private void ResolvePlatformCollisions()
        {
            var hits = Physics2D.OverlapBoxAll(transform.position,
                _boxCollider.size, 0, _platformLayers);

            foreach (var hit in hits)
            {
                if (hit == _boxCollider)
                    continue;

                var colliderDistance = hit.Distance(_boxCollider);

                if (colliderDistance.isOverlapped)
                {
                    Vector2 dir = colliderDistance.pointA - colliderDistance.pointB;

                    if (_boxCollPrevBoundsMinY - hit.bounds.max.y < 0)
                        continue;

                    if (_boxCollider.bounds.min.y - hit.bounds.min.y < 0)
                        continue;

                    // TODO: shouldn't be there.
                    _isGrounded = true;

                    transform.Translate(dir);
                }
            }

            _boxCollPrevBoundsMinY = _boxCollider.bounds.min.y;
        }
    }
}

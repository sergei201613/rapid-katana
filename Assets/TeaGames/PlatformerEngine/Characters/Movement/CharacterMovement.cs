using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CharacterMovement : MonoBehaviour
    {
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
        private bool _isDash = false;

        private BoxCollider2D _boxCollider;
        private Vector2 _prevPos;
        private Vector2 _velocity;
        private float _boxCollPrevBoundsMinY;
        private int _airJumps = 0;
        private bool _canHorizontalMovement = true;
        private bool _canJump = true;
        private bool _isGravityActive = true;
        private bool _isGrounded;

        private readonly int _animIsRun =
            Animator.StringToHash("IsRun");
        private readonly int _animIsFall =
            Animator.StringToHash("IsFall");
        private readonly int _animIsDash =
            Animator.StringToHash("IsDash");
        private readonly int _animIsGrounded =
            Animator.StringToHash("IsGrounded");
        private readonly int _animJump =
            Animator.StringToHash("Jump");

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>(); 
            _prevPos = transform.position;
        }

        private void Update()
        {
            HandleGravity();
            HandleDash();
            HandleMovement();
            HandleJump();

            ApplyVelocity();

            ResolveCollisions(2);
            ResolvePlatformCollisions(2);

            FixVelocity();

            RotateToMovementDirection();
            UpdateAnimData();

            _prevPos = transform.position;
        }

        public void SetMovementEnabled(bool value)
        {
            _canHorizontalMovement = value;
        }

        public void SetJumpingEnabled(bool value)
        {
            _canJump = value;
        }

        public void StopHorizontalMovement()
        {
            _velocity.x = 0;
        }

        private void HandleJump()
        {
            if (!_canJump)
                return;

            if (_isDash)
                return;

            // TODO: Get input from other script.
            if (Input.GetButtonDown("Jump"))
            {
                AddJumpVelocity();
            }
        }

        private void UpdateAnimData()
        {
            _animator.SetBool(_animIsRun, 
                Mathf.Abs(_velocity.x) > .01);
            _animator.SetBool(_animIsFall, _velocity.y < -2);
            _animator.SetBool(_animIsDash, _isDash);
            _animator.SetBool(_animIsGrounded, _isGrounded);
        }

        private void FixVelocity()
        {
            _velocity = ((Vector2)transform.position - _prevPos) / Time.deltaTime;
        }

        private void ApplyVelocity()
        {
            transform.Translate(_velocity * Time.deltaTime);
        }

        private void HandleGravity()
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
                _isGrounded = false;
            }
            else if (_airJumps < _maxAirJumps)
            {
                ApplyVelocity();
                _airJumps++;
            }

            _animator.SetTrigger(_animJump);

            void ApplyVelocity()
            {
                _velocity.y = Mathf.Sqrt(2 * _jumpHeight *
                    Mathf.Abs(Physics2D.gravity.y));
            }
        }

        private void HandleMovement()
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

        private void HandleDash()
        {
            if (Time.time - _lastTimeDashed > _dashDuration)
            {
                if (_isDash)
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
            _isDash = true;
        }

        private void EndDash()
        {
            _isDash = false;

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

        private void ResolveCollisions(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var hits = Physics2D.OverlapBoxAll(transform.position,
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

                        transform.Translate(colliderDistance.pointA -
                            colliderDistance.pointB);
                    }
                }
            }
        }

        private void ResolvePlatformCollisions(int iterations)
        {
            for (int i = 0; i < iterations; i++)
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

                        _isGrounded = true;

                        transform.Translate(dir);
                    }
                }

                _boxCollPrevBoundsMinY = _boxCollider.bounds.min.y;
            }
        }
    }
}

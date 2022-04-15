using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(CharacterMovementAnalyzer))]
    public class CharacterMovement : MonoBehaviour
    {
        public bool IsGrounded => _isGrounded;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private LayerMask _collisionLayers;

        [SerializeField]
        private LayerMask _platformLayers;

        [SerializeField]
        private float _gravityMultiplier = 3f;

        [SerializeField]
        private float _jumpHeight = 9f;

        [SerializeField]
        private int _maxAirJumps = 1;

        [SerializeField]
        private float _speed = 5f;

        [SerializeField]
        private SpriteRenderer _sprite;

        [SerializeField]
        private float _runAcceleration = 100;

        [SerializeField]
        private float _airAcceleration = 50;

        private CharacterMovementState _state;
        private CharacterMovementAnalyzer _movementAnalyzer;
        private BoxCollider2D _boxCollider;
        private Vector2 _velocity;
        private Vector2 _prevPos;
        private float _boxCollPrevBoundsMinY;
        private int _airJumps = 0;
        private bool _isGrounded;

        private readonly int _animVelocityX = Animator.StringToHash("VelocityX");
        private readonly int _animVelocityY = Animator.StringToHash("VelocityY");
        private readonly int _animIsGrounded = Animator.StringToHash("IsGrounded");
        private readonly int _animJump = Animator.StringToHash("Jump");

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>(); 
            _movementAnalyzer = GetComponent<CharacterMovementAnalyzer>(); 

            _state = new CharacterMovementStateIdle();
        }

        private void Update()
        {
            AddGravityVel();
            HandleJumpVel();
            HandleMovementVel();

            ApplyVelocity();

            UpdateIsGrounded();

            ResolveCollisions();
            ResolvePlatformCollisions();

            CorrectVelocity();

            RotateToMovementDirection();
            UpdateAnimation();

            _prevPos = transform.position;
        }

        private void HandleJumpVel()
        {
            // TODO: Get input from other script.
            if (Input.GetButtonDown("Jump"))
            {
                AddJumpVelocity();
            }
        }

        private void CorrectVelocity()
        {
            _velocity = ((Vector2)transform.position - _prevPos) / Time.deltaTime;
        }

        private void ApplyVelocity()
        {
            transform.Translate(_velocity * Time.deltaTime);
        }

        private void AddGravityVel()
        {
            _velocity.y += Physics2D.gravity.y * _gravityMultiplier * 
                Time.deltaTime;
        }
        public void AddJumpVelocity()
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

            _animator.SetTrigger(_animJump);

            void ApplyVelocity()
            {
                _velocity.y = Mathf.Sqrt(2 * _jumpHeight *
                    Mathf.Abs(Physics2D.gravity.y));
            }
        }

        private void HandleMovementVel()
        {
            // TODO: Should get the input from other component.
            float moveInput = Input.GetAxisRaw("Horizontal");

            float acceleration = _isGrounded ? _runAcceleration :
                _airAcceleration;

            _velocity.x = Mathf.MoveTowards(_velocity.x, _speed * moveInput,
                acceleration * Time.deltaTime);
        }

        private void RotateToMovementDirection()
        {
            if (_velocity.x > .01f)
                _sprite.flipX = false;

            if (_velocity.x < -.01)
                _sprite.flipX = true;
        }

        private void UpdateAnimation()
        {
            _animator.SetFloat(_animVelocityX, 
                Mathf.Abs(_movementAnalyzer.MovementDelta.x));

            _animator.SetFloat(_animVelocityY, 
                _movementAnalyzer.MovementDelta.y);

            _animator.SetBool(_animIsGrounded, _isGrounded);
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

                    if (angle < 90f)
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

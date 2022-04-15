using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(CharacterVelocity))]
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

        private CharacterMovementState _state;
        private CharacterMovementAnalyzer _movementAnalyzer;
        private BoxCollider2D _boxCollider;
        private CharacterVelocity _velocity;
        private float _boxCollPrevBoundsMinY;
        private bool _isGrounded;
        private readonly int _animVelocityX = Animator.StringToHash("VelocityX");
        private readonly int _animVelocityY = Animator.StringToHash("VelocityY");
        private readonly int _animIsGrounded = Animator.StringToHash("IsGrounded");

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>(); 
            _velocity = GetComponent<CharacterVelocity>(); 
            _movementAnalyzer = GetComponent<CharacterMovementAnalyzer>(); 

            _state = new CharacterMovementStateIdle();
        }

        private void Update()
        {
            ApplyMovement();

            UpdateIsGrounded();

            ResolveCollisions();
            ResolvePlatformCollisions();

            UpdateAnimation();
        }

        private void ApplyMovement()
        {
            transform.Translate(_velocity.Velocity * Time.deltaTime);
            print($"pos.x: {transform.position.x} vel.x: {_velocity.X}");
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

                    _isGrounded = true;
                    transform.Translate(dir);
                }
            }

            _boxCollPrevBoundsMinY = _boxCollider.bounds.min.y;
        }
    }
}

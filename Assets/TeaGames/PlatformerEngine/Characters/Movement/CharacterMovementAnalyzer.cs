using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class CharacterMovementAnalyzer : MonoBehaviour
    {
        public Vector2 MovementDelta => _movementDelta;

        private Vector2 _prevPosition;
        private Vector2 _movementDelta;

        private void Awake()
        {
            _prevPosition = transform.position;
        }

        private void Update()
        {
            _movementDelta = ((Vector2)transform.position - _prevPosition) / 
                Time.deltaTime;

            _prevPosition = transform.position;
        }
    }
}

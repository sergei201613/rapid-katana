using UnityEngine;
using TeaGames.PlatformerEngine.Player;

namespace TeaGames.PlatformerEngine.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 5f;

        private Transform _target;
        private Vector3 _offset;

        private void Awake()
        {
            // TODO: do this with Zenject.
            _target = FindObjectOfType<PlayerCharacter>().transform;

            _offset = transform.position - _target.position;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, 
                _target.position + _offset, _speed * Time.deltaTime);
        }
    }
}

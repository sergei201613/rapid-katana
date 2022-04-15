using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class CharacterVelocity : MonoBehaviour
    {
        public float X
        {
            get => Velocity.x;
            set => Velocity.x = value;
        }
        public float Y
        {
            get => Velocity.y;
            set => Velocity.y = value;
        }

        public Vector2 Velocity;
    }
}

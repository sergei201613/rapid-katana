using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStatePunch : MovementState
    {
        public MovementStatePunch(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Punch");
        }

        public override MovementType GetMovementType()
        {
            return MovementType.Punch;
        }

        public override void Update()
        {
        }
    }
}
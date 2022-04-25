using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateFall : CharacterAnimationState
    {
        public MovementStateFall(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Fall");
        }

        public override MovementType GetMovementType()
        {
            return MovementType.Fall;
        }

        public override void Update()
        {
            HandleTransitionToRun();
            HandleTransitionToIdle();
        }
    }
}
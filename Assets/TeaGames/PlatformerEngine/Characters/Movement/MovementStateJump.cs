using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateJump : CharacterAnimationState
    {
        public MovementStateJump(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Jump");
        }

        public override MovementType GetMovementType()
        {
            return MovementType.Jump;
        }

        public override void Update()
        {
            HandleTransitionToFall();
            HandleTransitionToRun();
            HandleTransitionToIdle();
        }
    }
}
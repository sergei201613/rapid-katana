using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateRun : MovementState
    {
        public MovementStateRun(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Run");
        }

        public override MovementType GetMovementType()
        {
            return MovementType.Run;
        }

        public override void Update()
        {
            HandleTransitionToIdle();
        }
    }
}

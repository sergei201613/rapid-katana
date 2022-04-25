using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateIdle : CharacterAnimationState
    {
        public MovementStateIdle(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Idle");
        }

        public override MovementType GetMovementType()
        {
            return MovementType.Idle;
        }

        public override void Update()
        {
            HandleTransitionToRun();
        }
    }
}
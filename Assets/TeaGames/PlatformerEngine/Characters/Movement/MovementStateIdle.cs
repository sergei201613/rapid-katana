using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateIdle : MovementState
    {
        public MovementStateIdle(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Idle");
        }

        public override void Update()
        {
            HandleTransitionToRun();
        }
    }
}
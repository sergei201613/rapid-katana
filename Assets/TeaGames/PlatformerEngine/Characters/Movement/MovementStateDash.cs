using System;
using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public class MovementStateDash : MovementState
    {
        public MovementStateDash(CharacterMovement movement) : base(movement)
        {
        }

        public override int GetAnimationHash()
        {
            return Animator.StringToHash("Dash");
        }

        public override void Update()
        {
        }
    }
}
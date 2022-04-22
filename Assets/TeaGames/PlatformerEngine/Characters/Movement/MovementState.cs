using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public abstract class MovementState
    {
        protected Vector2 Velocity => Movement.Velocity;
        protected bool IsGrounded => Movement.IsGrounded;
        protected bool IsHorizontalMoving => Mathf.Abs(Velocity.x) > .1f;
        protected bool IsFalling => Velocity.y < -3f;

        protected CharacterMovement Movement;

        public MovementState(CharacterMovement movement)
        {
            Movement = movement;
            movement.PlayAnimation(GetAnimationHash());
        }

        public abstract void Update();

        public abstract int GetAnimationHash();

        public abstract MovementType GetMovementType();

        public virtual void HandleTransitionTo(MovementState state)
        {
            Movement.SetState(state);
        }

        protected virtual void HandleTransitionToIdle()
        {
            if (!IsGrounded)
                return;

            if (!IsHorizontalMoving)
            {
                Movement.SetState(new MovementStateIdle(Movement));
                return;
            }
        }

        protected virtual void HandleTransitionToRun()
        {
            if (!IsGrounded)
                return;

            if (IsHorizontalMoving)
            {
                Movement.SetState(new MovementStateRun(Movement));
                return;
            }
        }

        protected virtual void HandleTransitionToFall()
        {
            if (IsGrounded)
                return;

            if (IsFalling)
            {
                Movement.SetState(new MovementStateFall(Movement));
                return;
            }
        }
    }
}

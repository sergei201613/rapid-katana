using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    public abstract class CharacterAnimationState : Animation.AnimationState
    {
        protected Vector2 Velocity => Movement.Velocity;
        protected bool IsGrounded => Movement.IsGrounded;
        protected bool IsHorizontalMoving => Mathf.Abs(Velocity.x) > .1f;
        protected bool IsFalling => Velocity.y < -3f;

        protected CharacterMovement Movement;
        protected Animator Animator;

        public CharacterAnimationState(CharacterMovement movement)
        {
            Movement = movement;
            Animator = movement.Animator;

            Animator.Play(GetAnimationHash(), -1, 0);
        }

        public abstract void Update();

        public abstract int GetAnimationHash();

        public abstract MovementType GetMovementType();

        public virtual void HandleTransitionTo(CharacterAnimationState state)
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

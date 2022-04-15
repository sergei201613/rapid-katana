namespace TeaGames.PlatformerEngine.Characters
{
    public abstract class CharacterMovementState
    {
        public virtual void HandleTransitionTo(CharacterMovementState state)
        {
            throw new System.NotImplementedException();
        }
    }
}

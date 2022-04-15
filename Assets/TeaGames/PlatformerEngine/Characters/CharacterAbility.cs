using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    /// <summary>
    /// Base class for character abilities.
    /// </summary>
    public abstract class CharacterAbility : MonoBehaviour
    {
        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        private void OnEnable()
        {
            OnEnabled();
        }
        private void OnDisable()
        {
            OnDisabled();
        }

        protected virtual void OnEnabled() { }

        protected virtual void OnDisabled() { }
    }
}
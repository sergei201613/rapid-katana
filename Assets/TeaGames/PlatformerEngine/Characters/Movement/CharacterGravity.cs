﻿using UnityEngine;

namespace TeaGames.PlatformerEngine.Characters
{
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterVelocity))]
    public class CharacterGravity : MonoBehaviour
    {
        [SerializeField]
        private float _gravityMultiplier = 3f;

        private CharacterMovement _characterMovement; 
        private CharacterVelocity _velocity; 

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _velocity = GetComponent<CharacterVelocity>();
        }

        private void Update()
        {
            if (_characterMovement.IsGrounded)
            {
                _velocity.Y = Mathf.Lerp(_velocity.Y, 0f, 10f * Time.deltaTime);
                return;
            }

            _velocity.Y += Physics2D.gravity.y * _gravityMultiplier * 
                Time.deltaTime;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCharacter
{
    public class CharacterAnimation : MonoBehaviour
    {
        public Animator animator;
        
        private GameInput _gameInput;
        private Vector2 _moveDirection = Vector2.zero;
        
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
        private static readonly int LastVertical = Animator.StringToHash("LastVertical");

        private void Awake()
        {
            _gameInput = new GameInput();
        }

        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Movement.performed += OnMovementPerformed;
            _gameInput.Player.Movement.canceled += OnMovementCancelled;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Movement.performed -= OnMovementPerformed;
            _gameInput.Player.Movement.canceled -= OnMovementCancelled;
        }
    
        private void FixedUpdate()
        {
            animator.SetFloat(Horizontal, _moveDirection.x);
            animator.SetFloat(Vertical, _moveDirection.y);
            animator.SetFloat(Speed, _moveDirection.sqrMagnitude);

            if (_moveDirection == Vector2.zero) return;
            animator.SetFloat(LastHorizontal, _moveDirection.x);
            animator.SetFloat(LastVertical, _moveDirection.y);
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            _moveDirection = value.ReadValue<Vector2>().normalized;
        }

        private void OnMovementCancelled(InputAction.CallbackContext value)
        {
            _moveDirection = Vector2.zero;
        }
    }
}

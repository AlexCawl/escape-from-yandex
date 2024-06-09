using System;
using GameMaster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviour
{
    public class TechRoomBehaviour : VisibleOnlyInLightBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private State _tooltipVisibilityState;
        private GameInput _gameInput;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
        }

        protected override void Start()
        {
            base.Start();
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _miniGameState = ServiceLocator.Get.Locate<SceneLoadState>("miniGameState");
            StartCoroutine(CheckVisibility());
        }

        private void Update()
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _tooltipVisibilityState.Activate();
        }
        
        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Interaction.performed += HandleMiniGameActivation;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Interaction.performed -= HandleMiniGameActivation;
        }

        private void HandleMiniGameActivation(InputAction.CallbackContext value)
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _miniGameState.Toggle();
        }
    }
}
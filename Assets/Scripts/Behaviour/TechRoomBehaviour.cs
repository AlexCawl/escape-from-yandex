using GameMaster;
using GameMaster.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviour
{
    public class TechRoomBehaviour : VisibleOnlyInLightBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private BooleanState _tooltipState;
        private GameInput _gameInput;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        protected override void Start()
        {
            base.Start();
            _tooltipState = ServiceLocator.Get.Locate<BooleanState>("tooltipVisibilityState");
            _miniGameState = ServiceLocator.Get.Locate<SceneLoadState>("miniGameState");
            StartCoroutine(CheckVisibility());
        }

        private void Update()
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _tooltipState.Activate();
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
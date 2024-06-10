using GameMaster;
using GameMaster.State;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Behaviour
{
    public class ExitDoorBehaviour : MonoBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private GameInput _gameInput;
        private GameLevelState _gameLevelState;
        private BooleanState _tooltipState;
        private BooleanState _miniGameCompleteState;
        private MusicPlayObserver _clickSoundEffect;
        private bool _isTutorial;

        private void Awake()
        {
            _gameInput = new GameInput();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            _isTutorial = gameObject.scene.name.Contains("Tutorial");
        }

        private void Start()
        {
            _tooltipState = ServiceLocator.Get.Locate<BooleanState>("tooltipVisibilityState");
            _miniGameCompleteState = ServiceLocator.Get.Locate<BooleanState>("miniGameCompleteState");
            _gameLevelState = ServiceLocator.Get.Locate<GameLevelState>();
            _clickSoundEffect = ServiceLocator.Get.Locate<MusicPlayObserver>("clickSound");
        }
    
        private void Update()
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            if (!_miniGameCompleteState.Get) return;
            _tooltipState.Activate();
        }
        
        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Interaction.performed += HandleExitActivation;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Interaction.performed -= HandleExitActivation;
        }

        private void HandleExitActivation(InputAction.CallbackContext value)
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _clickSoundEffect.Observe(this);
            if (!_miniGameCompleteState.Get) return;
            switch (_isTutorial)
            {
                case true:
                    NavigateTutorial();
                    break;
                case false:
                    NavigateLevel();
                    break;
            }
        }

        private void NavigateLevel()
        {
            var nextLevel = _gameLevelState.IsLast() ? "Scenes/Victory" : _gameLevelState.Next();
            SceneManager.LoadSceneAsync(nextLevel, LoadSceneMode.Single);
        }
        
        private static void NavigateTutorial() => SceneManager.LoadSceneAsync("Scenes/Splash", LoadSceneMode.Single);
    }
}

using System.Collections;
using GameMaster;
using GameMaster.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCharacter
{
    public class CharacterHealth : MonoBehaviour
    {
        [Range(100f, 300f)] public float reloadTime;
        [Range(10, 100)] public int reloadIterations;
        [Range(10, 50)] public int healthNumber;

        private GameInput _gameInput;
        private NumberState _playerHealth;
        private ProgressState _healingCooldownState;
        
        private void Awake()
        {
            _gameInput = new GameInput();
            _healingCooldownState = ServiceLocator.Get.Create(new ProgressState(reloadIterations), "healingCooldownState");
        }

        private void Start()
        {
            _playerHealth = ServiceLocator.Get.Locate<NumberState>("playerHealth");
        }

        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Healing.performed += Heal;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Healing.performed -= Heal;
        }

        private void Heal(InputAction.CallbackContext value)
        {
            if (!_healingCooldownState.IsReady()) return;
            _healingCooldownState.Reset();
            StartCoroutine(ScheduleReload());
            _playerHealth.Increase(healthNumber);
        }

        private IEnumerator ScheduleReload()
        {
            while (!_healingCooldownState.IsReady())
            {
                _healingCooldownState.Load();
                yield return new WaitForSeconds(reloadTime / _healingCooldownState.StepCount);
            }
        }
    }
}

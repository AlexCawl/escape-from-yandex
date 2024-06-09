using System;
using System.Collections;
using GameMaster;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCharacter
{
    public class CharacterHealing : MonoBehaviour
    {
        [Range(100f, 300f)] public float reloadTime;
        [Range(10, 100)] public int reloadIterations;
        [Range(10, 50)] public int healthNumber;

        private GameInput _gameInput;
        private ReloadHolder _reloadHealingState;
        private HealthHolder _health;
        
        private void Awake()
        {
            _reloadHealingState = ServiceLocator.Get.Create(new ReloadHolder(reloadIterations), "reloadHealingState");
            _gameInput = new GameInput();
        }

        private void Start()
        {
            _health = ServiceLocator.Get.Locate<HealthHolder>("playerHealth");
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
            if (!_reloadHealingState.CanShoot) return;
            _reloadHealingState.Shoot();
            StartCoroutine(ScheduleReload());
            _health.Increase(healthNumber);
        }

        private IEnumerator ScheduleReload()
        {
            while (!_reloadHealingState.CanShoot)
            {
                _reloadHealingState.Reload();
                yield return new WaitForSeconds(reloadTime / _reloadHealingState.Steps);
            }
        }
    }
}

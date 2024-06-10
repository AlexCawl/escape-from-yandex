using System.Collections;
using GameCharacter.FieldOfView;
using GameMaster;
using GameMaster.State;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCharacter
{
    public class CharacterAttack : MonoBehaviour
    {
        public GameObject flashPrefab;
        [Range(1f, 3f)] public float reloadTime;
        [Range(10, 100)] public int reloadIterations;

        private GameInput _gameInput;
        private Camera _camera;
        private ProgressState _attackCooldownState;
        private MusicPlayObserver _attackSoundEffect;
        private SceneLoadState _pauseState;
        private SceneLoadState _miniGameState;

        private void Awake()
        {
            _gameInput = new GameInput();
            _camera = Camera.main;
            _attackCooldownState =
                ServiceLocator.Get.Create(new ProgressState(reloadIterations), "attackCooldownState");
        }

        private void Start()
        {
            _attackSoundEffect = ServiceLocator.Get.Locate<MusicPlayObserver>("shootSound");
            _pauseState = ServiceLocator.Get.Locate<SceneLoadState>("pauseState");
            _miniGameState = ServiceLocator.Get.Locate<SceneLoadState>("miniGameState");
        }

        private void OnEnable()
        {
            _gameInput.Enable();
            _gameInput.Player.Fire.performed += Shoot;
        }

        private void OnDisable()
        {
            _gameInput.Disable();
            _gameInput.Player.Fire.performed -= Shoot;
        }

        private void Shoot(InputAction.CallbackContext value)
        {
            if (!_attackCooldownState.IsReady()) return;
            if (_pauseState.Get || _miniGameState.Get) return;
            _attackCooldownState.Reset();
            StartCoroutine(ScheduleReload());
            var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
            var character = Utils.ReduceDimension(transform.position);
            var direction = Utils.GetVectorFromAngle(Utils.GetAngleBetweenVectors(character, mouse));
            var bullet = Instantiate(flashPrefab, transform.position, Quaternion.identity);
            bullet.transform.up = Utils.IncreaseDimension(direction, transform.position.z);
            _attackSoundEffect.Observe(this);
        }

        private IEnumerator ScheduleReload()
        {
            while (!_attackCooldownState.IsReady())
            {
                _attackCooldownState.Load();
                yield return new WaitForSeconds(reloadTime / _attackCooldownState.StepCount);
            }
        }
    }
}
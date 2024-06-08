using System.Collections;
using FieldOfView;
using GameCharacter;
using GameMaster;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public GameObject flashPrefab;
    [Range(1f, 3f)] public float reloadTime;
    [Range(10, 100)] public int reloadIterations;

    private GameInput _gameInput;
    private ReloadHolder _reloadState;
    private Camera _camera;

    private void Awake()
    {
        _reloadState = ServiceLocator.Get.Create(new ReloadHolder(reloadIterations), "reloadState");
        _gameInput = new GameInput();
        _camera = Camera.main;
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
        if (!_reloadState.CanShoot) return;
        _reloadState.Shoot();
        StartCoroutine(ScheduleReload());
        var mouse = Utils.ReduceDimension(_camera.ScreenToWorldPoint(Input.mousePosition));
        var character = Utils.ReduceDimension(transform.position);
        var direction = Utils.GetVectorFromAngle(Utils.GetAngleBetweenVectors(character, mouse));
        var bullet = Instantiate(flashPrefab, transform.position, Quaternion.identity);
        bullet.transform.up = Utils.IncreaseDimension(direction, transform.position.z);
    }

    private IEnumerator ScheduleReload()
    {
        while (!_reloadState.CanShoot)
        {
            _reloadState.Reload();
            yield return new WaitForSeconds(reloadTime / _reloadState.Steps);
        }
    }
}

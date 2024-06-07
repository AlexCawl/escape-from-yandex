using UnityEngine;

namespace GameMaster
{
    public class ExitDoorBehaviour : MonoBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private State _tooltipVisibilityState;
        private State _miniGameState;
        private State _exitOpenState;
    
        private void Start()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _miniGameState = ServiceLocator.Get.Locate<State>("miniGamePassedState");
            _exitOpenState = ServiceLocator.Get.Locate<State>("exitOpenState");
        }
    
        private void Update()
        {
            if (!_miniGameState.Get) return;
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _tooltipVisibilityState.Activate();
            if (!Input.GetKeyDown("e")) return;
            _exitOpenState.Activate();
        }
    }
}

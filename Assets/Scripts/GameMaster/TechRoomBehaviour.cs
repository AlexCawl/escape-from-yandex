using UnityEngine;

namespace GameMaster
{
    public class TechRoomBehaviour : MonoBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private State _tooltipVisibilityState;
        private IntentState _miniGameOverlayState;

        private void Start()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _miniGameOverlayState = ServiceLocator.Get.Locate<IntentState>("miniGameOverlayState");
        }

        private void Update()
        {
            if (!(Vector3.Distance(player.position, transform.position) < distance)) return;
            _tooltipVisibilityState.Activate();
            var pressed = Input.GetKeyDown("e");
            if (pressed)
            {
                _miniGameOverlayState.Toggle();
            }
        }
    }
}
using FieldOfView;
using GameMaster;
using UnityEngine;

namespace Behaviour
{
    public class TechRoomBehaviour : VisibleOnlyInLightBehaviour
    {
        [Range(1f, 5f)] public float distance;
        public Transform player;

        private State _tooltipVisibilityState;
        private IntentState _miniGameOverlayState;

        protected override void Start()
        {
            base.Start();
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _miniGameOverlayState = ServiceLocator.Get.Locate<IntentState>("miniGameOverlayState");
            StartCoroutine(CheckVisibility());
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
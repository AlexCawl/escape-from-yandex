using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameMaster;
using UnityEngine;

namespace FieldOfView
{
    public class VisibleOnlyInLightBehaviour : MonoBehaviour
    {
        protected State VisibilityState;
        private Renderer _renderer;

        protected virtual void Start()
        {
            _renderer = GetComponent<Renderer>();
            VisibilityState = ServiceLocator.Get.Locate<State>("visibilityState");
        }

        public void Highlight() => VisibilityState.Activate();

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        protected IEnumerator CheckVisibility()
        {
            while (true)
            {
                _renderer.enabled = VisibilityState.Get;
                VisibilityState.Deactivate();
                yield return null;
            }
        }
    }
}

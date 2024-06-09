using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameMaster;
using GameMaster.State;
using UnityEngine;

namespace Behaviour
{
    public class VisibleOnlyInLightBehaviour : MonoBehaviour
    {
        protected BooleanState VisibilityState;
        private Renderer _renderer;

        protected virtual void Start()
        {
            _renderer = GetComponent<Renderer>();
            VisibilityState = ServiceLocator.Get.Locate<BooleanState>("visibilityState");
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
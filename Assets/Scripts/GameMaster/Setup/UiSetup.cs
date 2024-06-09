using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace GameMaster.Setup
{
    public class UiSetup : MonoBehaviour
    {
        public Image healthBar;
        public Image reloadBar;
        public GameObject tooltipBox;
        public Image miniGameBar;
        public Image healingBar;

        private HealthHolder _playerHealth;
        private State _tooltipVisibilityState;
        private ReloadHolder _reloadState;
        private State _miniGamePassedState;
        private ReloadHolder _healingReloadState;

        private void Awake()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _playerHealth = ServiceLocator.Get.Locate<HealthHolder>("playerHealth");
            _reloadState = ServiceLocator.Get.Locate<ReloadHolder>("reloadState");
            _healingReloadState = ServiceLocator.Get.Locate<ReloadHolder>("reloadHealingState");
            _miniGamePassedState = ServiceLocator.Get.Locate<State>("miniGamePassedState");
        }

        private void Start()
        {
            StartCoroutine(CheckHealth());
            StartCoroutine(CheckTooltipBox());
            StartCoroutine(CheckReloading());
            StartCoroutine(CheckMiniGameState());
            StartCoroutine(CheckHealing());
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckHealth()
        {
            while (true)
            {
                healthBar.fillAmount = _playerHealth.Percent;
                yield return null;
            }
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckTooltipBox()
        {
            while (true)
            {
                tooltipBox.SetActive(_tooltipVisibilityState.Get);
                _tooltipVisibilityState.Deactivate();
                yield return null;
            }
        }
        
        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckReloading()
        {
            while (true)
            {
                reloadBar.fillAmount = _reloadState.Percent;
                yield return null;
            }
        }
        
        private static readonly Color Passed = new(0.663f, 0.762f, 0.984f, 1.000f);
        private static readonly Color NotPassed = new(0f, 0f, 0f, 0.5f);

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckMiniGameState()
        {
            while (true)
            {
                miniGameBar.color = _miniGamePassedState.Get ? Passed : NotPassed;
                yield return null;
            }
        }
        
        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckHealing()
        {
            while (true)
            {
                healingBar.fillAmount = _healingReloadState.Percent;
                yield return null;
            }
        }
    }
}
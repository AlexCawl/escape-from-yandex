using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using GameMaster.State;
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

        private NumberState _playerHealth;
        private BooleanState _tooltipState;
        private ReloadHolder _reloadState;
        private BooleanState _miniGameCompleteState;
        private ReloadHolder _healingReloadState;

        private void Awake()
        {
            _tooltipState = ServiceLocator.Get.Locate<BooleanState>("tooltipVisibilityState");
            _playerHealth = ServiceLocator.Get.Locate<NumberState>("playerHealth");
            _reloadState = ServiceLocator.Get.Locate<ReloadHolder>("reloadState");
            _healingReloadState = ServiceLocator.Get.Locate<ReloadHolder>("reloadHealingState");
            _miniGameCompleteState = ServiceLocator.Get.Locate<BooleanState>("miniGameCompleteState");
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
                tooltipBox.SetActive(_tooltipState.Get);
                _tooltipState.Deactivate();
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
                miniGameBar.color = _miniGameCompleteState.Get ? Passed : NotPassed;
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
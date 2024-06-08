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

        private HealthHolder _playerHealth;
        private State _tooltipVisibilityState;
        private ReloadHolder _reloadState;

        private void Awake()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _playerHealth = ServiceLocator.Get.Locate<HealthHolder>("playerHealth");
            _reloadState = ServiceLocator.Get.Locate<ReloadHolder>("reloadState");
        }

        private void Start()
        {
            StartCoroutine(CheckHealth());
            StartCoroutine(CheckTooltipBox());
            StartCoroutine(CheckReloading());
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
    }
}
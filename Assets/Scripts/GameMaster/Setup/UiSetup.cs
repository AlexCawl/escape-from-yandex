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
        public GameObject tooltipBox;

        private HealthHolder _playerHealth;
        private State _tooltipVisibilityState;

        private void Awake()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _playerHealth = ServiceLocator.Get.Locate<HealthHolder>("playerHealth");
        }

        private void Start()
        {
            StartCoroutine(CheckHealth());
            StartCoroutine(CheckTooltipBox());
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
    }
}
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
        
        private CharacterHealthHolder _playerHealth;
        private State _tooltipVisibilityState;

        private void Start()
        {
            _tooltipVisibilityState = ServiceLocator.Get.Locate<State>("tooltipVisibilityState");
            _playerHealth = CharacterHealthHolder.GetInstance();
            StartCoroutine(CheckHealth());
            StartCoroutine(CheckTooltipBox());
        }

        [SuppressMessage("ReSharper", "IteratorNeverReturns")]
        private IEnumerator CheckHealth()
        {
            while (true)
            {
                var health = (float)_playerHealth.Get / CharacterHealthHolder.GetMax;
                if (health > 1)
                {
                    health = 1;
                }

                if (health < 0)
                {
                    health = 0;
                }
                healthBar.fillAmount = health;
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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using GameCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace GameMaster
{
    public class UiSetup : MonoBehaviour
    {
        public Image healthBar;
        public GameObject tooltipBox;
        private CharacterHealthHolder _playerHealth;

        private void Awake()
        {
            _playerHealth = CharacterHealthHolder.GetInstance();
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
                tooltipBox.SetActive(TooltipMarker.Controller.Get);
                yield return null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameMaster
{
    public class NumbersChallengeManager : MonoBehaviour
    {
        public List<Button> buttons;
        private List<Button> _shuffledButtons;
        private int _counter;

        private void Start()
        {
            Restart();
        }

        private void Restart()
        {
            _counter = 0;
            _shuffledButtons = buttons.OrderBy(_ => Random.Range(1, 100)).ToList();
            for (var i = 0; i < _shuffledButtons.Count; i++)
            {
                var button = _shuffledButtons[i];
                button.GetComponentInChildren<Text>().text = (i + 1).ToString();
                button.interactable = true;
                button.image.color = Color.white;
            }
        }

        public void ButtonPressAction(Button button)
        {
            var buttonNumber = int.Parse(button.GetComponentInChildren<Text>().text);
            if (buttonNumber == _counter + 1)
            {
                _counter += 1;
                button.interactable = false;
                button.image.color = Color.green;
                if (_counter == buttons.Count)
                {
                    StartCoroutine(AssumeResult(true));
                }
            }
            else
            {
                StartCoroutine(AssumeResult(false));
            }
        }

        private IEnumerator AssumeResult(bool win)
        {
            if (!win)
            {
                buttons.ForEach(button =>
                {
                    button.image.color = Color.red;
                    button.interactable = false;
                });
            }

            yield return new WaitForSeconds(2);
            Restart();
        }
    }
}
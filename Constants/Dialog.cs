using System;
using TMPro;
using UnityEngine.UI;

namespace Constants
{
    [Serializable]
    public class Dialog
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI message;
        public Button cancelButton;
        public Button continueButton;

        public Dialog SetTitle(string text)
        {
            title.text = text;

            return this;
        }

        public Dialog SetMessage(string text)
        {
            message.text = text;

            return this;
        }

        public Dialog ModifyButton(string buttonName, string newName, bool disable = false)
        {
            switch (buttonName)
            {
                case nameof(cancelButton):
                    cancelButton.interactable = disable;
                    break;
                case nameof(continueButton):
                    continueButton.interactable = disable;
                    break;
            }
            
            return this;
        }

        public void RemoveAllListenersFromButtons()
        {
            cancelButton.onClick.RemoveAllListeners();
            continueButton.onClick.RemoveAllListeners();
        }
    }
}
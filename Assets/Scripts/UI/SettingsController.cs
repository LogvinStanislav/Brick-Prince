using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI
{

    public class SettingsController : MonoBehaviour
    {
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private MainMenuController mainMenuController;

        void OnEnable()
        {
            if (volumeSlider != null)
            {
                volumeSlider.value = AudioListener.volume;
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }
        }

        void OnDisable()
        {
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            }
        }

        private void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
        }

        public void OnBackPressed()
        {
            if (mainMenuController != null)
            {
                mainMenuController.ShowMainPanel();
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI
{
    /// <summary>
    /// Минимальная панель настроек для MVP: общая громкость и возврат в меню.
    /// Громкость пока не сохраняется между запусками игры — это осознанно
    /// оставлено для будущей доработки (PlayerPrefs или система сохранений),
    /// чтобы не плодить недоделанный код сейчас.
    /// </summary>
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

using UnityEngine;

namespace Platformer.UI
{
    /// <summary>
    /// Описание одного уровня для экрана выбора уровня.
    /// sceneName должно точно совпадать с именем .unity файла и быть
    /// добавлено в Build Settings, иначе загрузка сцены не сработает.
    /// </summary>
    [System.Serializable]
    public class LevelInfo
    {
        [Tooltip("Название, которое увидит игрок на кнопке")]
        public string displayName = "Level";

        [Tooltip("Точное имя сцены (как файл .unity), которое будет загружено")]
        public string sceneName = "Level01";

        [Tooltip("Если сцена ещё не создана/не готова — кнопка будет показана заблокированной")]
        public bool isAvailable = true;
    }
}

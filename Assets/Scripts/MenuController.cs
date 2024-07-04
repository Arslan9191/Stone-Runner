using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private TextMeshProUGUI gameTitle;

    [SerializeField]
    private Button playDustButton;

    [SerializeField]
    private TextMeshProUGUI chooseLevelText;

    [SerializeField]
    private Button playForestButton;

    [SerializeField]
    private Button playWinterButton;

    [SerializeField]
    private TextMeshProUGUI bestRecordText;

    [SerializeField]
    private Button closeButton;

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private AudioClip clickSound; // Аудиоклип для звука клика

    private AudioSource audioSource;
    private AudioManager audioManager;

    void Start()
    {
        // Получить компонент AudioSource
        audioSource = GetComponent<AudioSource>();

        // Привязка событий кнопок
        playButton.onClick.AddListener(() => { PlayClickSound(); OnPlayButtonClick(); });
        settingsButton.onClick.AddListener(() => { PlayClickSound(); OnSettingsButtonClick(); });
        closeButton.onClick.AddListener(() => { PlayClickSound(); OnCloseButtonClick(); });

        // Привязка событий изменения настроек
        volumeSlider.onValueChanged.AddListener(delegate { PlayClickSound(); OnVolumeSliderChanged(volumeSlider.value); });

        // Привязка событий кнопок выбора уровня
        playDustButton.onClick.AddListener(() => { PlayClickSound(); LoadScene("Dust"); });
        playForestButton.onClick.AddListener(() => { PlayClickSound(); LoadScene("Forest"); });
        playWinterButton.onClick.AddListener(() => { PlayClickSound(); LoadScene("Winter"); });

        // Загрузка сохраненных настроек
        LoadSettings();

        // Скрытие элементов, которые должны быть скрыты по умолчанию
        SetSecondaryMenuVisibility(false);
        SetSettingsMenuVisibility(false);

        // Обновление текста лучшего результата
        UpdateBestRecordText();

        // Получение экземпляра AudioManager
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("VolumeLevel", 1.0f);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    private void OnPlayButtonClick()
    {
        // Скрыть элементы главного меню
        playButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        gameTitle.gameObject.SetActive(false);

        // Показать элементы вторичного меню
        SetSecondaryMenuVisibility(true);
    }

    private void OnSettingsButtonClick()
    {
        // Скрыть элементы главного меню
        settingsButton.gameObject.SetActive(false);
        gameTitle.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);

        // Показать элементы настроек
        SetSettingsMenuVisibility(true);
    }

    private void OnCloseButtonClick()
    {
        // Скрыть элементы вторичного меню и меню настроек
        SetSecondaryMenuVisibility(false);
        SetSettingsMenuVisibility(false);

        // Показать элементы главного меню
        playButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        gameTitle.gameObject.SetActive(true);
    }

    private void SetSecondaryMenuVisibility(bool isVisible)
    {
        playDustButton.gameObject.SetActive(isVisible);
        chooseLevelText.gameObject.SetActive(isVisible);
        playForestButton.gameObject.SetActive(isVisible);
        playWinterButton.gameObject.SetActive(isVisible);
        bestRecordText.gameObject.SetActive(isVisible);
        closeButton.gameObject.SetActive(isVisible);
    }

    private void SetSettingsMenuVisibility(bool isVisible)
    {
        volumeSlider.gameObject.SetActive(isVisible);
        closeButton.gameObject.SetActive(isVisible);
    }

    private void OnVibrationToggleChanged(bool isOn)
    {
        // Сохранение состояния вибрации в PlayerPrefs
        PlayerPrefs.SetInt("VibrationEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnVolumeSliderChanged(float volume)
    {
        // Установка громкости через AudioManager
        if (audioManager != null)
        {
            audioManager.SetVolume(volume);
        }
    }

    private void LoadSettings()
    {
        // Загрузка уровня громкости из PlayerPrefs
        float volume = PlayerPrefs.GetFloat("VolumeLevel", 1.0f);
        volumeSlider.value = volume;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateBestRecordText()
    {
        int bestRecord = PlayerPrefs.GetInt("BestRecord", 0);
        bestRecordText.text = "Best Record: " + bestRecord.ToString();
    }
}

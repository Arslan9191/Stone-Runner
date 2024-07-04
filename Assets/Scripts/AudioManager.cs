using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip clickSound; // Аудиоклип для звука клика

    void Awake()
    {
        // Если экземпляра еще не существует, назначьте его и сохраните на переход между сценами
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Если экземпляр уже существует, уничтожьте новый экземпляр
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Получаем компонент AudioSource
        audioSource = GetComponent<AudioSource>();

        // Применяем сохраненный уровень громкости
        ApplySavedVolume();
    }

    // Метод для проигрывания звука
    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    // Метод для остановки звука
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    // Метод для проигрывания звука клика
    public void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Метод для установки уровня громкости
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("VolumeLevel", volume);
        PlayerPrefs.Save();
    }

    // Метод для применения сохраненного уровня громкости
    private void ApplySavedVolume()
    {
        float volume = PlayerPrefs.GetFloat("VolumeLevel", 1.0f);
        AudioListener.volume = volume;
    }
}

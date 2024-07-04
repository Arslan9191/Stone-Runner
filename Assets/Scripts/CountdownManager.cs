using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Элемент текста для отображения обратного отсчета

    private void Start()
    {
        // Убедитесь, что текст обратного отсчета изначально скрыт или пуст
        countdownText.text = "";
        // Запустить корутину обратного отсчета
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        // Установить длительность обратного отсчета
        int countdownDuration = 3;

        // Показать текст обратного отсчета
        countdownText.text = countdownDuration.ToString();

        // Остановить время в игре
        Time.timeScale = 0;

        // Ждать 1 секунду (реальное время) и обновлять текст обратного отсчета
        while (countdownDuration > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            countdownDuration--;
            countdownText.text = countdownDuration.ToString();
        }

        // Обратный отсчёт завершён, можно добавить любые дополнительные действия здесь
        countdownText.text = "Go!";
        yield return new WaitForSecondsRealtime(1);
        countdownText.text = ""; // Скрыть текст обратного отсчета после завершения

        // Возобновить время в игре
        Time.timeScale = 1;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CarHandler : MonoBehaviour
{
    // Переменные для сериализации компонентов в инспекторе Unity
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform gameModel;
    [SerializeField] private ExplodeHandler explodeHandler;
    [SerializeField] private Button repeatButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI bestRecordText;
    [SerializeField] private float maxSteerVelocity = 2;
    [SerializeField] private float maxForwardVelocity = 30;
    [SerializeField] private float accelerationMultiplier = 3;
    [SerializeField] private float steeringMultiplier = 5;

    // Публичные кнопки для управления
    [SerializeField] public Button rightMoveButton;
    [SerializeField] public Button leftMoveButton;

    // Внутренние переменные
    private bool isExploded = false;
    private bool gameStarted = false;
    private float horizontalInput = 0f;
    private int score;
    private float scoreTimer;
    private Vector2 input = Vector2.zero;

    void Start()
    {
        // Установка начальной скорости
        rb.velocity = Vector3.forward * maxForwardVelocity * 0.5f;
        // Отключение кнопок и назначение функций нажатий
        repeatButton.gameObject.SetActive(false);
        repeatButton.onClick.AddListener(ReloadScene);
        closeButton.gameObject.SetActive(false);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        // Запуск корутины для отложенного старта игры
        StartCoroutine(DelayedStart());
        // Инициализация очков и таймера
        score = 0;
        scoreTimer = 0f;
        UpdateScoreText();
        // Скрытие текста с лучшим результатом
        bestRecordText.gameObject.SetActive(false);
    }

    // Корутина для отложенного старта игры
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1.0f);
        gameStarted = true;
    }

    void Update()
    {
        // Если машина взорвалась, выходим из метода
        if (isExploded) return;

        // Поворот модели в зависимости от скорости
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);

        if (gameStarted)
        {
            // Обновление таймера и очков
            scoreTimer += Time.deltaTime;
            if (scoreTimer >= 1f)
            {
                score++;
                scoreTimer = 0f;
                UpdateScoreText();
            }
        }

        // Установка ввода управления
        SetInput(new Vector2(horizontalInput, 1));
    }

    private void FixedUpdate()
    {
        if (isExploded)
        {
            // Уменьшение скорости и остановка машины при взрыве
            rb.drag = rb.velocity.z * 0.1f;
            rb.drag = Mathf.Clamp(rb.drag, 1.5f, 10);
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * 0.5f));
            return;
        }

        // Ускорение и управление
        Accelerate();
        Steer();

        // Остановка машины, если скорость падает до нуля
        if (rb.velocity.z <= 0) rb.velocity = Vector3.zero;
    }

    // Метод для ускорения машины
    void Accelerate()
    {
        rb.drag = 0;
        if (rb.velocity.z >= maxForwardVelocity) return;
        rb.AddForce(rb.transform.forward * accelerationMultiplier);
    }

    // Метод для управления поворотами
    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            float speedBaseSteerLimit = rb.velocity.z / 5.0f;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);
            rb.AddForce(rb.transform.right * steeringMultiplier * input.x * speedBaseSteerLimit);
            float normalizedX = rb.velocity.x / maxSteerVelocity;
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);
            rb.velocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.velocity.z);
        }
        else
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, rb.velocity.z), Time.fixedDeltaTime * 3);
        }
    }

    // Установка входных данных
    public void SetInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
    }

    // Установка горизонтального ввода
    public void SetHorizontalInput(float input)
    {
        horizontalInput = input;
    }

    // Установка максимальной скорости
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }

    // Корутина для замедления времени
    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale <= 1.0f)
        {
            Time.timeScale += Time.deltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;
    }

    // Обработчик столкновений
    private void OnCollisionEnter(Collision collision)
    {
        if (!gameStarted || isExploded) return;

        if (collision.collider.CompareTag("Obstacle"))
        {
            Vector3 velocity = rb.velocity;
            explodeHandler.Explode(velocity * 45);

            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

            isExploded = true;
            repeatButton.gameObject.SetActive(true);
            closeButton.gameObject.SetActive(true);
            rightMoveButton.gameObject.SetActive(false);
            leftMoveButton.gameObject.SetActive(false);
            SaveBestRecord();
            StartCoroutine(SlowDownTimeCO());
            bestRecordText.gameObject.SetActive(true);
        }
    }

    // Сохранение лучшего результата
    void SaveBestRecord()
    {
        int bestRecord = PlayerPrefs.GetInt("BestRecord", 0);
        if (score > bestRecord)
        {
            PlayerPrefs.SetInt("BestRecord", score);
            PlayerPrefs.Save();
        }
        UpdateBestRecordText();
    }

    // Обновление текста с очками
    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    // Обновление текста с лучшим результатом
    void UpdateBestRecordText()
    {
        int bestRecord = PlayerPrefs.GetInt("BestRecord", 0);
        bestRecordText.text = "Best: " + bestRecord.ToString();
    }

    // Обработчик нажатия кнопки закрытия
    private void OnCloseButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }

    // Перезагрузка сцены
    public void ReloadScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

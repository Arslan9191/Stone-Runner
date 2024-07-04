using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessSectionHandler : MonoBehaviour
{
    Transform playerCarTransform; // Трансформ машины игрока

    // Метод Start вызывается перед первым обновлением кадра
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform; // Получение трансформа машины игрока
    }

    // Метод Update вызывается один раз за кадр
    void Update()
    {
        float distanceToPlayer = transform.position.z - playerCarTransform.position.z; // Расстояние до игрока

        float lerpPercentage = 1.0f - ((distanceToPlayer - 100) / 150.0f); // Вычисление процента интерполяции
        lerpPercentage = Mathf.Clamp01(lerpPercentage); // Ограничение значения от 0 до 1

        // Интерполяция позиции секции
        transform.position = Vector3.Lerp(new Vector3(transform.position.x, -10, transform.position.z), new Vector3(transform.position.x, 0, transform.position.z), lerpPercentage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionsPrefabs; // Префабы секций

    GameObject[] sectionsPool = new GameObject[15]; // Пул секций

    GameObject[] sections = new GameObject[5]; // Активные секции

    Transform playerCarTransform; // Трансформ машины игрока

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f); // Ожидание 100 миллисекунд

    const float sectionLength = 25; // Длина секции

    // Метод Start вызывается перед первым обновлением кадра
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform; // Получение трансформа машины игрока

        int prefabIndex = 0;

        // Создаем пул для наших бесконечных секций
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);

            prefabIndex++;

            // Сбрасываем индекс префаба, если закончились префабы
            if (prefabIndex > sectionsPrefabs.Length - 1)
                prefabIndex = 0;
        }

        // Добавляем первые секции на дорогу
        for (int i = 0; i < sections.Length; i++)
        {
            // Получаем случайную секцию
            GameObject randomSection = GetRandomSectionFromPool();

            // Перемещаем её на нужную позицию и активируем
            randomSection.transform.position = new Vector3(sectionsPool[i].transform.position.x, -10, i * sectionLength);
            randomSection.SetActive(true);

            // Устанавливаем секцию в массив
            sections[i] = randomSection;
        }

        StartCoroutine(UpdateLessOftenCO());
    }

    // Короутина, которая обновляет позиции секций с определенной задержкой
    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }

    // Обновление позиций секций
    void UpdateSectionPositions()
    {
        for (int i = 0; i < sections.Length; i++) 
        {
            // Проверяем, не слишком ли секция отстала от игрока
            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                // Сохраняем позицию секции и отключаем её
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                // Получаем новую секцию, включаем её и перемещаем вперёд
                sections[i] = GetRandomSectionFromPool();
                sections[i].transform.position = new Vector3(lastSectionPosition.x, -10, lastSectionPosition.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
            }
        }
    }

    // Получение случайной секции из пула
    GameObject GetRandomSectionFromPool()
    {
        int randomIndex = Random.Range(0, sectionsPool.Length); // Случайный индекс

        bool isNewSectionFound = false;

        while(!isNewSectionFound)
        {
            // Проверяем, не активна ли секция. Если нет, значит мы нашли новую секцию
            if (!sectionsPool[randomIndex].activeInHierarchy)
                isNewSectionFound = true;
            else
            {
                // Если секция была активна, пытаемся найти другую, увеличивая индекс
                randomIndex++;

                // Если дошли до конца массива, начинаем с начала
                if (randomIndex > sectionsPool.Length - 1)
                    randomIndex = 0;
            }
        }

        return sectionsPool[randomIndex];
    }
}

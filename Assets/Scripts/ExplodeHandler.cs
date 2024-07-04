using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeHandler : MonoBehaviour
{
    [SerializeField]
    GameObject orignalObject; // Оригинальный объект

    [SerializeField]
    GameObject model; // Модель объекта

    Rigidbody[] rigidbodies; // Массив Rigidbody компонентов

    // Метод Awake вызывается при инициализации скрипта
    private void Awake()
    {
        rigidbodies = model.GetComponentsInChildren<Rigidbody>(true); // Получаем все Rigidbody компоненты в дочерних объектах модели
    }

    // Метод Start вызывается перед первым обновлением кадра
    void Start()
    {
        //Explode(Vector3.forward); // Пример вызова метода Explode с внешней силой направленной вперёд
    }

    // Метод для выполнения взрыва с применением внешней силы
    public void Explode(Vector3 externalForce)
    {
        orignalObject.SetActive(false); // Отключаем оригинальный объект
        foreach (Rigidbody rb in rigidbodies)
        {
            rb.transform.parent = null; // Отсоединяем Rigidbody от родителя

            rb.GetComponent<MeshCollider>().enabled = true; // Включаем коллайдер

            rb.gameObject.SetActive(true); // Активируем объект
            rb.isKinematic = false; // Отключаем кинематику
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Включаем интерполяцию для плавного движения
            rb.AddForce(Vector3.up * 200 + externalForce, ForceMode.Force); // Добавляем силу для взрыва
            rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse); // Добавляем случайный крутящий момент
        }
    }
}

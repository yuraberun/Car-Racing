using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Collection", menuName = "Car Collection")]
public class CarsCollection : ScriptableObject
{
    [System.Serializable]
    public class Car
    {
        public CarName carName;

        public GameObject prefab;
    }

    [SerializeField] private List<Car> _cars = new List<Car>();

    public GameObject GetCarPrefab(CarName carName)
    {
        var car = _cars.Find(car => car.carName == carName);

        if (car == null)
            Debug.LogError("the collection does not contain such a car");

        return car.prefab;
    }

    public GameObject GetRandomCarPrefab()
    {
        var car = _cars[Random.Range(0, _cars.Count)];

        return car.prefab;
    }

    public List<Car> GetCollection()
    {
        return _cars;
    }
}

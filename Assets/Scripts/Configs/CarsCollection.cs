using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Collection", menuName = "Car Collection")]
public class CarsCollection : ScriptableObject
{
    [System.Serializable]
    public class CarInfo
    {
        public CarName carName;

        public EnemyAiRules enemyAiRules;

        public GameObject prefab;
    }

    [SerializeField] private List<CarInfo> _cars = new List<CarInfo>();

    public CarInfo GetCarInfoPrefab(CarName carName)
    {
        var carInfo = _cars.Find(carInfo => carInfo.carName == carName);

        if (carInfo == null)
            Debug.LogError("the collection does not contain such a car");

        return carInfo;
    }

    public CarInfo GetRandomCarInfoPrefab()
    {
        var carInfo = _cars[Random.Range(0, _cars.Count)];

        return carInfo;
    }

    public List<CarInfo> GetCollection()
    {
        return _cars;
    }
}

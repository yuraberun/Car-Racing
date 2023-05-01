using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GarageWindow : Window
{
    [Header("Car Info")]
    [SerializeField] private TextMeshProUGUI _massLabel;
    [SerializeField] private TextMeshProUGUI _motorPowerLabel;
    [SerializeField] private TextMeshProUGUI _nitroPowerLabel;
    [SerializeField] private TextMeshProUGUI _amountOfNitroLabel;
    [SerializeField] private TextMeshProUGUI _rotateSpeedLabel;

    [Header("Car")]
    [SerializeField] private CarsCollection _carsCollection;

    [SerializeField] private Transform _carContainer;

    [SerializeField] private Vector3 _carPosition;
    [SerializeField] private Vector3 _carRotation;

    private List<CarBase> _cars = new List<CarBase>();

    private CarBase _currCar;

    private int _currPlayerCarIndex;

    public void Init()
    {
        var currPlayerCarName = (CarName)PlayerPrefs.GetInt("PlayerCar");
        var carsInfos = _carsCollection.GetCollection();

        foreach (var carInfo in carsInfos)
        {
            if (currPlayerCarName == carInfo.carName)
                _currPlayerCarIndex = carsInfos.IndexOf(carInfo);

            var car = Instantiate(carInfo.prefab, _carContainer).GetComponent<CarBase>();
            car.gameObject.SetActive(false);
            _cars.Add(car);
        }

        ShowCurrentCar();
    }

    public void Open()
    {
        base.OpenBase();
    }

    public void Close()
    {
        base.CloseBase();
    }

    private void ShowCurrentCar()
    {
        foreach (var car in _cars)
        {
            car.transform.position = Vector3.zero;
            car.gameObject.SetActive(false);
        }

        _currCar = _cars[_currPlayerCarIndex];
        _currCar.gameObject.SetActive(true);
        _currCar.transform.position = _carPosition;
        _currCar.transform.rotation = Quaternion.Euler(_carRotation);

        PlayerPrefs.SetInt("PlayerCar", (int)_currCar.CarName);

        ResetLabels();
    }

    private void ResetLabels()
    {
        _massLabel.text = _currCar.Mass + "";
        _motorPowerLabel.text = _currCar.MotorPower + "";
        _nitroPowerLabel.text = _currCar.NitroPower + "";
        _amountOfNitroLabel.text = _currCar.MaxAmountOfNitro + "";
        _rotateSpeedLabel.text = _currCar.RotateSpeed + "";
    }

    public void OnBackButtonClick()
    {
        base.OnButtonClick();

        MenuHUD.Instance.PlayHideGarageAnimation();
    }

    public void OnChangeCarButtonClick(int direction)
    {
        _currPlayerCarIndex += direction;

        if (_currPlayerCarIndex > _cars.Count - 1)
            _currPlayerCarIndex = 0;

        if (_currPlayerCarIndex < 0)
            _currPlayerCarIndex = _cars.Count - 1;

        ShowCurrentCar();

        base.OnButtonClick();
    }
}

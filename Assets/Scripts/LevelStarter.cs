using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CarBase _playerCar;

    [SerializeField] private LevelCamera _levelCamera;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        StartLevel();
    }

    public void Init()
    {
        _playerCar.Init();
        _levelCamera.Init(_playerCar.transform);
    }

    public void StartLevel()
    {
        _playerCar.Activate();
        _levelCamera.Activate();
    }
}

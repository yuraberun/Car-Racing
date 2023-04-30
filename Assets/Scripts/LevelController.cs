using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CarBase _playerCar;
    [SerializeField] private CarBase _enemyCar;

    [SerializeField] private LevelCamera _levelCamera;

    private Player _player;

    private void Start()
    {
        Init();
        StartLevel();
    }

    public void Init()
    {
        _playerCar.Init();
        _player = gameObject.AddComponent<Player>();
        _player.Init(_playerCar);

        _enemyCar?.Init();

        _levelCamera.Init(_playerCar.transform);
    }

    public void StartLevel()
    {
        _player.UnblockInput();
        _playerCar.StartAutoMove();
        _enemyCar?.StartAutoMove();

        _levelCamera.Activate();
    }

    public void EndLevel()
    {

    }
}

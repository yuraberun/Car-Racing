using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelController : SingletonComponent<LevelController>
{
    [Header("Configs")]
    [SerializeField] private CarsCollection _carsCollection;

    [Header("Components")]
    [SerializeField] private LevelCamera _levelCamera;

    [SerializeField] private Transform _playerCarSpawnTransform;
    [SerializeField] private Transform _enemyCarSpawnTransform;

    [Header("Settings")]
    [SerializeField] private float _showResultsWindowDelay;

    private List<FinishedCarInfo> _finishedCarInfos = new List<FinishedCarInfo>();

    public CarBase PlayerCar { get; private set; }

    public CarBase EnemyCar { get; private set; }

    public Player Player { get; private set; }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        LevelHUD.Instance.PlayCountdownAnimation(StartLevel);
    }

    public void Init()
    {
        Time.timeScale = 1f;

        var playerCarPrefab = _carsCollection.GetRandomCarPrefab();
        PlayerCar = Instantiate(playerCarPrefab, _playerCarSpawnTransform.position, Quaternion.identity).GetComponent<CarBase>();
        PlayerCar.Init(true);
        Player = gameObject.AddComponent<Player>();
        Player.Init(PlayerCar);

        var enemyCarPrefab = _carsCollection.GetRandomCarPrefab();
        EnemyCar = Instantiate(enemyCarPrefab, _enemyCarSpawnTransform.position, Quaternion.identity).GetComponent<CarBase>();
        EnemyCar.Init(false);

        _finishedCarInfos.Clear();
        LevelHUD.Instance.Init();
        _levelCamera.Init(PlayerCar.transform);
        _levelCamera.Activate();
    }

    public void StartLevel()
    {
        Player.UnblockInput();

        PlayerCar.Activate();
        EnemyCar.Activate();
    }

    public void EndLevel()
    {
        Player.BlockInput();

        _levelCamera.Deactivate();
    }

    public void OnAnyCarFinish(CarName carName, int completeTime, bool isPlayer)
    {
        var position = _finishedCarInfos.Count + 1;

        _finishedCarInfos.Add(new FinishedCarInfo(carName, position, completeTime, isPlayer));

        if (isPlayer || _finishedCarInfos.Count == 2)
        {   
            EndLevel();
            Invoke(nameof(OpenResultsWindow), _showResultsWindowDelay);
        }
    }

    private void OpenResultsWindow()
    {
        LevelHUD.Instance.levelResultsWindow.Open(_finishedCarInfos);
    }

    public void RestartLevel()
    {
        var currSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(currSceneName);
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

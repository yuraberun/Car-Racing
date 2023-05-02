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

    [SerializeField] private PathCreation.PathCreator _playerRoad;
    [SerializeField] private PathCreation.PathCreator _enemyRoad;

    [Header("Settings")]
    [SerializeField] private float _showResultsWindowDelay;

    private List<FinishCarInfo> _finishCarInfos = new List<FinishCarInfo>();

    public Car PlayerCar { get; private set; }

    public Car EnemyCar { get; private set; }

    public Player Player { get; private set; }

    public EnemyAI EnemyAI { get; private set; }

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

        var playerCarName = (CarName)PlayerPrefs.GetInt("PlayerCar");
        var playerCarInfo = _carsCollection.GetCarInfoPrefab(playerCarName);
        PlayerCar = Instantiate(playerCarInfo.prefab, _playerCarSpawnTransform.position, Quaternion.identity).GetComponent<Car>();
        PlayerCar.Init(true);
        Player = gameObject.AddComponent<Player>();
        Player.Init(PlayerCar);

        var enemyCarInfo = _carsCollection.GetRandomCarInfoPrefab();
        EnemyCar = Instantiate(enemyCarInfo.prefab, _enemyCarSpawnTransform.position, Quaternion.identity).GetComponent<Car>();
        EnemyCar.Init(false);
        EnemyAI = gameObject.AddComponent<EnemyAI>();
        EnemyAI.Init(EnemyCar, _enemyRoad.path, enemyCarInfo.enemyAiRules, PlayerCar.transform);

        LevelHUD.Instance.Init();
        _finishCarInfos.Clear();
        _levelCamera.Init(PlayerCar.transform);
        _levelCamera.Activate();
    }

    public void StartLevel()
    {
        Player.UnblockInput();
        PlayerCar.Activate();
        
        EnemyAI.Activate();
        EnemyCar.Activate();
    }

    public void EndLevel()
    {
        Player.BlockInput();
        EnemyAI.Deactivate();

        _levelCamera.Deactivate();
    }

    public void OnAnyCarFinish(CarName carName, int completeTime, bool isPlayer)
    {
        var position = _finishCarInfos.Count + 1;

        _finishCarInfos.Add(new FinishCarInfo(carName, position, completeTime, isPlayer));

        if (isPlayer || _finishCarInfos.Count == 2)
        {   
            EndLevel();
            Invoke(nameof(OpenResultsWindow), _showResultsWindowDelay);
        }
    }

    private void OpenResultsWindow()
    {
        LevelHUD.Instance.levelResultsWindow.Open(_finishCarInfos);
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

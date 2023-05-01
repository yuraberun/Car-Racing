using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelController : SingletonComponent<LevelController>
{
    [Header("Components")]
    [SerializeField] private LevelCamera _levelCamera;

    public CarBase playerCar;
    public CarBase enemyCar;

    [Header("Settings")]
    [SerializeField] private float _showResultsWindowDelay;

    private List<FinishedCarInfo> _finishedCarInfos = new List<FinishedCarInfo>();

    private Player _player;

    private void Start()
    {
        Init();

        LevelHUD.Instance.PlayCountdownAnimation(StartLevel);
    }

    public void Init()
    {
        Time.timeScale = 1f;

        playerCar.Init();
        _player = gameObject.AddComponent<Player>();
        _player.Init(playerCar);

        enemyCar?.Init();

        _finishedCarInfos.Clear();
        LevelHUD.Instance.Init();
        _levelCamera.Init(playerCar.transform);
        _levelCamera.Activate();
    }

    public void StartLevel()
    {
        _player.UnblockInput();
        playerCar.Activate();
        enemyCar?.Activate();
    }

    public void EndLevel()
    {
        _player.BlockInput();

        _levelCamera.Deactivate();

        playerCar.Deactivate();
        enemyCar?.Deactivate();
    }

    public void OnAnyCarFinish(bool isPlayer, int completeTime)
    {
        var position = _finishedCarInfos.Count + 1;

        _finishedCarInfos.Add(new FinishedCarInfo(position, completeTime, isPlayer));

        if (_finishedCarInfos.Count == 1)
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

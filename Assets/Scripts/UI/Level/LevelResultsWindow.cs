using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class LevelResultsWindow : Window
{
    [Header("Components")]
    [SerializeField] private GameObject _completionTimeDifference;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI _positionLabel;
    [SerializeField] private TextMeshProUGUI _completionTimeLabel;
    [SerializeField] private TextMeshProUGUI _completionTimeDifferenceLabel;

    public void Open(List<FinishedCarInfo> finishedCarInfos)
    {
        var playerInfo = finishedCarInfos.Find(info => info.isPlayer);

        _positionLabel.text = playerInfo.position + "";
        _completionTimeLabel.text = SecondsToString(playerInfo.completeTime);

        if (playerInfo.position != 1)
        {
            var firstPositionCar = finishedCarInfos.Find(info => info.position == 1);
            var timeDifference = SecondsToString(playerInfo.completeTime - firstPositionCar.completeTime);

            _completionTimeDifference.SetActive(true);
            _completionTimeDifferenceLabel.text = timeDifference;
        }

        else
        {
            _completionTimeDifference.SetActive(false);
        }

        Time.timeScale = 0f; 
        
        base.OpenBase();
    }
    private string SecondsToString(float seconds)
    {
        var m = Mathf.FloorToInt(seconds / 60f);
        var s = seconds - m * 60;

        var strTime = "";

        if (m > 0)
            strTime += m + "m ";

        if (s > 0)
            strTime += s + "s";

        return strTime; 
    }

    public void OnRestartButtonClick()
    {
        base.OnButtonClick();

        LevelController.Instance.RestartLevel();
    }

    public void OnExitButtonClick()
    {
        base.OnButtonClick();

        LevelController.Instance.ExitToMenu();
    }
}

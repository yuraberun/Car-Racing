using UnityEngine;

public class PauseWindow : Window
{
    public void Open()
    {
        Time.timeScale = 0f;

        base.OpenBase();
    }

    public void Close()
    {
        base.CloseBase();

        Time.timeScale = 1f;
    }

    public void OnContinueButtonClick()
    {
        base.OnButtonClick();

        Close();
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

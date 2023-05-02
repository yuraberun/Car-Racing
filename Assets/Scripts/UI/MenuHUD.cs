using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuHUD : SingletonComponent<MenuHUD>
{
    [Header("Windows")]
    public GarageWindow garageWindow;

    [Header("Components")]
    [SerializeField] private Animator _animator;

    [SerializeField] private MenuCamera _menuCamera;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (!PlayerPrefs.HasKey("PlayerCar"))
            PlayerPrefs.SetInt("PlayerCar", (int)CarName.Car1);

        Time.timeScale = 1f;

        garageWindow.Init();
    }

    public void OpenGarageWindow()
    {
        garageWindow.Open();
    }

    public void HideGarageWindow()
    {
        garageWindow.Close();
    }

    public void PlayHideGarageAnimation()
    {
        _animator.SetTrigger("HideGarage");
        _menuCamera.MoveToDefaultPosition();
    }

    private void OnButtonClick()
    {
        
    }

    public void OnPlayButtonClick()
    {
        OnButtonClick();

        SceneManager.LoadScene("1");
    }

    public void OnGarageButtonClick()
    {
        OnButtonClick();

        _animator.SetTrigger("OpenGarage");
        _menuCamera.MoveToGaragePosition();
    }

    public void OnSettingsButtonClick()
    {
        OnButtonClick();
    }

    public void OnExitButtonClick()
    {
        OnButtonClick();

        #if !UNITY_EDITOR
        Application.Quit();
        #endif
    }
}

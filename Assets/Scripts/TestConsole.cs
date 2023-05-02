using System.Collections;
using UnityEngine;
using TMPro; 

public class TestConsole : SingletonComponent<TestConsole>
{
    [Header("Info labels")]
    [SerializeField] private TextMeshProUGUI _fpsLabel;
    [SerializeField] private TextMeshProUGUI _versionLabel;

    [Header("Another")]
    [SerializeField] private Canvas _canvas;

    [SerializeField] private Transform _activeContainer;
    [SerializeField] private Transform _unActiveContaner;

    private Coroutine _calculateFpsCoroutine;

    public int FPS { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _versionLabel.text = "v" + Application.version;

        _calculateFpsCoroutine = StartCoroutine(CalculateFPS());

        if(!PlayerPrefs.HasKey("IsConsoleActive"))
            PlayerPrefs.SetInt("IsConsoleActive", 1);

        bool isActive = PlayerPrefs.GetInt("IsConsoleActive") == 1;

        _activeContainer.gameObject.SetActive(isActive);
        _unActiveContaner.gameObject.SetActive(!isActive);
    }

    public void SetActive(bool value)
    {
        PlayerPrefs.SetInt("IsConsoleActive", value ? 1 : 0);

        _activeContainer.gameObject.SetActive(value);
        _unActiveContaner.gameObject.SetActive(!value);
    }

    private IEnumerator CalculateFPS()
    {
        var elapsedTime = 0f;
        var frames = 0;

        while(true)
        {        
            yield return null;

            frames ++;
            elapsedTime += Time.deltaTime;

            if(elapsedTime >= 1.0f)
            {
                FPS = frames;
                
                elapsedTime = 0f;
                frames = 0;

                UpdateLabels();
            }
        }
    }

    private void UpdateLabels()
    {
        _fpsLabel.text = "FPS: " + FPS;
    }

    private void ReloadScene()
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}

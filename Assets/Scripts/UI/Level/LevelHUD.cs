using UnityEngine.UI;
using UnityEngine;

public class LevelHUD : MonoBehaviour
{
    [Header("Windows")]
    public PauseWindow pauseWindow;
    public LevelResultsWindow levelResultsWindow;

    [Header("Screen UI")]
    [SerializeField] private Slider _nitroSlider;

    public void OnPauseButtonClick()
    {

    }
}

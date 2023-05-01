using UnityEngine;

public class Window : MonoBehaviour
{
    protected void OpenBase()
    {
        gameObject.SetActive(true);
    }

    protected void CloseBase()
    {
        gameObject.SetActive(false);
    }

    protected void OnButtonClick()
    {
        
    }
}

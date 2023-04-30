using UnityEngine;

public class Window : MonoBehaviour
{
    protected virtual void Open()
    {
        gameObject.SetActive(true);
    }

    protected virtual void Close()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnButtonClick()
    {
        
    }
}

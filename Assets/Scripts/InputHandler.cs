using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private CarHandler carHandler;

    public void OnLeftButtonPressed(BaseEventData eventData)
    {
        carHandler.SetHorizontalInput(-1);
    }

    public void OnRightButtonPressed(BaseEventData eventData)
    {
        carHandler.SetHorizontalInput(1);
    }

    public void OnButtonReleased(BaseEventData eventData)
    {
        carHandler.SetHorizontalInput(0);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class JoyStick : MonoBehaviour
{
    private Image jsContainer;
    private Image joystick;
    public Vector3 InputDirection;

    private bool isActive = false; // Toggle state

    void Start()
    {
        jsContainer = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
        InputDirection = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click toggles joystick
        {
            isActive = !isActive;

            if (!isActive) // Reset joystick when deactivated
            {
                InputDirection = Vector3.zero;
                joystick.rectTransform.anchoredPosition = Vector3.zero;
            }
        }

        if (isActive)
        {
            TrackMousePosition();
        }
    }

    private void TrackMousePosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;

        // Convert mouse position to local UI coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(jsContainer.rectTransform, mousePosition, null, out localPoint);

        // Normalize position based on container size
        localPoint.x = (localPoint.x / jsContainer.rectTransform.sizeDelta.x) * 2;
        localPoint.y = (localPoint.y / jsContainer.rectTransform.sizeDelta.y) * 2;

        InputDirection = new Vector3(localPoint.x, localPoint.y);
        InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

        // Move joystick based on input
        joystick.rectTransform.anchoredPosition = new Vector3(
            InputDirection.x * (jsContainer.rectTransform.sizeDelta.x / 3),
            InputDirection.y * (jsContainer.rectTransform.sizeDelta.y / 3)
        );
    }
}

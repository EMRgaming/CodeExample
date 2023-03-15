using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class managerJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image joystickBackground;
    private Image joystickKnob;
    private Vector2 posInput;

    // Start is called before the first frame update
    void Start()
    {
        joystickBackground = GetComponent<Image>();
        joystickKnob = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground.rectTransform, eventData.position, eventData.pressEventCamera, out posInput))
        {
            posInput.x = posInput.x / (joystickBackground.rectTransform.sizeDelta.x);
            posInput.y = posInput.y / (joystickBackground.rectTransform.sizeDelta.y);

            if(posInput.magnitude > 1.0f)
            {
                posInput = posInput.normalized;
            }

            joystickKnob.rectTransform.anchoredPosition = new Vector2(
                posInput.x * (joystickBackground.rectTransform.sizeDelta.x / 2),
                posInput.y * (joystickBackground.rectTransform.sizeDelta.y / 2));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        posInput = Vector2.zero;
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float inputHorizontal()
    {
        if (posInput.x != 0)
            return posInput.x;
        else
            return Input.GetAxis("Horizontal");
    }
    public float inputVertical()
    {
        if (posInput.y != 0)
            return posInput.y;
        else
            return Input.GetAxis("Vertical");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSelector : MonoBehaviour, IDragHandler
{
    private RectTransform rectTransform;
    private FlareCreationMenu flareCreation;
    private Canvas canvas;
    Vector3 cerPoint;
    Vector3 vigPoint;
    Vector3 finPoint;

    private bool cursor_over;

    public void Setup(GameObject[] attPoints_in)
    {
        rectTransform = GetComponent<RectTransform>();
        flareCreation = FindObjectOfType<FlareCreationMenu>();
        canvas = GetComponentInParent<Canvas>();

        cerPoint = attPoints_in[0].transform.Find("Point").GetComponent<RectTransform>().position;
        vigPoint = attPoints_in[1].transform.Find("Point").GetComponent<RectTransform>().position;
        finPoint = attPoints_in[2].transform.Find("Point").GetComponent<RectTransform>().position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        CursorOverLogic(eventData.position);

        if (!cursor_over)
        {
            //return;
        }

        float theta = Mathf.Atan2((transform.position.y - vigPoint.y), (transform.position.x - vigPoint.x));

        if (theta < (Mathf.PI / 3))
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x -2f, rectTransform.anchoredPosition.y + 2f);
            return;
        }
        else if(theta > (2 * Mathf.PI / 3))
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + 2f, rectTransform.anchoredPosition.y + 2f);
            return;
        }
        else if(transform.position.y > cerPoint.y)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y - 4);
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / (canvas.scaleFactor);
        flareCreation.UpdateAptitude(eventData.position);
    }


    private void CursorOverLogic(Vector2 mousePos)
    {
        float dist_to_selector = Vector2.Distance(mousePos, transform.position);

        if (cursor_over && dist_to_selector > 30)
        {
            cursor_over = false;
        }
        else if(!cursor_over && dist_to_selector < 15)
        {
            cursor_over = true;
        }
    }
}

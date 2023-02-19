using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    Image image;

    private void Awake()
    {
        if(image == null)
        {
            if(transform.parent != null)
            {
                image = GetComponentInParent<Image>();
            }
        }

        image.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.enabled = false;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        image.enabled = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class PageBarButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image arrowImage;
        public Image buttonImage;

        [HideInInspector]
        public PageBarUI pageBarUI;

        // Start is called before the first frame update
        void Start()
        {
            buttonImage.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            buttonImage.gameObject.SetActive(true);
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            buttonImage.gameObject.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            pageBarUI.ChangePage(this.gameObject.name);
        }
    }
}
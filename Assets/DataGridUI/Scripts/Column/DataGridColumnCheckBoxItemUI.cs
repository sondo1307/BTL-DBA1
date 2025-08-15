using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridColumnCheckBoxItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public RectTransform border;
        public Image image;

        [HideInInspector]
        public DataGridUI dataGridUI;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        void UpdateTheme()
        {
            if (image != null)
            {
                image.color = dataGridUI.theme.columnItemTheme.backgroundColor;
                image.sprite = dataGridUI.theme.checkBoxTheme.normalIcon;
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(
                                    dataGridUI.theme.checkBoxTheme.iconSize,
                                    dataGridUI.theme.checkBoxTheme.iconSize);
            }

            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = dataGridUI.theme.columnItemTheme.backgroundColor;
            }

            if (border != null)
            {
                if (dataGridUI.theme.columnItemTheme.isBorder == false)
                {
                    border.gameObject.SetActive(false);
                }
                else
                {
                    border.gameObject.SetActive(true);
                    for (int i = 0; i < border.childCount; i++)
                    {
                        Image imgBorder = border.GetChild(i).GetComponent<Image>();
                        if (imgBorder != null)
                        {
                            imgBorder.color = dataGridUI.theme.columnItemTheme.borderColor;
                        }
                    }
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = dataGridUI.theme.columnItemTheme.selectedColor;
            }

        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = dataGridUI.theme.columnItemTheme.backgroundColor;
            }
        }

    }
}
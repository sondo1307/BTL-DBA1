using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{


    public class DataGridColumnItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
    {
        public enum enumSortState
        {
            NONE,
            SortAscending,
            SortDescending,
        }

        public RectTransform border;
        public Text text;
        public Image sortImage;

        public Sprite sortAscendingSprite;
        public Sprite sortDescendingSprite;

        bool isOwnerClick = false;
        enumSortState sortSate = enumSortState.NONE;

        [HideInInspector]
        public DataGridColumnData data;
        [HideInInspector]
        public DataGridColumnUI columnUI;

        // Start is called before the first frame update
        void Start()
        {
            UpdateTheme();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateTheme()
        {
            if (text != null)
            {
                text.text = data.name;
                text.color = columnUI.dataGridUI.theme.columnItemTheme.textColor;
                //text.alignment = columnUI.dataGridUI.theme.columnItemTheme.textAlignment;
                text.alignment = data.columnTextAlignment;

                sortImage.color = text.color;
                sortImage.gameObject.SetActive(false);

                if (columnUI.dataGridUI.theme.columnItemTheme.textSpace > 0)
                {
                    RectTransform textRect = text.gameObject.GetComponent<RectTransform>();
                    textRect.offsetMin = new Vector2(columnUI.dataGridUI.theme.rowTheme.controlSpace, 0);
                    textRect.offsetMax = new Vector2(-columnUI.dataGridUI.theme.rowTheme.controlSpace, 0);

                    //switch (data.columnTextAlignment)
                    //{
                    //    case TextAnchor.UpperLeft:
                    //    case TextAnchor.MiddleLeft:
                    //    case TextAnchor.LowerLeft:
                    //        textRect.localPosition = new Vector3(textRect.rect.width / 2f + columnUI.dataGridUI.theme.columnItemTheme.textSpace, textRect.rect.height / -2f, 0);
                    //        break;
                    //    case TextAnchor.UpperRight:
                    //    case TextAnchor.MiddleRight:
                    //    case TextAnchor.LowerRight:
                    //        textRect.localPosition = new Vector3(textRect.rect.width / 2f - columnUI.dataGridUI.theme.columnItemTheme.textSpace, textRect.rect.height / -2f, 0);
                    //        break;
                    //}

                }
            }

            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = columnUI.dataGridUI.theme.columnItemTheme.backgroundColor;
            }

            if (border != null)
            {
                if (columnUI.dataGridUI.theme.columnItemTheme.isBorder == false)
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
                            imgBorder.color = columnUI.dataGridUI.theme.columnItemTheme.borderColor;
                        }
                    }
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (text != null)
            {
                text.color = columnUI.dataGridUI.theme.columnItemTheme.selectedTextColor;
            }
            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = columnUI.dataGridUI.theme.columnItemTheme.selectedColor;
            }

        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (text != null)
            {
                text.color = columnUI.dataGridUI.theme.columnItemTheme.textColor;
            }
            Image imgBG = GetComponent<Image>();
            if (imgBG != null)
            {
                imgBG.color = columnUI.dataGridUI.theme.columnItemTheme.backgroundColor;
            }
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (columnUI.dataGridUI.isColumnSort == false)
                return;

            isOwnerClick = true;
            columnUI.ClearAllSortState();
            isOwnerClick = false;

            switch (sortSate)
            {
                case enumSortState.NONE:
                    sortSate = enumSortState.SortAscending;
                    sortImage.gameObject.SetActive(true);
                    sortImage.sprite = sortAscendingSprite;
                    columnUI.dataGridUI.SortByColumn(data, false);
                    break;
                case enumSortState.SortAscending:
                    sortSate = enumSortState.SortDescending;
                    sortImage.gameObject.SetActive(true);
                    sortImage.sprite = sortDescendingSprite;
                    columnUI.dataGridUI.SortByColumn(data, true);
                    break;
                case enumSortState.SortDescending:
                    sortSate = enumSortState.SortAscending;
                    sortImage.gameObject.SetActive(true);
                    sortImage.sprite = sortAscendingSprite;
                    columnUI.dataGridUI.SortByColumn(data, false);
                    break;
            }
        }

        public void ClearSortState()
        {
            if (isOwnerClick == false)
                sortSate = enumSortState.NONE;

            sortImage.gameObject.SetActive(false);
        }
    }
}
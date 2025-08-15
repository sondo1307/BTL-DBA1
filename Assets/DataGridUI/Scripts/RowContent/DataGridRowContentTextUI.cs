using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContentTextUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Text text;

        // Start is called before the first frame update
        void Start()
        {
            //UpdateTheme();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateTheme()
        {
            if (text != null)
            {
                if (columnData.columnType == DataGridColumnData.enumColumnType.Text)
                {
                    text.text = rowItemData.textData;
                    rowItemData.value= text.text;
                    rowItemData.value = rowItemData.textData;

                    text.fontSize = dataGridUI.theme.rowTheme.textFontSize;
                    if (isAlternating == false)
                        text.color = dataGridUI.theme.rowTheme.textColor;
                    else
                        text.color = dataGridUI.theme.rowTheme.alternatingTextColor;

                    text.alignment = columnData.rowTextAlignment;

                    if (dataGridUI.theme.rowTheme.controlSpace > 0)
                    {
                        RectTransform textRect = text.gameObject.GetComponent<RectTransform>();
                        textRect.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, 0);
                        textRect.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, 0);

                        //switch (columnData.rowTextAlignment)
                        //{
                        //    case TextAnchor.UpperLeft:
                        //    case TextAnchor.MiddleLeft:
                        //    case TextAnchor.LowerLeft:
                        //        textRect.localPosition = new Vector3(textRect.rect.width / 2f + dataGridUI.theme.rowTheme.textSpace, textRect.rect.height / -2f, 0);
                        //        break;
                        //    case TextAnchor.UpperRight:
                        //    case TextAnchor.MiddleRight:
                        //    case TextAnchor.LowerRight:
                        //        textRect.localPosition = new Vector3(textRect.rect.width / 2f - dataGridUI.theme.rowTheme.textSpace, textRect.rect.height / -2f, 0);
                        //        break;
                        //}

                    }
                }
                else
                {
                    text.gameObject.SetActive(false);
                }
            }

            base.UpdateTheme();
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.UpdateSelectState(enumItemState.Enter, true);
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.UpdateSelectState(enumItemState.Normal, true);
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (rowUI != null)
                rowUI.OnRowClick();
        }

        public override void UpdateSelectState(enumItemState newState)
        {
            switch (newState)
            {
                case enumItemState.Normal:
                    if (isAlternating == false)
                    {
                        text.color = dataGridUI.theme.rowTheme.textColor;
                    }
                    else
                    {
                        text.color = dataGridUI.theme.rowTheme.alternatingTextColor;
                    }
                    break;
                case enumItemState.Select:
                    text.color = dataGridUI.theme.rowTheme.selectedTextColor;
                    break;
                case enumItemState.Enter:
                    text.color = dataGridUI.theme.rowTheme.enterTextColor;
                    break;

            }

            base.UpdateSelectState(newState);

        }

        public override void SetValue(string value)
        {
            text.text = value;
            base.SetValue(value);
        }
    }
}
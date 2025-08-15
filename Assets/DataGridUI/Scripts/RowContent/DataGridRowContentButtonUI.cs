using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContentButtonUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Button button;

        // Start is called before the first frame update
        void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(buttonOnClick);
            }
        }

        void buttonOnClick()
        {
            dataGridUI.onRowButtonClick.Invoke(rowItemData);
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        public override void UpdateTheme()
        {
            if (button != null)
            {
                if (columnData.columnType == DataGridColumnData.enumColumnType.Button)
                {
                    button.GetComponentInChildren<Text>().text = rowItemData.textData;
                    rowItemData.value= rowItemData.textData;
                    button.GetComponentInChildren<Text>().fontSize = dataGridUI.theme.rowTheme.textFontSize;
                    RectTransform rectTransform = button.gameObject.GetComponent<RectTransform>();
                    rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.controlHeight / -2f);
                    rectTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.rowTheme.controlHeight / 2f);

                    if (dataGridUI.theme.rowTheme.controlSpace > 0)
                    {
                        RectTransform textRect = button.gameObject.GetComponent<RectTransform>();
                    }
                }
                else
                {
                    button.gameObject.SetActive(false);
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
            //switch (newState)
            //{
            //    case enumItemState.Normal:
            //        if (isAlternating == false)
            //        {
            //            text.color = dataGridUI.theme.rowTheme.textColor;
            //        }
            //        else
            //        {
            //            text.color = dataGridUI.theme.rowTheme.alternatingTextColor;
            //        }
            //        break;
            //    case enumItemState.Select:
            //        text.color = dataGridUI.theme.rowTheme.selectedTextColor;
            //        break;
            //    case enumItemState.Enter:
            //        text.color = dataGridUI.theme.rowTheme.enterTextColor;
            //        break;

            //}

            base.UpdateSelectState(newState);

        }

        public override void SetValue(string value)
        {
            button.GetComponentInChildren<Text>().text = value;
            base.SetValue(value);
        }
    }
}
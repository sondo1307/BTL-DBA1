using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{
    public class DataGridRowContentPercentageUI :  DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Text text;
        public Image barImage;
        public Image bgImage;

        float percentValue = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateTheme()
        {
            if (text != null && barImage!=null)
            {
                percentValue = Convert.ToSingle(rowItemData.textData);
                text.text = rowItemData.textData + "%";
                rowItemData.value = rowItemData.textData;

                text.fontSize = dataGridUI.theme.rowTheme.textFontSize;
                //if (isAlternating == false)
                //    text.color = dataGridUI.theme.rowTheme.textColor;
                //else
                //    text.color = dataGridUI.theme.rowTheme.alternatingTextColor;

                text.alignment =  TextAnchor.MiddleCenter;

                RectTransform rectTransform = text.gameObject.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / -2f);
                rectTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / 2f);

                rectTransform = bgImage.gameObject.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / -2f);
                rectTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / 2f);
                //rectTransform = progressImage.GetComponent<RectTransform>();
                //rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / -2f);
                //rectTransform.offsetMax = new Vector2(-dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / 2f);

                bgImage.color = dataGridUI.theme.percentageTheme.barBackgroundColor;

                UpdateProgressRangeColor();
                UpdateProgressValue();
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


        void UpdateProgressRangeColor()
        {
            if (dataGridUI.theme.percentageTheme.rangeColors == null)
            {
                UpdateProgressNormalColor();
                return;
            }
            if (dataGridUI.theme.percentageTheme.rangeColors.Count == 0)
            {
                UpdateProgressNormalColor();
                return;
            }

            foreach(DataGridPercentageRangeTheme range in dataGridUI.theme.percentageTheme.rangeColors)
            {
                if (percentValue >= range.startRange && percentValue <= range.endRange)
                {
                    text.color = range.textColor;
                    barImage.color = range.barColor;

                    return;
                }
            }

            UpdateProgressNormalColor();
        }

        void UpdateProgressNormalColor()
        {
            text.color = dataGridUI.theme.percentageTheme.textColor;
            barImage.color = dataGridUI.theme.percentageTheme.barColor;
        }

        void UpdateProgressValue()
        {
            float scaleWidth = (columnData.width - dataGridUI.theme.rowTheme.controlSpace );
            float p = (scaleWidth - scaleWidth * ( percentValue/100f) + dataGridUI.theme.rowTheme.controlSpace) * -1f;
            //float scaleWidth = columnData.width ;
            //float p = (scaleWidth - scaleWidth * (percentValue / 100f)) * -1f;

            RectTransform rectTransform = barImage.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(dataGridUI.theme.rowTheme.controlSpace, dataGridUI.theme.percentageTheme.barHeight / -2f);
            rectTransform.offsetMax = new Vector2(p, dataGridUI.theme.percentageTheme.barHeight / 2f);

        }

        public override void OnResize()
        {
            UpdateProgressValue();
            base.OnResize();
        }

        public override void SetValue(string value)
        {
            if (float.TryParse(value, out percentValue))
            {
                text.text = percentValue.ToString() + "%";
                UpdateProgressRangeColor();
                UpdateProgressValue();
            }

            base.SetValue(value);
        }
    }
}

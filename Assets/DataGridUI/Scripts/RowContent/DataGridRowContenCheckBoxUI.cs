using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowContenCheckBoxUI : DagaGridRowContentUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Image imageCheckBox;

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
            if (columnData.columnType == DataGridColumnData.enumColumnType.CheckBox && rowItemData.checkData)
            {
                imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.checkIcon;
                rowItemData.value = "True";

            }
            else
            {
                imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.normalIcon;
                rowItemData.value = "False";
            }

            imageCheckBox.GetComponent<RectTransform>().sizeDelta = new Vector2(
                                dataGridUI.theme.checkBoxTheme.iconSize,
                                dataGridUI.theme.checkBoxTheme.iconSize);

            base.UpdateTheme();
        }

        public override void UpdateSelectState(enumItemState newState)
        {
            switch (newState)
            {
                case enumItemState.Normal:
                    if (columnData.columnType == DataGridColumnData.enumColumnType.GridMultipleCheckBox && imageCheckBox != null)
                    {
                        imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.normalIcon;
                        rowItemData.value = "false";
                    }
                    break;
                case enumItemState.Select:
                    if (columnData.columnType == DataGridColumnData.enumColumnType.GridMultipleCheckBox && imageCheckBox != null)
                    {
                        imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.checkIcon;
                        rowItemData.value="true";
                    }
                    break;
                case enumItemState.Enter:
                    break;

            }

            base.UpdateSelectState(newState);
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
            if (columnData.columnType == DataGridColumnData.enumColumnType.CheckBox)
            {
                rowItemData.checkData = !rowItemData.checkData;
                rowItemData.value = rowItemData.checkData.ToString();

                if (rowItemData.checkData)
                {
                    imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.checkIcon;
                    dataGridUI.onRowContentChange.Invoke("Checked", rowItemData);
                }
                else
                {
                    imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.normalIcon;
                    dataGridUI.onRowContentChange.Invoke("Unchecked", rowItemData);
                }

                return;
            }

            if (rowUI != null)
                rowUI.OnRowClick();
        }

        public override void SetValue(string value)
        {
            if (columnData.columnType != DataGridColumnData.enumColumnType.CheckBox)
                return;

            if (Boolean.TryParse(value, out rowItemData.checkData)==true)
            {
                if (rowItemData.checkData)
                {
                    imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.checkIcon;
                    dataGridUI.onRowContentChange.Invoke("Checked", rowItemData);
                }
                else
                {
                    imageCheckBox.sprite = dataGridUI.theme.checkBoxTheme.normalIcon;
                    dataGridUI.onRowContentChange.Invoke("Unchecked", rowItemData);
                }

            }

            base.SetValue(value);
        }

    }
}
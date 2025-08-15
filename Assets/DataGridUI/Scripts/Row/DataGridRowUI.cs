using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maything.UI.DataGridUI.DagaGridRowContentUI;

namespace Maything.UI.DataGridUI
{

    public class DataGridRowUI : MonoBehaviour
    {
        [HideInInspector]
        public DataGridUI dataGridUI;

        [HideInInspector]
        public DataGridRowData rowData;

        [HideInInspector]
        public bool isAlternating = false;

        RectTransform ownerTransform;

        [HideInInspector]
        public List<DagaGridRowContentUI> rowItems = new List<DagaGridRowContentUI>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InitializationRow()
        {
            ownerTransform = GetComponent<RectTransform>();
            if (rowData == null) return;

            float offsetX = 0;

            float rHeight = dataGridUI.rowHeight;
            if (rHeight == 0) rHeight = dataGridUI.theme.rowTheme.rowHeight;
            //multipleCheckBox

            for (int i = 0; i < dataGridUI.columnData.Count; i++)
            {
                if (i >= rowData.rowData.Count)
                    continue;

                rowData.rowData[i].columnIndex = i;

                GameObject go = null;

                switch (dataGridUI.columnData[i].columnType)
                {
                    case DataGridColumnData.enumColumnType.CheckBox:
                    case DataGridColumnData.enumColumnType.GridMultipleCheckBox:
                        go = Instantiate(dataGridUI.rowCheckBoxTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.Text:
                        go = Instantiate(dataGridUI.rowTextItemTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.Photo:
                        go = Instantiate(dataGridUI.rowPhotoTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.Dropdown:
                        go = Instantiate(dataGridUI.rowDropdownTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.InputField:
                        go = Instantiate(dataGridUI.rowInputFieldTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.Button:
                        go = Instantiate(dataGridUI.rowButtomTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.MultilineInputField:
                        go = Instantiate(dataGridUI.rowMultilineInputFieldTemplate, ownerTransform);
                        break;
                    case DataGridColumnData.enumColumnType.Percentage:
                        go = Instantiate(dataGridUI.rowPercentageTemplate, ownerTransform);
                        break;

                }

                RectTransform rect = go.GetComponent<RectTransform>();
                //rect.sizeDelta = new Vector2(dataGridUI.columnData[i].width, dataGridUI.theme.rowTheme.rowHeight);
                rect.sizeDelta = new Vector2(dataGridUI.columnData[i].width, rHeight);
                rect.localPosition = new Vector3(offsetX, 0, 0);

                DagaGridRowContentUI itemUI = go.GetComponent<DagaGridRowContentUI>();
                if (itemUI != null)
                {
                    itemUI.dataGridUI = dataGridUI;
                    itemUI.isAlternating = isAlternating;
                    itemUI.rowItemData = rowData.rowData[i];
                    itemUI.columnData = dataGridUI.columnData[i];
                    itemUI.rowUI = this;

                    itemUI.UpdateTheme();

                    rowItems.Add(itemUI);
                }

                offsetX += dataGridUI.columnData[i].width;
            }
        }

        public void ResizeRow()
        {
            float offsetX = 0;
            float rHeight = dataGridUI.rowHeight;
            if (rHeight == 0) rHeight = dataGridUI.theme.rowTheme.rowHeight;

            for (int i = 0; i < dataGridUI.columnData.Count; i++)
            {
                if (i >= rowData.rowData.Count)
                    continue;

                GameObject go = rowItems[i].gameObject;
                RectTransform rect = go.GetComponent<RectTransform>();
                //rect.sizeDelta = new Vector2(dataGridUI.columnData[i].width, dataGridUI.theme.rowTheme.rowHeight);
                rect.sizeDelta = new Vector2(dataGridUI.columnData[i].width, rHeight);
                rect.localPosition = new Vector3(offsetX, 0, 0);
                rowItems[i].OnResize();

                offsetX += dataGridUI.columnData[i].width;

            }
        }

        public void UpdateSelectState(enumItemState newState, bool checkSelected)
        {
            if (dataGridUI.IsRowSelected(this) && checkSelected == true)
            {
                foreach (DagaGridRowContentUI itemUI in rowItems)
                {
                    itemUI.UpdateSelectState(enumItemState.Select);
                }
                rowData.isSelected = true;

            }
            else
            {
                foreach (DagaGridRowContentUI itemUI in rowItems)
                {
                    itemUI.UpdateSelectState(newState);
                }

                if (newState == enumItemState.Select)
                    rowData.isSelected = true;
                else
                    rowData.isSelected = false;

            }
        }

        public void OnRowClick()
        {
            dataGridUI.WhenRowClick(this,true);
            //UpdateSelectState(enumItemState.Select,true);
        }
    }
}
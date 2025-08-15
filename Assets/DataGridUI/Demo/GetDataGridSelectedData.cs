using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Maything.UI.DataGridUI
{

    public class GetDataGridSelectedData : MonoBehaviour
    {
        public DataGridUI dataGridUI;
        public Text text;
        public Image image;

        public void GetSelectedData(List<DataGridColumnData> columnData, DataGridRowData data)
        {
            text.text = "";

            if (data == null) return;

            for (int i = 0; i < data.rowData.Count; i++)
            {
                switch (columnData[i].columnType)
                {
                    case DataGridColumnData.enumColumnType.GridMultipleCheckBox:
                        break;
                    case DataGridColumnData.enumColumnType.CheckBox:
                        text.text += data.rowData[i].checkData.ToString() + ",";
                        break;
                    case DataGridColumnData.enumColumnType.Text:
                        text.text += data.rowData[i].textData + ",";
                        break;
                    case DataGridColumnData.enumColumnType.Photo:
                        image.sprite = data.rowData[i].photoData;
                        break;
                    default:
                        text.text += data.rowData[i].textData + ",";
                        break;
                }
            }
        }

        public void GetMultipleSelectedData(List<DataGridColumnData> columnData, List<DataGridRowData> dataList)
        {
            text.text = "";

            foreach (DataGridRowData data in dataList)
            {
                for (int i = 0; i < data.rowData.Count; i++)
                {
                    switch (columnData[i].columnType)
                    {
                        case DataGridColumnData.enumColumnType.GridMultipleCheckBox:
                            break;
                        case DataGridColumnData.enumColumnType.CheckBox:
                            text.text += data.rowData[i].checkData.ToString() + ",";
                            break;
                        case DataGridColumnData.enumColumnType.Text:
                            text.text += data.rowData[i].textData + ",";
                            break;
                        case DataGridColumnData.enumColumnType.Photo:
                            image.sprite = data.rowData[i].photoData;
                            break;
                        default:
                            text.text += data.rowData[i].textData + ",";
                            break;
                    }
                }
                text.text += Environment.NewLine;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
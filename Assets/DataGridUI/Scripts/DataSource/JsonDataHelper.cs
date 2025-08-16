using System;
using System.Collections;
using System.Collections.Generic;
using static Maything.UI.DataGridUI.DataGridUI;
using System.IO;
using System.Text;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    public class JsonDataHelper
    {
        public static void LoadJsonFile(DataGridUI dataGridUI, string file)
        {
            FileStream FS = new FileStream(file, FileMode.Open);
            StreamReader SR = new StreamReader(FS, Encoding.UTF8);
            string code = SR.ReadToEnd();

            //jsonr

            SR.Close();
            SR = null;
            FS.Close();
            FS = null;

            JsonToDataGridData(dataGridUI,code);
        }

        public static void JsonToDataGridData(DataGridUI dataGridUI, string code)
        {
            JsonData jsonData = new JsonData();

            jsonData = JsonUtility.FromJson(code, typeof(JsonData)) as JsonData;
            if (jsonData == null) return;

            if (dataGridUI.dataType == enumDataType.Column ||
                dataGridUI.dataType == enumDataType.ColumnAndRow)
            {
                JsonStringToColumnData(dataGridUI,jsonData);
            }

            if (dataGridUI.dataType == enumDataType.ColumnAndRow ||
                dataGridUI.dataType == enumDataType.Row)
            {
                JsonStringToRowData(dataGridUI, jsonData, null);
            }

        }

        public static void JsonStringToColumnData(DataGridUI dataGridUI, JsonData jsonData)
        {
            dataGridUI.ColumnClear();
            dataGridUI.columnData.Clear();
            foreach (JsonColumnData column in jsonData.columnData)
            {
                DataGridColumnData cData = new DataGridColumnData();
                cData.name = column.name;
                cData.width = column.width;
                cData.rowTextAlignment = TextAnchor.MiddleLeft;
                cData.columnTextAlignment = TextAnchor.MiddleLeft;
                cData.columnType = DataGridColumnData.enumColumnType.Text;

                if (column.dropdownData != null)
                    cData.dropdownData = column.dropdownData;

                switch (column.type)
                {
                    case "SelectedBox":
                        cData.columnType = DataGridColumnData.enumColumnType.GridMultipleCheckBox;
                        break;
                    case "CheckBox":
                        cData.columnType = DataGridColumnData.enumColumnType.CheckBox;
                        break;
                    case "InputField":
                        cData.columnType = DataGridColumnData.enumColumnType.InputField;
                        break;
                    case "Button":
                        cData.columnType = DataGridColumnData.enumColumnType.Button;
                        break;
                    case "Dropdown":
                        cData.columnType = DataGridColumnData.enumColumnType.Dropdown;
                        break;
                    case "Percentage":
                        cData.columnType = DataGridColumnData.enumColumnType.Percentage;
                        break;
                    case "Int":
                        cData.columnType = DataGridColumnData.enumColumnType.Int;
                        break;
                    case "MultilineInputField":
                        cData.columnType = DataGridColumnData.enumColumnType.MultilineInputField;
                        break;
                    case "Photo":
                        cData.columnType = DataGridColumnData.enumColumnType.Photo;
                        //rowData contains marker and name of resource, e.g. "spriteRes:Images/5Smileys/Smiley0"
                        break;
                }

                dataGridUI.columnData.Add(cData);
            }
        }

        public static void JsonStringToRowData(DataGridUI dataGridUI, JsonData jsonData, DataGridRowUI selectItem)
        {
            int i = 1;
            foreach (JsonRowData jData in jsonData.rowData)
            {
                DataGridRowData rData = new DataGridRowData();
                foreach (string d in jData.data)
                {
                    DataGridRowItemData iData = new DataGridRowItemData();
                    iData.textData = d;
                    if (iData.value == "")
                        iData.value = iData.textData;

                    //iData.key = d;
                    string s = d.ToLower();
                    if (s == "true" || s == "false")
                    {
                        iData.checkData = Convert.ToBoolean(s);
                    }
                    if (d.IndexOf("SpriteResource:") == 0)
                    {
                        //s hold resource name like "images/5Smileys/Smiley0"
                        //(images folder must be located in Asstes/Resources folder
                        s = d.Remove(0, "SpriteResource:".Length);
                        iData.photoData = SpriteHelper.GetResourceSprite(s);
                    }
                    if (d.IndexOf("SpriteStreaming:") ==0)
                    {
                        //s hold resource name like "images/5Smileys/Smiley0"
                        //(images folder must be located in StreamingAssets folder
                        s = d.Remove(0, "SpriteStreaming:".Length);
                        iData.photoData = SpriteHelper.GetStreamingAssetsSprite(s);
                    }

                    rData.rowData.Add(iData);
                }

                if (selectItem != null && selectItem.rowData.rowData.Count > 0)
                {
                    dataGridUI.rowData.Insert(selectItem.rowData.rowData[0].rowIndex + i, rData);
                    i++;
                }
                else
                    dataGridUI.rowData.Add(rData);
            }
        }

        public static void DataFromJson(DataGridUI dataGridUI,  bool isColumn, bool isRow, bool isClearRow, bool isInsertRow, string jsonText)
        {
            JsonData jsonData = new JsonData();
            DataGridRowUI selectItem = null;

            jsonData = JsonUtility.FromJson(jsonText, typeof(JsonData)) as JsonData;
            if (jsonData == null) return;

            if (isInsertRow)
            {
                selectItem = dataGridUI.GetLastSelectItem();
                if (selectItem == null) isInsertRow = false;
            }

            if (isColumn)
            {
                JsonStringToColumnData(dataGridUI, jsonData);
                dataGridUI.InitializationColumn();
            }

            if (isRow)
            {
                if (isClearRow)
                {
                    dataGridUI.PageRowClear();
                    dataGridUI.RowClear();
                }

                JsonStringToRowData(dataGridUI, jsonData, selectItem);
            }

            dataGridUI.InitializationPaging();
            dataGridUI.InitializationRow(isClearRow);

        }

        public static string ExportDataToJson(DataGridUI dataGridUI)
        {
            JsonData jsonData = new JsonData();

            foreach(DataGridColumnData column in dataGridUI.columnData)
            {
                JsonColumnData jColumn = new JsonColumnData();
                jColumn.name=column.name;
                jColumn.width=column.width;
                jColumn.dropdownData= column.dropdownData;
                switch(column.columnType)
                {
                    case DataGridColumnData.enumColumnType.GridMultipleCheckBox:
                        jColumn.type = "SelectedBox";
                        break;
                    case DataGridColumnData.enumColumnType.CheckBox:
                        jColumn.type = "CheckBox";
                        break;
                    case DataGridColumnData.enumColumnType.InputField:
                        jColumn.type = "InputField";
                        break;
                    case DataGridColumnData.enumColumnType.Button:
                        jColumn.type = "Button";
                        break;
                    case DataGridColumnData.enumColumnType.Dropdown:
                        jColumn.type = "Dropdown";
                        break;
                    case DataGridColumnData.enumColumnType.Percentage:
                        jColumn.type = "Percentage";
                        break;
                    case DataGridColumnData.enumColumnType.Int:
                        jColumn.type = "Int";
                        break;   
                    case DataGridColumnData.enumColumnType.MultilineInputField:
                        jColumn.type = "MultilineInputField";
                        break;
                    case DataGridColumnData.enumColumnType.Photo:
                        jColumn.type = "Photo";
                        break;

                }
                jsonData.columnData.Add(jColumn);

            }

            foreach (DataGridRowData row in dataGridUI.rowData)
            {
                JsonRowData jRowData = new JsonRowData();
                for (int x = 0; x < row.rowData.Count; x++)
                {
                    DataGridRowItemData item = row.rowData[x];

                    jRowData.data.Add(item.value);
                }
                

                jsonData.rowData.Add(jRowData);
            }

            return JsonUtility.ToJson(jsonData,true);
        }
    }
}
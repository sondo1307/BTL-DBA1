using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static Maything.UI.DataGridUI.DataGridUI;

namespace Maything.UI.DataGridUI
{
    public class CSVDataHelper 
    {
        public static void CSVToDataGridData(DataGridUI dataGridUI, string code)
        {
            string tmp = code.Replace("\r\n", "\n");
            string[] spt = tmp.Split('\n');
            if (spt.Length <= 0) return;

            if (dataGridUI.dataType == enumDataType.Column ||
                dataGridUI.dataType == enumDataType.ColumnAndRow)
            {
                if (CSVStringToColumnData(dataGridUI,spt[0]) == false)
                {
                    if (dataGridUI.dataType == enumDataType.ColumnAndRow)
                        dataGridUI.rowData.Add(CSVStringToRowData(spt[0]));
                }

            }
            else
            {
                dataGridUI.rowData.Add(CSVStringToRowData(spt[0]));
            }

            if (dataGridUI.dataType == enumDataType.ColumnAndRow ||
                dataGridUI.dataType == enumDataType.Row)
            {
                for (int i = 1; i < spt.Length; i++)
                {
                    if (spt[i].Trim() == "")
                        continue;

                    dataGridUI.rowData.Add(CSVStringToRowData(spt[i]));
                }
            }

        }

        public static void LoadCSVFile(DataGridUI dataGridUI, string file)
        {
            FileStream FS = new FileStream(file, FileMode.Open);
            StreamReader SR = new StreamReader(FS, Encoding.UTF8);

            dataGridUI.rowData.Clear();
            string tmp = SR.ReadLine();

            if (dataGridUI.dataType == enumDataType.Column ||
                dataGridUI.dataType == enumDataType.ColumnAndRow)
            {
                if (CSVStringToColumnData(dataGridUI, tmp) == false)
                {
                    if (dataGridUI.dataType == enumDataType.ColumnAndRow)
                        dataGridUI.rowData.Add(CSVStringToRowData(tmp));
                }

            }
            else
            {
                dataGridUI.rowData.Add(CSVStringToRowData(tmp));
            }

            if (dataGridUI.dataType == enumDataType.ColumnAndRow ||
                dataGridUI.dataType == enumDataType.Row)
            {
                while (!SR.EndOfStream)
                {
                    tmp = SR.ReadLine();
                    if (tmp.Trim() == "")
                        continue;
                    dataGridUI.rowData.Add(CSVStringToRowData(tmp));

                }
            }


            SR.Close();
            SR = null;
            FS.Close();
            FS = null;
        }

        public static bool CSVStringToColumnData(DataGridUI dataGridUI, string code)
        {
            if (code.Trim() == "") return false;
            if (code.Substring(0, 1) == "[" && code.Substring(code.Length - 1, 1) == "]")
            {
                //Column
                dataGridUI.ColumnClear();
                dataGridUI.columnData.Clear();
                string[] spt = code.Substring(1, code.Length - 2).Split(',');
                for (int i = 0; i < spt.Length; i++)
                {
                    string[] param = spt[i].Split('|');
                    DataGridColumnData cData = new DataGridColumnData();
                    cData.name = param[0];
                    if (param.Length >= 2)
                    {
                        cData.width = Convert.ToSingle(param[1]);
                        if (param.Length == 3)
                        {
                            switch (param[2])
                            {
                                case "SelectedBox":
                                    cData.columnType = DataGridColumnData.enumColumnType.GridMultipleCheckBox;
                                    break;
                                case "CheckBox":
                                    cData.columnType = DataGridColumnData.enumColumnType.CheckBox;
                                    break;
                                case "Percentage":
                                    cData.columnType = DataGridColumnData.enumColumnType.Percentage;
                                    break;
                                case "Photo":
                                    cData.columnType = DataGridColumnData.enumColumnType.Photo;
                                    //rowData contains marker and name of resource, e.g. "spriteRes:Images/5Smileys/Smiley0"
                                    break;
                            }
                        }
                    }

                    dataGridUI.columnData.Add(cData);
                }

                return true;
            }
            else
            {
                //rowData.Add(CSVStringToRowData(code));
                return false;
            }

        }

        public static DataGridRowData CSVStringToRowData(string code)
        {
            string[] spt = code.Split(',');
            DataGridRowData data = new DataGridRowData();
            foreach (string s in spt)
            {
                DataGridRowItemData itemData = new DataGridRowItemData();
                itemData.textData = s;
                string s1 = s.ToLower();
                if (s1 == "true" || s1 == "false")
                {
                    itemData.checkData = Convert.ToBoolean(s1);
                }
                if (s.IndexOf("SpriteResource:") == 0)
                {
                    //s hold resource name like "images/5Smileys/Smiley0"
                    //(images folder must be located in Asstes/Resources folder
                    itemData.photoData = SpriteHelper.GetResourceSprite(s.Remove(0, "SpriteResource:".Length));
                }
                if (s.IndexOf("SpriteStreaming:") == 0)
                {
                    //s hold resource name like "images/5Smileys/Smiley0"
                    //(images folder must be located in StreamingAssets folder
                    itemData.photoData = SpriteHelper.GetStreamingAssetsSprite(s.Remove(0, "SpriteStreaming:".Length));
                }

                data.rowData.Add(itemData);
            }

            return data;
        }


        public static void DataFromCSV(DataGridUI dataGridUI, bool isColumn, bool isRow, bool isClearRow, bool isInsertRow, string csvText)
        {
            string tmp = csvText.Replace("\r\n", "\n");
            string[] spt = tmp.Split('\n');
            if (spt.Length <= 0) return;

            DataGridRowUI selectItem = null;

            if (isInsertRow)
            {
                selectItem = dataGridUI.GetLastSelectItem();
                if (selectItem == null) isInsertRow = false;
            }

            if (isColumn)
            {
                CSVStringToColumnData(dataGridUI, spt[0]);
                dataGridUI.InitializationColumn();
            }

            if (isRow)
            {
                if (isClearRow)
                {
                    dataGridUI.PageRowClear();
                    dataGridUI.RowClear();
                }

                int startIndex = 1;
                if (isColumn == false)
                    startIndex = 0;

                for (int i = startIndex; i < spt.Length; i++)
                {
                    if (spt[i].Trim() == "")
                        continue;

                    if (isInsertRow)
                    {
                        dataGridUI.rowData.Insert(selectItem.rowData.rowData[0].rowIndex + i - startIndex + 1, CSVDataHelper.CSVStringToRowData(spt[i]));
                    }
                    else
                        dataGridUI.rowData.Add(CSVDataHelper.CSVStringToRowData(spt[i]));
                }

            }
            dataGridUI.InitializationPaging();
            dataGridUI.InitializationRow(isClearRow);

        }

        public static string ExportToCSV(DataGridUI dataGridUI)
        {
            string csv = "[";
            foreach(DataGridColumnData column in dataGridUI.columnData)
            {
                csv += "|" + column.width.ToString();
                switch(column.columnType)
                {
                    case DataGridColumnData.enumColumnType.GridMultipleCheckBox:
                        csv = csv + "|SelectedBox";
                        break;
                    case DataGridColumnData.enumColumnType.CheckBox:
                        csv = csv + "|CheckBox";
                        break;
                    case DataGridColumnData.enumColumnType.Percentage:
                        csv = csv + "|Percentage";
                        break;
                    case DataGridColumnData.enumColumnType.Photo:
                        csv = csv + "|Photo";
                        break;

                }
                csv += "," + column.name;
            }
            csv += "]" + Environment.NewLine;

            foreach(DataGridRowData row in dataGridUI.rowData)
            {
                for(int x=0;x<row.rowData.Count;x++) 
                {
                    DataGridRowItemData item = row.rowData[x];

                    csv += item.value;

                    if (x<row.rowData.Count-1)
                        csv+= ",";
                }
                csv += Environment.NewLine;
            }

            return csv;
        }
    }
}
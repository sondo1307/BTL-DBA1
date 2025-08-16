using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    public class DataSortHelper
    {
        // public static List<DataGridRowData> SortRowData(List<DataGridRowData> rowData,int orderIndex, bool isDescending)
        // {
        //     List<DataGridRowData> sortList = new List<DataGridRowData>();
        //
        //     if (isDescending)
        //     {
        //         sortList = rowData.OrderByDescending(u => u.rowData[orderIndex].value).ToList();
        //     }
        //     else
        //     {
        //         sortList = rowData.OrderBy(u => u.rowData[orderIndex].value).ToList();
        //     }
        //
        //     return sortList;
        // }

        public static List<DataGridRowData> SortRowData(List<DataGridRowData> rowData, int orderIndex, bool isDescending)
        {
            List<DataGridRowData> sortList = new List<DataGridRowData>();

            if (rowData == null || rowData.Count == 0)
                return rowData;

            // Lấy sample value để đoán kiểu dữ liệu
            string sampleValue = rowData[0].rowData[orderIndex].value;

            bool isInt = int.TryParse(sampleValue, out _);
            bool isDouble = double.TryParse(sampleValue, out _);
            bool isDate = DateTime.TryParse(sampleValue, out _);

            if (isInt)
            {
                if (isDescending)
                    sortList = rowData.OrderByDescending(u => int.TryParse(u.rowData[orderIndex].value, out var num) ? num : int.MinValue).ToList();
                else
                    sortList = rowData.OrderBy(u => int.TryParse(u.rowData[orderIndex].value, out var num) ? num : int.MaxValue).ToList();
            }
            else if (isDouble)
            {
                if (isDescending)
                    sortList = rowData.OrderByDescending(u => double.TryParse(u.rowData[orderIndex].value, out var num) ? num : double.MinValue).ToList();
                else
                    sortList = rowData.OrderBy(u => double.TryParse(u.rowData[orderIndex].value, out var num) ? num : double.MaxValue).ToList();
            }
            else if (isDate)
            {
                if (isDescending)
                    sortList = rowData.OrderByDescending(u => DateTime.TryParse(u.rowData[orderIndex].value, out var dt) ? dt : DateTime.MinValue).ToList();
                else
                    sortList = rowData.OrderBy(u => DateTime.TryParse(u.rowData[orderIndex].value, out var dt) ? dt : DateTime.MaxValue).ToList();
            }
            else
            {
                if (isDescending)
                    sortList = rowData.OrderByDescending(u => u.rowData[orderIndex].value).ToList();
                else
                    sortList = rowData.OrderBy(u => u.rowData[orderIndex].value).ToList();
            }

            return sortList;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    public class DataSortHelper
    {
        public static List<DataGridRowData> SortRowData(List<DataGridRowData> rowData,int orderIndex, bool isDescending)
        {
            List<DataGridRowData> sortList = new List<DataGridRowData>();

            if (isDescending)
            {
                sortList = rowData.OrderByDescending(u => u.rowData[orderIndex].value).ToList();
            }
            else
            {
                sortList = rowData.OrderBy(u => u.rowData[orderIndex].value).ToList();
            }

            return sortList;
        }

    }
}
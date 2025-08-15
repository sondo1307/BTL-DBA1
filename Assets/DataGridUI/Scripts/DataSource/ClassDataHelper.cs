using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maything.UI.DataGridUI
{
    public class ClassDataHelper
    {
        public static void DataFromRowItemsData(DataGridUI dataGridUI, bool isClearRow, bool isInsertRow, List<DataGridRowData> rowsData)
        {
            DataGridRowUI selectItem = null;
            if (isInsertRow)
            {
                selectItem = dataGridUI.GetLastSelectItem();
                if (selectItem == null) isInsertRow = false;
            }

            if (isClearRow)
            {
                dataGridUI.PageRowClear();
                dataGridUI.RowClear();
            }

            if (selectItem == null)
                dataGridUI.rowData.AddRange(rowsData);
            else
                dataGridUI.rowData.InsertRange(selectItem.rowData.rowData[0].rowIndex + 1, rowsData);

            dataGridUI.InitializationPaging();
            dataGridUI.InitializationRow(isClearRow);
        }
    }
}

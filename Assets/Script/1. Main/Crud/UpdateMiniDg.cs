using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateMiniDg : UpdateAndInsertMiniBase
{
    [SerializeField] private Button _updateBtn;

    protected override void Start()
    {
        base.Start();
        _updateBtn.onClick.AddListener(UpdateRow);
    }

    public override void Show(string tableName, DataGridRowData rowData)
    {
        base.Show(tableName, rowData);
        if (_dataGridUI.columnData.Count == 0)
        {
            CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dataGridUI, UpdateOrInsert.Update, tableName);
        }
        _dataGridUI.rowData[0] = rowData;

        InitDg();
    }

    private void UpdateRow()
    {
        MySQLManager.Instance.UpdateOneRow(MainCrud.Instance.CurrentMainCrud.TableName,
            CSVDataHelper.ExportRowsToCSV(_dataGridUI), Hide);
    }
}
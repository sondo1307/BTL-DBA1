using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class InsertMiniDg : UpdateAndInsertMiniBase
{
    [SerializeField] protected Button _addBtn;
    [SerializeField] [ReadOnly] protected DataGridRowData _freshRowData;

    protected override void Start()
    {
        base.Start();
        _addBtn.onClick.AddListener(AddRow);
    }

    public override void Show(string tableName, DataGridRowData rowData)
    {
        base.Show(tableName, rowData);
        if (_dataGridUI.columnData.Count == 0)
        {
            CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dataGridUI, UpdateOrInsert.Insert,
                tableName);
        }

        var numberOfColumns = _dataGridUI.columnData.Count;
        for (int i = 0; i < numberOfColumns; i++)
        {
            _freshRowData.cellData.Add(new DataGridRowItemData());
        }

        _dataGridUI.rowData[0] = _freshRowData;

        InitDg();
    }

    protected virtual void AddRow()
    {
        MySQLManager.Instance.InsertOneRow(MainCrud.Instance.CurrentMainCrud.TableName,
            CSVDataHelper.ExportRowsToCSV(_dataGridUI), true, Hide);
    }

    protected override void Hide()
    {
        base.Hide();
        _freshRowData.cellData.Clear();
    }
}
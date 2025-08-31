using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

public enum UpdateOrInsert
{
    Update, // 0
    Insert // 1
}

public class UpdateAndInsertDatagridBase : MonoBehaviour
{
    [SerializeField] private Button _updateBtn;
    [SerializeField] private Button _addBtn;
    [SerializeField] private DataGridUI _dataGridUI;
    [SerializeField] private DataGridRowData _freshRowData;

    private void Start()
    {
        _updateBtn.onClick.AddListener(UpdateRow);
        _addBtn.onClick.AddListener(AddRow);
    }

    public virtual void Show(UpdateOrInsert updateOrInsert, string tableName, DataGridRowData rowData)
    {
        CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dataGridUI, tableName);
        switch (updateOrInsert)
        {
            case UpdateOrInsert.Update:
                _addBtn.gameObject.SetActive(false);
                _updateBtn.gameObject.SetActive(true);
                _dataGridUI.rowData[0] = rowData;
                break;
            case UpdateOrInsert.Insert:
                _addBtn.gameObject.SetActive(true);
                _updateBtn.gameObject.SetActive(false);
                var numberOfColumns = _dataGridUI.columnData.Count;
                for (int i = 0; i < numberOfColumns; i++)
                {
                    _freshRowData.cellData.Add(new DataGridRowItemData());
                }

                _dataGridUI.rowData[0] = _freshRowData;
                break;
        }

        // _dataGridUI.Start();
        // _dataGridUI.RowClear();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        // _dataGridUI.rowData.Clear();
        gameObject.SetActive(false);
        MainCrud.Instance.RefreshData();
    }

    private void UpdateRow()
    {
        MySQLManager.Instance.UpdateOneRow(MainCrud.Instance.CurrentMainCrud.TableName,
            CSVDataHelper.ExportRowsToCSV(_dataGridUI));
        Hide();
    }

    private void AddRow()
    {
        MySQLManager.Instance.InsertOneRow(MainCrud.Instance.CurrentMainCrud.TableName,
            CSVDataHelper.ExportRowsToCSV(_dataGridUI));
        Hide();
    }
}
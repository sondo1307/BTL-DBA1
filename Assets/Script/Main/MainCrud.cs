using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;

public class MainCrud : MonoBehaviour
{
    [SerializeField] private DataGridUI _dataGridUI;
    [SerializeField] private MainUpdateAndInsert _addDataGob;
    [SerializeField] private string _headerColumnMain = "[ID|100|Int,Tên|500|Text,Số lượng|100|Int,Giá|250|Int,Ngày nhập|300|Int]";
    [SerializeField] private string _headerColumnCRUD = "[ID|100|Int,Tên|500|InputField,Số lượng|100|InputField,Giá|250|InputField,Ngày nhập|300|InputField]";

    private void Start()
    {
        // _dataGridUI.columnData
    }

    public void ExportToCSV()
    {
        var a = CSVDataHelper.ExportToCSV(_dataGridUI);
        print(a);
    }

    public void _DeleteSelectedRow()
    {
        // Please Select At least 1 Row
        
    }

    public void _UpdateOneRow()
    {
        // Please Select Only 1 Row
        if (_dataGridUI.selectedRowUIs.Count == 0)
        {
            print("Please select 1");
            return;
        }
        else if (_dataGridUI.selectedRowUIs.Count >= 2)
        {
            print("Please select only 1 row");
            return;
        }

        var temp = _dataGridUI.GetLastSelectItem().rowData;
        _addDataGob.Show(UpdateOrInsert.Update, temp, null);
    }

    public void _ShowAddDataGob()
    {
        _addDataGob.Show(UpdateOrInsert.Insert, null, null);
    }

    public void RefreshData()
    {
        _dataGridUI.RowClear();
        _dataGridUI.Start();
    }
}

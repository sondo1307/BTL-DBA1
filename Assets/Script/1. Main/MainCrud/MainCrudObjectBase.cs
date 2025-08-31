using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using UnityEngine;

public class MainCrudObjectBase : MonoBehaviour
{
    // public string HeaderColumnMain = "[ID|100|Int,Tên|500|Text,Số lượng|100|Int,Giá|250|Int,Ngày nhập|300|Int]";
    // public string HeaderColumnCRUD = "[ID|100|Int,Tên|500|InputField,Số lượng|100|InputField,Giá|250|InputField,Ngày nhập|300|InputField]";
    [SerializeField] private InsertMiniDg _insertMiniDg;
    [SerializeField] private UpdateMiniDg _updateMiniDg;
    public DataGridUI DataGridUI;
    public string TableName;

    protected virtual void OnValidate()
    {
        DataGridUI = GetComponentInChildren<DataGridUI>();
        _insertMiniDg = GetComponentInChildren<InsertMiniDg>(true);
        _updateMiniDg = GetComponentInChildren<UpdateMiniDg>(true);
        TableName = gameObject.name;
    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        // Load Data
    }

    public async void Setup(bool isOn)
    {
        var h = MySQLManager.Instance.GetTableHeaderAsCsv(TableName);
        var h1 = StringUtils.ConvertHeaderToDataGridHeader(h);
        if (DataGridUI.columnData.Count == 0)
        {
            CSVDataHelper.CSVStringToColumnData(DataGridUI, h1);
        }

        // DataGridUI.InitializationColumn();
        var data = MySQLManager.Instance.GetTableDataAsCsv(TableName);
        CSVDataHelper.DataFromCSV(DataGridUI, false, true, true, false, data);
    }

    public void UpdateOneRow(DataGridUI dataGridUI)
    {
        switch (dataGridUI.selectedRowUIs.Count)
        {
            // Please Select Only 1 Row
            case 0:
                UIManager.Instance.ShowToast("Please select 1");
                return;
            case >= 2:
                UIManager.Instance.ShowToast("Please select only 1 row");
                return;
            default:
            {
                var rowData = dataGridUI.GetLastSelectItem().rowData;

                _updateMiniDg.Show(TableName, rowData);
                break;
            }
        }
    }

    public void DeleteSelectedRow(DataGridUI dataGridUI)
    {
        // Please Select At least 1 Row
        if (dataGridUI.selectedRowUIs.Count == 0)
        {
            UIManager.Instance.ShowToast("Please select 1");
            return;
        }

        if (dataGridUI.selectedRowUIs.Count == 1)
        {
            MySQLManager.Instance.DeleteOneRow(TableName, dataGridUI.GetLastSelectItem().rowData.cellData[0].value);
        }
        else
        {
            var selectedRows = dataGridUI.selectedRowUIs;
            List<int> selectedIds = new List<int>();
            foreach (var item in selectedRows)
            {
                selectedIds.Add(int.Parse(item.rowData.cellData[0].value));
            }
            MySQLManager.Instance.DeleteMultipleRows(TableName, selectedIds);
        }
        
        dataGridUI.RemoveSelectedItem();
    }

    public void ShowAddDataGob()
    {
        _insertMiniDg.Show(TableName, null);
    }
}
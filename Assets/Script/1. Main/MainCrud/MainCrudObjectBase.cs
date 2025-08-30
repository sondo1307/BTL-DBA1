using System;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using UnityEngine;

public class MainCrudObjectBase : MonoBehaviour
{
    // public string HeaderColumnMain = "[ID|100|Int,Tên|500|Text,Số lượng|100|Int,Giá|250|Int,Ngày nhập|300|Int]";
    // public string HeaderColumnCRUD = "[ID|100|Int,Tên|500|InputField,Số lượng|100|InputField,Giá|250|InputField,Ngày nhập|300|InputField]";
    [SerializeReference] public UpdateAndInsertDatagridBase AddDataGob;
    public DataGridUI DataGridUI;
    public string TableName;

    protected virtual void OnValidate()
    {
        DataGridUI = GetComponentInChildren<DataGridUI>();
        AddDataGob = GetComponentInChildren<UpdateAndInsertDatagridBase>(true);
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
        UIManager.Instance.ShowPermantCircle();
        var h = MySQLManager.Instance.GetTableHeaderAsCsv(TableName);
        var h1 = StringUtils.ConvertHeaderToDataGridHeader(h);
        if (DataGridUI.columnData.Count == 0)
        {
            CSVDataHelper.CSVStringToColumnData(DataGridUI, h1);
        }

        // DataGridUI.InitializationColumn();
        var data = MySQLManager.Instance.GetTableDataAsCsv(TableName);
        CSVDataHelper.DataFromCSV(DataGridUI, false, true, true, false, data);
        await Task.Delay(1000);
        UIManager.Instance.HideCircle();
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

                AddDataGob.Show(UpdateOrInsert.Update, TableName, rowData, null);
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

        dataGridUI.RemoveSelectedItem();
    }

    public void ShowAddDataGob()
    {
        AddDataGob.Show(UpdateOrInsert.Insert, null, null, null);
    }
}
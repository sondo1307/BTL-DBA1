using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using UnityEngine;

public class MainCrudObjectBase
{
    public string HeaderColumnMain = "[ID|100|Int,Tên|500|Text,Số lượng|100|Int,Giá|250|Int,Ngày nhập|300|Int]";
    public string HeaderColumnCRUD = "[ID|100|Int,Tên|500|InputField,Số lượng|100|InputField,Giá|250|InputField,Ngày nhập|300|InputField]";
    public UpdateAndInsertCauthuDataGrid AddDataGob;
    public DataGridUI DataGridUI;
    
    public void _UpdateOneRow(DataGridUI dataGridUI)
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
                var temp = dataGridUI.GetLastSelectItem().rowData;
                AddDataGob.Show(UpdateOrInsert.Update, temp, null);
                break;
            }
        }
    }

    public void _DeleteSelectedRow(DataGridUI dataGridUI)
    {
        // Please Select At least 1 Row
        if (dataGridUI.selectedRowUIs.Count == 0)
        {
            UIManager.Instance.ShowToast("Please select 1");
            return;
        }
        
        dataGridUI.RemoveSelectedItem();
    }
    
    public void _ShowAddDataGob()
    {
        AddDataGob.Show(UpdateOrInsert.Insert, null, null);
    }
}

[System.Serializable]
public class MainCrudCauThu: MainCrudObjectBase
{
    
}

[System.Serializable]
public class MainCrudDoiBong: MainCrudObjectBase
{
    
}

[System.Serializable]
public class MainCrudTrongTai: MainCrudObjectBase
{
    
}

public class MainCrud : MonoBehaviour
{
    [SerializeField] private DataGridUI _dataGridUI;
    public MainCrudCauThu CauThu;
    public MainCrudDoiBong DoiBong;
    public MainCrudTrongTai TrongTai;
    
    #region CauThu

    public void ShowMainCauThu()
    {
        CauThu.DataGridUI.gameObject.SetActive(true);
    }
    
    public void UpdateMainCauThu()
    {
        CauThu._UpdateOneRow(_dataGridUI);
    }
    
    public void DeleteMainCauThu()
    {
        CauThu._DeleteSelectedRow(_dataGridUI);
    }

    public void AddCauThu()
    {
        CauThu._ShowAddDataGob();
    }
    #endregion

    public void ExportToCSV()
    {
        var a = CSVDataHelper.ExportToCSV(_dataGridUI);
        print(a);
    }

    [ContextMenu( "Refresh Data" )]
    public async void RefreshData()
    {
        _dataGridUI.PageRowClear();
        await Task.Delay(1000);
        print(_dataGridUI.dataText);
        CSVDataHelper.DataFromCSV(_dataGridUI, false, true, true, false, _dataGridUI.dataText);
    }
}

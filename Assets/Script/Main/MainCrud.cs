using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class MainCrudObjectBase
{
    public string HeaderColumnMain = "[ID|100|Int,Tên|500|Text,Số lượng|100|Int,Giá|250|Int,Ngày nhập|300|Int]";
    public string HeaderColumnCRUD = "[ID|100|Int,Tên|500|InputField,Số lượng|100|InputField,Giá|250|InputField,Ngày nhập|300|InputField]";
    [SerializeReference]
    public UpdateAndInsertDatagridBase AddDataGob;
    public DataGridUI DataGridUI;
    
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
                var temp = dataGridUI.GetLastSelectItem().rowData;
                AddDataGob.Show(UpdateOrInsert.Update, temp, null);
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
    public MainCrudObjectBase CurrentMainCrud;
    [SerializeField] private Toggle _cauThuToggle;
    [SerializeField] private Toggle _doiBongToggle;
    [SerializeField] private Toggle _trongTaiToggle;

    private void Start()
    {
        _cauThuToggle.onValueChanged.AddListener((isOn) =>
        {
            CurrentMainCrud = CauThu;
        });
        _doiBongToggle.onValueChanged.AddListener((isOn) =>
        {
            CurrentMainCrud = DoiBong;
        });
        _trongTaiToggle.onValueChanged.AddListener((isOn) =>
        {
            CurrentMainCrud = TrongTai;
        });
    }

    public void OnAddBtnClick()
    {
        CurrentMainCrud.ShowAddDataGob();
    }
    
    public void OnUpdateBtnClick()
    {
        CurrentMainCrud.UpdateOneRow(_dataGridUI);
    }
    
    public void OnDeleteBtnClick()
    {
        CurrentMainCrud.DeleteSelectedRow(_dataGridUI);
    }

    public void ExportToCSV()
    {
        var a = CSVDataHelper.ExportToCSV(_dataGridUI);
        print(a);
    }

    [ContextMenu( "Refresh Data" )]
    public async void RefreshData()
    {
        _dataGridUI.PageRowClear();
        UIManager.Instance.ShowCircle();
        await Task.Delay(1000);
        CSVDataHelper.DataFromCSV(_dataGridUI, false, true, true, false, _dataGridUI.dataText);
    }
}

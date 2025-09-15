using System;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

public class TranDauDgObjectBase : MonoBehaviour
{
    [SerializeField] private DataGridUI _dg;
    [SerializeField] private string _name;
    [SerializeField] private Button _saveBtn;
    
    
    private void OnValidate()
    {
        _dg = GetComponentInChildren<DataGridUI>();
        _name = gameObject.name;
        _saveBtn = GetComponentInChildren<Button>();
    }

    private void Start()
    {
        _saveBtn.onClick.AddListener(OnSaveBtnClick);
    }

    public void Open(int matchID)
    {
        
        if (_dg.columnData.Count == 0)
        {
            CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dg, UpdateOrInsert.Update,
                _name);
        }

        var matchEventData = MySQLManager.Instance.GetRowByColumnValueAsCsv(_name, "", matchID);
        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, matchEventData);
    }

    private void OnSaveBtnClick()
    {
        print("Save");
        // MySQLManager.Instance.UpdateOneRow(_name, CSVDataHelper.ExportRowsToCSV(_dg), Close);
    }

    private void Close()
    {
        print("Close");
    }
}
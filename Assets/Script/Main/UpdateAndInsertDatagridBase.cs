using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

public enum UpdateOrInsert
{
    Update,   // 0
    Insert    // 1
}

public class UpdateAndInsertDatagridBase : MonoBehaviour
{
    [SerializeField] private GameObject _insertTotalGob;
    [SerializeField] private Button _insertTotalRowBtn;
    [SerializeField] private Button _updateBtn;
    [SerializeField] private Button _addBtn;
    [SerializeField] private DataGridUI _dataGridUI;
    [SerializeField] private MainCrud _mainCrud;
    [SerializeField] private DataGridRowData _freshRowData;
    
    private void Start()
    {
        _insertTotalRowBtn.onClick.AddListener(InsertRow);
        _updateBtn.onClick.AddListener(UpdateRow);
        _addBtn.onClick.AddListener(AddRow);
    }

    public virtual void Show(UpdateOrInsert updateOrInsert, Action callback)
    {
        gameObject.SetActive(true);
        switch (updateOrInsert)
        {
            case UpdateOrInsert.Update:
                _insertTotalGob.SetActive(false);
                _addBtn.gameObject.SetActive(false);
                _updateBtn.gameObject.SetActive(true);
                break;
            case UpdateOrInsert.Insert:
                _insertTotalGob.SetActive(true);
                _addBtn.gameObject.SetActive(true);
                _updateBtn.gameObject.SetActive(false);
                break;
        }
        
        callback?.Invoke();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        _mainCrud.RefreshData();
        // _dataGridUI.ClearAllRows();
        // _dataGridUI.rowData.Add(_freshRowData);
        _dataGridUI.Start();
    }
    
    private void InsertRow()
    {
        print(123);
    }

    private void UpdateRow()
    {
        
    }
    
    private void AddRow()
    {
        Hide();
    }
}

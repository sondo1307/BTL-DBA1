using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum UpdateOrInsert
{
    Update,
    Insert,
}

public class UpdateAndInsertMiniBase : MonoBehaviour
{
    [SerializeField] protected Button _closeBtn;
    [SerializeField] protected DataGridUI _dataGridUI;
    [SerializeField] private UnityEvent _closeEvent;
    [SerializeField] private UnityEvent _openEvent;
    [SerializeField] protected string _tableName;
    
    protected virtual void Start()
    {
        _closeBtn.onClick.AddListener(Hide);
    }

    public virtual void Show(string tableName, DataGridRowData rowData)
    {
        _tableName = tableName;
        _openEvent?.Invoke();
    }

    protected void InitDg()
    {
        gameObject.SetActive(true);
        _dataGridUI.InitializationRow(true);
    }

    protected virtual void Hide()
    {
        gameObject.SetActive(false);
        _closeEvent?.Invoke();
    }
}
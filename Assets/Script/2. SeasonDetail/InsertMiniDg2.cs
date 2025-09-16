using System;
using Maything.UI.DataGridUI;
using UnityEngine;

public class InsertMiniDg2 : InsertMiniDg
{
    [SerializeField] private TranDauDetailClass _tranDauDetail;

    public override void Show(string tableName, DataGridRowData rowData)
    {
        base.Show(tableName, rowData);
        _addBtn.interactable = _tranDauDetail.AllowEdit;
    }
}
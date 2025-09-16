using Maything.UI.DataGridUI;
using UnityEngine;

public class UpdateMiniDg2 : UpdateMiniDg
{
    [SerializeField] private TranDauDetailClass _tranDauDetail;

    public override void Show(string tableName, DataGridRowData rowData)
    {
        base.Show(tableName, rowData);
        _updateBtn.interactable = _tranDauDetail.AllowEdit;
    }
}
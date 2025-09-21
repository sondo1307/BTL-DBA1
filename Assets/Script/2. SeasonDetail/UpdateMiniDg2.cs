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

    protected override void UpdateRow()
    {
        MySQLManager.Instance.UpdateOneRow(_tableName,
            CSVDataHelper.ExportRowsToCSV(_dataGridUI), Hide);
    }
}
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

public class VongDauDetailClass : MonoBehaviour
{
    public GameObject VongDauDetail;
    public DataGridUI DataGridUI;
    public Button CancelButton;

    public void Open(string header)
    {
        VongDauDetail.gameObject.SetActive(true);
        CancelButton.onClick.AddListener(OnCancelBtnClick);
        if (DataGridUI.columnData.Count == 0)
        {
            var a = CSVDataHelper.CSVStringToColumnData(DataGridUI, header);
        }
    }

    private void OnCancelBtnClick()
    {
        Close();
    }

    public void Close()
    {
        VongDauDetail.gameObject.SetActive(false);
        DataGridUI.RowClear();
        CancelButton.onClick.RemoveAllListeners();
    }
}
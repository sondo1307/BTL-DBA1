using System;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EachTranDauDgObjectBase : MonoBehaviour
{
    [SerializeField] protected DataGridUI _dg;

    [FormerlySerializedAs("_name")] [SerializeField]
    protected string _tableName;

    [SerializeField] protected Button _updateBtn;
    [SerializeField] protected Button _insertBtn;
    [SerializeField] protected Button _deleteBtn;
    [SerializeField] protected InsertMiniDg2 _insertMiniDg;
    [SerializeField] protected UpdateMiniDg2 _updateMiniDg;


    private void OnValidate()
    {
        _dg = GetComponentInChildren<DataGridUI>();
        _tableName = gameObject.name;
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        _updateBtn.onClick.AddListener(OnUpdateBtnClick);
        _insertBtn.onClick.AddListener(OnInsertBtnClick);
        _deleteBtn.onClick.AddListener(OnDeleteBtnClick);
        var headerGob = transform.Find("header");
        headerGob.GetComponent<TMP_Text>().text = _tableName;
    }

    public virtual void Open(int matchID)
    {
        print("open1");
        if (_dg.columnData.Count == 0)
        {
            CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dg, UpdateOrInsert.Update,
                _tableName);
        }

        var data = MySQLManager.Instance.GetRowsByColumnValueAsCsv(_tableName, "match_id", matchID.ToString());
        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, data);

        _insertBtn.interactable = Main_SeasonDetail.Instance.tranDauDetailClass.AllowEdit;
        _updateBtn.interactable = Main_SeasonDetail.Instance.tranDauDetailClass.AllowEdit;
    }

    private void OnUpdateBtnClick()
    {
        switch (_dg.selectedRowUIs.Count)
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
                var rowData = _dg.GetLastSelectItem().rowData;

                _updateMiniDg.Show(_tableName, rowData);
                break;
            }
        }
    }

    private void OnInsertBtnClick()
    {
        _insertMiniDg.Show(_tableName, null);
    }

    private void OnDeleteBtnClick()
    {
        if (_dg.selectedRowUIs.Count == 0)
        {
            UIManager.Instance.ShowToast("Please select 1");
            return;
        }

        if (_dg.selectedRowUIs.Count == 1)
        {
            MySQLManager.Instance.DeleteOneRow(_tableName, _dg.GetLastSelectItem().rowData.cellData[0].value);
        }
        else
        {
            var selectedRows = _dg.selectedRowUIs;
            List<int> selectedIds = new List<int>();
            foreach (var item in selectedRows)
            {
                selectedIds.Add(int.Parse(item.rowData.cellData[0].value));
            }

            MySQLManager.Instance.DeleteMultipleRows(_tableName, selectedIds);
        }

        _dg.RemoveSelectedItem();
    }
}
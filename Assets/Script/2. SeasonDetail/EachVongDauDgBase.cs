using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.Serialization;

public class EachVongDauDgBase : MonoBehaviour
{
    [SerializeField] protected DataGridUI _dg;

    [SerializeField] protected string _tableName;

    private void OnValidate()
    {
        _dg = GetComponentInChildren<DataGridUI>();
        _tableName = gameObject.name;
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public virtual void Open(int tourId)
    {
        print("open1");
        if (_dg.columnData.Count == 0)
        {
            CSVDataHelper.GetTableHeaderAndSetToDG(_dg, _tableName);
        }

        var matches =
            MySQLManager.Instance.GetCellsDataListByRowId(SonConst.PrematchTable, "match_id", "tournament_round",
                tourId.ToString());
        print(StringUtils.ConvertListToCsv(matches));

        var data = MySQLManager.Instance.GetRowsByColumnValuesAsCsv(_tableName, "match_id", matches);
        print(data);
        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, data);
    }
}
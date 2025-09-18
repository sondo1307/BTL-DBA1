using Maything.UI.DataGridUI;
using UnityEngine;

    public class EachTranDauDgPlayerLineup : EachTranDauDgObjectBase
    {
        public override void Open(int matchID)
        {
            var a = MySQLManager.Instance.GetMatchPlayerLineupWithTeamIdAsCsv();
            print(a);

            // if (_dg.columnData.Count == 0)
            // {
            //     var h = MySQLManager.Instance.GetMatchPlayerLineupHeaderCsv();
            //     var h1 = StringUtils.ConvertHeaderToDataGridHeader(h);
            //     var a = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            //     CSVDataHelper.CSVStringToColumnData(_dg, a);
            // }
            //
            // var data = MySQLManager.Instance.GetMatchPlayerLineupDataCsv();
            // print(data);
            // CSVDataHelper.DataFromCSV(_dg, false, true, true, false, data);
            //
            // _insertBtn.interactable = Main_SeasonDetail.Instance.tranDauDetailClass.AllowEdit;
            // _updateBtn.interactable = Main_SeasonDetail.Instance.tranDauDetailClass.AllowEdit;
        }
    }

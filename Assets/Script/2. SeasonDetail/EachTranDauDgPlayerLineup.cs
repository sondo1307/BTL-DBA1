using Maything.UI.DataGridUI;
using UnityEngine;

    public class EachTranDauDgPlayerLineup : EachTranDauDgObjectBase
    {
        public override void Open(int matchID)
        {
            base.Open(matchID);
            
            // var a = MySQLManager.Instance.GetMatchPlayerLineupWithTeamIdAsCsv();
            // print(a);

            // if (_dg.columnData.Count == 0)
            // {
            //   CSVDataHelper.GetTableHeaderAndSetToDG(_dg, _tableName);
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

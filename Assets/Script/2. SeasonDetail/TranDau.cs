using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Maything.UI.DataGridUI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TranDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _tenCapDau;
    [SerializeField] private TMP_Text _ngayThiDau;
    [SerializeField] private TMP_Text _tiSo;
    public string Team1 { get; private set; }
    public int Team1ID { get; private set; }
    public string Team2 { get; private set; }
    public int Team2ID { get; private set; }
    public string NgayDau { get; private set; }

    private Image _img;

    [FormerlySerializedAs("_matchDb")] [SerializeField]
    private PrematchDB prematchDB;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
        _img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        Main_SeasonDetail.Instance.EventUpdateTodayDate += UpdateDate;
    }

    private void OnDisable()
    {
        Main_SeasonDetail.Instance.EventUpdateTodayDate -= UpdateDate;
    }

    public void SetCapDau(PrematchDB prematchDB)
    {
        this.prematchDB = prematchDB;
        Team1 = GetTeamName(this.prematchDB.home_team_id);
        Team2 = GetTeamName(this.prematchDB.away_team_id);
        Team1ID = this.prematchDB.home_team_id;
        Team2ID = this.prematchDB.away_team_id;
        _tenCapDau.text = Team1ID + "." + Team1 + "\n" +
                          Team2ID + "." + Team2;
        _tiSo.text = "0" + "\n" + "0";
        _ngayThiDau.text = this.prematchDB.match_date;
        // MySQLManager.Instance.InsertOneRow(SonConst.PrematchTable, prematchDB.ConvertToCsv(), null);
    }

    private string GetTeamName(int teamID)
    {
        return MySQLManager.Instance.GetCellDataByRowId(SonConst.TeamTable, "team_name", "team_id", teamID);
    }

    public void SetTiSo(int team1, int team2)
    {
        _tiSo.text = team1 + " - " + team2;
    }

    public void OnBtnClick()
    {
        Main_SeasonDetail.Instance.tranDauDetailClass.Open(prematchDB.match_id);
        // CSVDataHelper.DataFromCSV(Main_SeasonDetail.Instance.tranDauDetailClass.MatchEvent, false, true, false, false,
        // "1,\"Bút bi\",100,5000,110\n2,\"Vở học sinh\",50,12000,120\n3,\"Thước kẻ\",80,8000,30\n4,\"Bút chì\",120,4000,40\n5,\"Tẩy\",60,3000,50");
    }

    private void UpdateDate(string date)
    {
        var myDate = DateTime.ParseExact(_ngayThiDau.text, SonConst.DateFormat, CultureInfo.InvariantCulture);
        var today = DateTime.ParseExact(date, SonConst.DateFormat, CultureInfo.InvariantCulture);
        _img.color = myDate.Date < today.Date ? Color.red : Color.white;
    }

    // Hủy Team hoặc Quá ngày đá mà không đá
    // 0-0, khong trong tai, khong the do, khong the vang, 
    private void CancelTranDau()
    {
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Maything.UI.DataGridUI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [SerializeField] private PrematchDB prematchDB;
    [SerializeField] private InmatchDB _inmatchDB;
    [SerializeField] private PostmatchDB _postmatchDB;
    [SerializeField] private MatchRefereeLineupDB _matchRefereeLineupDB;
    [SerializeField] private MatchPlayerLineupDB _matchPlayerLineupDB;


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

        RandomRefereeLineup();
        RandomPlayerLineup();
        RandomInMatch();
        RandomPostMatch();
    }


    // pre - in - post - ref - player
    private void RandomRefereeLineup()
    {
        var listRef = MySQLManager.Instance.GetValuesByColumn(SonConst.RefereeTable, "referee_id");
        var a = new MatchRefereeLineupDB(prematchDB.match_id, GetOneRef(), GetOneRef(), GetOneRef(), GetOneRef());
        var b = a.ConvertToCsv();
        print(b);
        // Main_SeasonDetail.Instance.tranDauDetailClass.Dgs[^2].
        return;

        int GetOneRef()
        {
            var index = UnityEngine.Random.Range(0, listRef.Count);
            var refID = int.Parse(listRef[index]);
            listRef.RemoveAt(index);
            return refID;
        }
    }

    private void RandomPlayerLineup()
    {
    }

    private void RandomInMatch()
    {
        // var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), 0,);
    }

    private void RandomPostMatch()
    {
    }

    private string GetTeamName(int teamID)
    {
        return MySQLManager.Instance.GetCellDataByRowId(SonConst.TeamTable, "team_name", "team_id", teamID);
    }

    private void OnBtnClick()
    {
        Main_SeasonDetail.Instance.tranDauDetailClass.Open(prematchDB.match_id);
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
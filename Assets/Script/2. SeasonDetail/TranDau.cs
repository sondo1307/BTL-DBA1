using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
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

    [SerializeField] [ReadOnly] private PrematchDB prematchDB;
    [SerializeField] [ReadOnly] private List<string> _listPlayerInMatch = new List<string>();

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
        _img = GetComponent<Image>();
    }

    private void Start()
    {
        Main_SeasonDetail.Instance.EventUpdateTodayDate += UpdateDate;
        Main_SeasonDetail.Instance.tranDauDetailClass.EventUpdateMatchID += UpdateUi;
    }

    private void OnDestroy()
    {
        Main_SeasonDetail.Instance.EventUpdateTodayDate -= UpdateDate;
        Main_SeasonDetail.Instance.tranDauDetailClass.EventUpdateMatchID -= UpdateUi;
    }

    private void UpdateUi(int matchID)
    {
        if (matchID != prematchDB.match_id)
        {
            return;
        }
        var homeScore =
            MySQLManager.Instance.GetCellDataByRowId(SonConst.PostMatchTable, "home_score", "match_id",
                prematchDB.match_id);
        var awayScore = MySQLManager.Instance.GetCellDataByRowId(SonConst.PostMatchTable, "away_score", "match_id",
            prematchDB.match_id);
        _tiSo.text = homeScore + "\n" + awayScore;
    }

    public void SetCapDau(PrematchDB prematchDB, bool insertData)
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
        if (insertData)
        {
            MySQLManager.Instance.InsertOneRow(SonConst.PrematchTable, prematchDB.ConvertToCsv(), false, null);
            StartCoroutine(Delay());
        }

        return;

        IEnumerator Delay()
        {
            RandomPlayerLineup();
            yield return SonCache.WaitSeconds;
            RandomInMatch();
            yield return SonCache.WaitSeconds;
            RandomPostMatch();
        }
    }


    private void RandomPlayerLineup()
    {
        var homePlayers =
            MySQLManager.Instance.GetValuesByCondition(SonConst.TeamPlayerTable, "player_id",
                "team_id", prematchDB.home_team_id);
        // foreach (var i in homePlayers)
        // {
        //     print(i);
        // }
        // print(StringUtils.ConvertListToCsv(homePlayers));

        _listPlayerInMatch.AddRange(homePlayers);
        //Home team
        for (var i = 0; i < 11; i++)
        {
            var a = new MatchPlayerLineupDB(prematchDB.match_id, GetOneHomePlayer(), true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 4; i++)
        {
            var a = new MatchPlayerLineupDB(prematchDB.match_id, GetOneHomePlayer(), false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        //Away team
        var awayPlayers =
            MySQLManager.Instance.GetValuesByCondition(SonConst.TeamPlayerTable, "player_id",
                "team_id", prematchDB.away_team_id);
        // foreach (var i in awayPlayers)
        // {
        //     print(i);
        // }
        // print(StringUtils.ConvertListToCsv(awayPlayers));

        _listPlayerInMatch.AddRange(awayPlayers);
        for (var i = 0; i < 11; i++)
        {
            var a = new MatchPlayerLineupDB(prematchDB.match_id, GetOneAwayPlayer(), true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 4; i++)
        {
            var a = new MatchPlayerLineupDB(prematchDB.match_id, GetOneAwayPlayer(), false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        return;

        int GetOneHomePlayer()
        {
            // var index = UnityEngine.Random.Range(homePlayers.Select(int.Parse).Min(),
            //     homePlayers.Select(int.Parse).Max() + 1);
            var index = UnityEngine.Random.Range(0, homePlayers.Count);
            var playerID = int.Parse(homePlayers[index]);
            homePlayers.RemoveAt(index);
            return playerID;
        }

        int GetOneAwayPlayer()
        {
            // var index = UnityEngine.Random.Range(awayPlayers.Select(int.Parse).Min(),
            //     awayPlayers.Select(int.Parse).Max() + 1);
            var index = UnityEngine.Random.Range(0, awayPlayers.Count);
            var playerID = int.Parse(awayPlayers[index]);
            awayPlayers.RemoveAt(index);
            return playerID;
        }
    }

    private void RandomInMatch()
    {
        var randGoal = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < randGoal; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.Goal,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }

        var randomYellow = UnityEngine.Random.Range(0, 3);
        for (int i = 0; i < randomYellow; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.YellowCard,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }

        var randomRed = UnityEngine.Random.Range(0, 3);
        for (int i = 0; i < randomRed; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.RedCard,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }
    }

    private void RandomPostMatch()
    {
        // _postmatchDB = new PostmatchDB(prematchDB.match_id, 3, 3, Random.Range(10, 90), Random.Range(91, 110));
        // var b = _postmatchDB.ConvertToCsv();
        // MySQLManager.Instance.InsertOneRow(SonConst.PostMatchTable, b, false, null);
        MySQLManager.Instance.CallSumaryPostMatch(prematchDB.match_id);
    }

    private string GetTeamName(int teamID)
    {
        return MySQLManager.Instance.GetCellDataByRowId(SonConst.TeamTable, "team_name", "team_id", teamID);
    }

    private void OnBtnClick()
    {
        if (!Main_SeasonDetail.Instance.DatePickerHasValue)
        {
            UIManager.Instance.ShowToast("Please select today date");
            return;
        }
        Main_SeasonDetail.Instance.tranDauDetailClass.Open(prematchDB.match_id, prematchDB.tournament_round,
            (_img.color == Color.yellow));
    }

    private void UpdateDate(string date)
    {
        var myDate = DateTime.ParseExact(_ngayThiDau.text, SonConst.DateFormat, CultureInfo.InvariantCulture);
        var today = DateTime.ParseExact(date, SonConst.DateFormat, CultureInfo.InvariantCulture);

        var postMatch =
            MySQLManager.Instance.GetCellDataByRowId(SonConst.PostMatchTable, "match_id", "match_id",
                prematchDB.match_id);

        // done
        if (!string.IsNullOrEmpty(postMatch))
        {
            _img.color = Color.green;
        }
        else
        {
            // not done and date < today
            if (myDate < today)
            {
                _img.color = Color.red;
            }
            // not done and date > today
            else if (myDate > today)
            {
                _img.color = Color.white;
            }
            // not done and date = today
            else if (myDate == today)
            {
                _img.color = Color.yellow;
            }
        }
    }

    // Hủy Team hoặc Quá ngày đá mà không đá
    // 0-0, khong trong tai, khong the do, khong the vang, 
    private void CancelTranDau()
    {
    }
}
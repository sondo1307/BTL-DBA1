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
    [SerializeField] [ReadOnly] private List<string> _listHomePlayersInMatch = new List<string>();
    [SerializeField] [ReadOnly] private List<string> _listAwayPlayersInMatch = new List<string>();
    [SerializeField] [ReadOnly] private List<string> _listPlayerInMatch = new List<string>();
    [SerializeField] private List<string> thuMonHomePlayers;
    [SerializeField] private List<string> hauVeHomePlayers;
    [SerializeField] private List<string> tienVeHomePlayers;
    [SerializeField] private List<string> tienDaoHomePlayers;
    [SerializeField] private List<string> thuMonAwayPlayers;
    [SerializeField] private List<string> hauVeAwayPlayers;
    [SerializeField] private List<string> tienVeAwayPlayers;
    [SerializeField] private List<string> tienDaoAwayPlayers;

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
                prematchDB.match_id.ToString());
        var awayScore = MySQLManager.Instance.GetCellDataByRowId(SonConst.PostMatchTable, "away_score", "match_id",
            prematchDB.match_id.ToString());
        _tiSo.text = homeScore + "\n" + awayScore;
        
        if (string.IsNullOrEmpty(homeScore) || string.IsNullOrEmpty(awayScore))
        {
            return;
        }
        _tiSo.text = homeScore + "\n" + awayScore;
    }

    public void SetCapDau(PrematchDB prematchDB, bool insertData, bool isInsertInMatch)
    {
        this.prematchDB = prematchDB;
        Team1 = GetTeamName(this.prematchDB.home_team_id);
        Team2 = GetTeamName(this.prematchDB.away_team_id);
        Team1ID = this.prematchDB.home_team_id;
        Team2ID = this.prematchDB.away_team_id;
        _tenCapDau.text = Team1ID + "." + Team1 + "\n" +
                          Team2ID + "." + Team2;
        // _tiSo.text = "0" + "\n" + "0";
        NgayDau = this.prematchDB.match_date;
        _ngayThiDau.text = NgayDau;
        if (insertData)
        {
            MySQLManager.Instance.InsertOneRow(SonConst.PrematchTable, prematchDB.ConvertToCsv(), false, null);
            StartCoroutine(Delay());
        }
        else
        {
            UpdateUi(this.prematchDB.match_id);
        }

        // UpdateDate(Main_SeasonDetail.Instance.DatePicker.SelectedDate.Date.ToString(SonConst.DateFormat,
        // CultureInfo.InvariantCulture));
        return;

        IEnumerator Delay()
        {
            RandomPlayerLineup();
            yield return SonCache.WaitForEndOfFrame;
            if (isInsertInMatch)
            {
                RandomInMatch();
            }
        }
    }


    private void RandomPlayerLineup()
    {
        thuMonHomePlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.home_team_id, "Thủ môn");
        hauVeHomePlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.home_team_id, "Hậu vệ");
        tienVeHomePlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.home_team_id, "Tiền vệ");
        tienDaoHomePlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.home_team_id, "Tiền đạo");

        print("thu mon" + " home " + prematchDB.home_team_id + " /" + StringUtils.ConvertListToCsv(thuMonHomePlayers));
        print("hau ve" + " home " + prematchDB.home_team_id + " /" + StringUtils.ConvertListToCsv(hauVeHomePlayers));
        print("tien ve" + " home " + prematchDB.home_team_id + "/ " + StringUtils.ConvertListToCsv(tienVeHomePlayers));
        print("tien dao" + " home " + prematchDB.home_team_id + " /" +
              StringUtils.ConvertListToCsv(tienDaoHomePlayers));
        _listHomePlayersInMatch.AddRange(thuMonHomePlayers);
        _listHomePlayersInMatch.AddRange(hauVeHomePlayers);
        _listHomePlayersInMatch.AddRange(tienVeHomePlayers);
        _listHomePlayersInMatch.AddRange(tienDaoHomePlayers);

        for (int i = 0; i < 1; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(thuMonHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 4; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(hauVeHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 3; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(tienVeHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 3; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(tienDaoHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        // DU BI
        for (int i = 0; i < 1; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(thuMonHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(hauVeHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(tienVeHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomHomePlayer(tienDaoHomePlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        thuMonAwayPlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.away_team_id, "Thủ môn");
        hauVeAwayPlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.away_team_id, "Hậu vệ");
        tienVeAwayPlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.away_team_id, "Tiền vệ");
        tienDaoAwayPlayers =
            MySQLManager.Instance.GetTeamPlayerIds(prematchDB.away_team_id, "Tiền đạo");


        print("thu mon" + " away " + prematchDB.home_team_id + " /" + StringUtils.ConvertListToCsv(thuMonAwayPlayers));
        print("hau ve" + " away " + prematchDB.home_team_id + " /" + StringUtils.ConvertListToCsv(hauVeAwayPlayers));
        print("tien ve" + " away " + prematchDB.home_team_id + " /" + StringUtils.ConvertListToCsv(tienVeAwayPlayers));
        print("tien dao" + " away " + prematchDB.home_team_id + "/ " +
              StringUtils.ConvertListToCsv(tienDaoAwayPlayers));
        _listAwayPlayersInMatch.AddRange(thuMonAwayPlayers);
        _listAwayPlayersInMatch.AddRange(hauVeAwayPlayers);
        _listAwayPlayersInMatch.AddRange(tienVeAwayPlayers);
        _listAwayPlayersInMatch.AddRange(tienDaoAwayPlayers);

        for (int i = 0; i < 1; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(thuMonAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 4; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(hauVeAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 3; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(tienVeAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 3; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(tienDaoAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, true);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        // DU BI
        for (int i = 0; i < 1; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(thuMonAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(hauVeAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(tienVeAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        for (int i = 0; i < 2; i++)
        {
            var randTeamPlayerId = RandomAwayPlayer(tienDaoAwayPlayers);
            _listPlayerInMatch.Add(randTeamPlayerId.ToString());
            var a = new MatchPlayerLineupDB(prematchDB.match_id, randTeamPlayerId, false);
            var b = a.ConvertToCsv();
            MySQLManager.Instance.InsertOneRow(SonConst.MatchPlayerLineupTable, b, false, null);
        }

        return;

        int RandomHomePlayer(List<string> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            var playerID = 0;
            try
            {
                playerID = int.Parse(list[index]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            list.RemoveAt(index);
            return playerID;
        }

        int RandomAwayPlayer(List<string> list)
        {
            var index = UnityEngine.Random.Range(0, list.Count);
            var playerID = 0;
            try
            {
                playerID = int.Parse(list[index]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            list.RemoveAt(index);
            return playerID;
        }
    }

    private void RandomInMatch()
    {
        var randGoal = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < randGoal; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.Goal,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }

        var randomYellow = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < randomYellow; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.YellowCard,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }

        var randomRed = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < randomRed; i++)
        {
            var a = new InmatchDB(0, prematchDB.match_id, Random.Range(10, 90), (int)EventTypeInMatch.RedCard,
                int.Parse(_listPlayerInMatch[Random.Range(0, _listPlayerInMatch.Count)]));
            var b = a.ConvertToCsv();
            // print(b);
            MySQLManager.Instance.InsertOneRow(SonConst.InMatchTable, b, true, null);
        }

        StartCoroutine(Delay());
        return;

        IEnumerator Delay()
        {
            yield return SonCache.WaitForEndOfFrame;
            RandomPostMatch();
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
        return MySQLManager.Instance.GetCellDataByRowId(SonConst.TeamTable, "team_name", "team_id", teamID.ToString());
    }

    private void OnBtnClick()
    {
        if (!Main_SeasonDetail.Instance.DatePickerHasValue)
        {
            UIManager.Instance.ShowToast("Please select today date");
            return;
        }

        var matchActiveS = MySQLManager.Instance.GetCellDataByRowId(SonConst.PrematchTable, "is_actived", "match_id",
            prematchDB.match_id.ToString());
        var matchActive = int.Parse(matchActiveS);

        Main_SeasonDetail.Instance.tranDauDetailClass.Open(prematchDB.match_id, prematchDB.tournament_round,
            _img.color == Color.yellow && matchActive == 1);
    }

    private void UpdateDate(string date)
    {
        var myDate = DateTime.ParseExact(_ngayThiDau.text, SonConst.DateFormat, CultureInfo.InvariantCulture);
        var today = DateTime.ParseExact(date, SonConst.DateFormat, CultureInfo.InvariantCulture);

        var postMatch =
            MySQLManager.Instance.GetCellDataByRowId(SonConst.PostMatchTable, "match_id", "match_id",
                prematchDB.match_id.ToString());

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
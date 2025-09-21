using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using Sirenix.OdinInspector;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[System.Serializable]
public class PrematchDB
{
    public int match_id;
    public int home_team_id;
    public int away_team_id;
    public int stadium_id;
    public string match_date;
    public int ticket_price;
    public int tournament_round;
    [FormerlySerializedAs("is_active")] public bool is_actived;

    public PrematchDB(int matchID, int homeTeamID, int awayTeamID, int stadiumID, string matchDate,
        int ticketPrice, int tournamentRound, bool isActived)
    {
        match_id = matchID;
        home_team_id = homeTeamID;
        away_team_id = awayTeamID;
        stadium_id = stadiumID;
        match_date = matchDate;
        ticket_price = ticketPrice;
        tournament_round = tournamentRound;
        is_actived = isActived;
    }

    public string ConvertToCsv()
    {
        return
            $"{match_id},{home_team_id},{away_team_id},{stadium_id},{match_date},{ticket_price},{tournament_round},{(is_actived ? "1" : "0")}";
    }
}

public enum EventTypeInMatch
{
    Goal = 0,
    YellowCard = 1,
    RedCard = 2,
}

[System.Serializable]
public class InmatchDB
{
    public int event_id;
    public int match_id;
    public int minute;
    public int event_type;
    public int team_player_id;

    public InmatchDB(int eventID, int matchID, int minute, int eventType, int teamPlayerID)
    {
        event_id = eventID;
        match_id = matchID;
        this.minute = minute;
        event_type = eventType;
        team_player_id = teamPlayerID;
    }

    public string ConvertToCsv()
    {
        return $"{event_id},{match_id},{minute},{event_type},{team_player_id}";
    }
}

[System.Serializable]
public class PostmatchDB
{
    public int match_id;
    public int home_score;
    public int away_score;
    public int attendance;
    public int total_time;

    public PostmatchDB(int matchID, int homeScore, int awayScore, int attendance, int totalTime)
    {
        match_id = matchID;
        home_score = homeScore;
        away_score = awayScore;
        this.attendance = attendance;
        total_time = totalTime;
    }

    public string ConvertToCsv()
    {
        return $"{match_id},{home_score},{away_score},{attendance},{total_time}";
    }
}

[System.Serializable]
public class MatchPlayerLineupDB
{
    public int match_id;
    public int team_player_id;
    public bool is_starting;

    public MatchPlayerLineupDB(int matchID, int teamPlayerID, bool isStarting)
    {
        match_id = matchID;
        team_player_id = teamPlayerID;
        is_starting = isStarting;
    }

    public string ConvertToCsv()
    {
        return $"{match_id},{team_player_id},{(is_starting ? "1" : "0")}";
    }
}

[System.Serializable]
public class MatchRefereeLineupDB
{
    public int match_id;
    public int referee_main_id;
    public int referee_assist_1_id;
    public int referee_assist_2_id;
    public int referee_var_id;

    public MatchRefereeLineupDB(int matchID, int refereeMainID, int refereeAssist1ID, int refereeAssist2ID,
        int refereeVarID)
    {
        match_id = matchID;
        referee_main_id = refereeMainID;
        referee_assist_1_id = refereeAssist1ID;
        referee_assist_2_id = refereeAssist2ID;
        referee_var_id = refereeVarID;
    }

    public string ConvertToCsv()
    {
        return $"{match_id},{referee_main_id},{referee_assist_1_id},{referee_assist_2_id},{referee_var_id}";
    }
}


[System.Serializable]
public class Team
{
    public int team_id;
    public string team_name;
    public DateTime team_created;
    public int country_id;
    public int stadium_id;
    public string home_kit;
    public string away_kit;
    public string third_kit;
    [FormerlySerializedAs("isActived")] public int is_actived;

    public Team(int teamID, string teamName, DateTime teamCreated, int countryID, int stadiumID, string homeKit,
        string awayKit, string thirdKit, int isActived)
    {
        team_id = teamID;
        team_name = teamName;
        team_created = teamCreated;
        country_id = countryID;
        stadium_id = stadiumID;
        home_kit = homeKit;
        away_kit = awayKit;
        third_kit = thirdKit;
        this.is_actived = isActived;
    }
}

public class Main_SeasonDetail : MonoBehaviour
{
    public static Main_SeasonDetail Instance { get; set; }
    [SerializeField] private Button _taoGiaiDauBtn;
    [SerializeField] private Button _xoaBtn;
    [SerializeField] private Transform _content;
    [SerializeField] private VongDau _vongDauPrefab;
    [SerializeField] private List<VongDau> _vongDaus = new List<VongDau>();
    public List<VongDau> VongDaus => _vongDaus;
    // [SerializeField] private TMP_InputField _soLuongMatchRenderInMatch;

    [SerializeField] private InputField _thanhTimKiem;

    [Header("DatePicker"), Space(10)] [SerializeField]
    private DatePicker _datePicker;

    public DatePicker DatePicker => _datePicker;

    public bool DatePickerHasValue => (_datePicker.SelectedDate.HasValue);

    [Header("TranDauDetail"), Space(10)] public TranDauDetailClass tranDauDetailClass;

    [Header("VongDauDetail"), Space(10)] public VongDauDetailClass vongDauDetailClass;

    public Action<string> EventUpdateTodayDate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (MySQLManager.Instance.IsTableEmpty(SonConst.PrematchTable))
        {
            // Tao giai dau
            _taoGiaiDauBtn.gameObject.SetActive(true);
        }
        else
        {
            // Load Giai dau
            _taoGiaiDauBtn.gameObject.SetActive(false);
            LoadGiaiDau();
        }
    }

    public void LoadGiaiDau()
    {
        // Lấy toàn bộ dữ liệu trận đấu từ DB
        var rows = MySQLManager.Instance.GetAllRowsAsList(SonConst.PrematchTable);
        if (rows.Count == 0) return;

        // Parse từng row thành MatchDb
        List<PrematchDB> matches = new List<PrematchDB>();
        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            try
            {
                var m = new PrematchDB(
                    int.Parse(row[0]), // match_id
                    int.Parse(row[1]), // home_team_id
                    int.Parse(row[2]), // away_team_id
                    int.Parse(row[3]), // stadium_id
                    row[4], // match_date
                    Convert.ToInt32(double.Parse(row[5])), // ticket_price
                    int.Parse(row[6]), // tournament_round
                    Convert.ToBoolean(int.Parse(row[7])) // is_active
                );
                matches.Add(m);
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Parse match row error: " + e.Message);
            }
        }

        StartCoroutine(TaoGiaiDau());
        return;

        IEnumerator TaoGiaiDau()
        {
            UIManager.Instance.ShowPermantCircle();

            // Nhóm theo vòng đấu
            var grouped = matches.GroupBy(m => m.tournament_round).OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                VongDau vongDau = Instantiate(_vongDauPrefab, _content);
                foreach (var match in group)
                {
                    vongDau.AddTranDau(match, false, false);
                    yield return SonCache.WaitForEndOfFrame;
                }

                _vongDaus.Add(vongDau);
                vongDau.SetVongDau(group.Key);
                // yield return SonCache.WaitForEndOfFrame;
            }

            UIManager.Instance.HideCircle();
        }
    }

    List<List<PrematchDB>> GenerateRoundsAndMatches()
    {
        var rowsTeam = MySQLManager.Instance.GetAllRowsAsList("team");
        // TODO: xoa de quay ve render 8 doi
        var a1 = rowsTeam[0];
        var b1 = rowsTeam[1];
        var b2 = rowsTeam[2];
        var b4 = rowsTeam[3];
        rowsTeam.Clear();
        rowsTeam.Add(a1);
        rowsTeam.Add(b1);
        rowsTeam.Add(b2);
        rowsTeam.Add(b4);
        int matchIdCount = 1;

        // Mảng làm việc: giữ arr[0] cố định, xoay các phần tử 1..n-1
        var arr = new List<Team>();
        var rounds = new List<List<PrematchDB>>();
        var n = rowsTeam.Count;

        foreach (var rowTeamData in rowsTeam)
        {
            var a = new Team(int.Parse(rowTeamData[0]), rowTeamData[1],
                DateTime.ParseExact(rowTeamData[2], SonConst.DateFormat, CultureInfo.InvariantCulture),
                int.Parse(rowTeamData[3]),
                int.Parse(rowTeamData[4]),
                rowTeamData[5], rowTeamData[6], rowTeamData[7], int.Parse(rowTeamData[8]));
            arr.Add(a);
        }

        DateTime matchDate = DateTime.Now;

        // Lượt đi: n-1 vòng
        for (int r = 0; r < n - 1; r++)
        {
            var matches = new List<PrematchDB>();
            for (int i = 0; i < n / 2; i++)
            {
                var home = arr[i];
                var away = arr[n - 1 - i];
                var m = new PrematchDB(matchIdCount, home.team_id, away.team_id, home.stadium_id,
                    matchDate.ToString(SonConst.DateFormat)
                    , RandomTicketPrice(),
                    r + 1, true);
                // matchDate = matchDate.AddDays(1);
                matchIdCount++;
                matches.Add(m);
            }

            rounds.Add(matches);

            matchDate = matchDate.AddDays(1);

            // Xoay vòng: giữ arr[0], dịch phải đoạn [1..n-1]
            Team last = arr[n - 1];
            for (int i = n - 1; i >= 2; i--)
                arr[i] = arr[i - 1];
            arr[1] = last;
        }

        // Lượt về: đảo sân của từng cặp theo đúng thứ tự vòng
        int total = rounds.Count;
        for (int i = 0; i < total; i++)
        {
            var ret = new List<PrematchDB>();
            foreach (var m in rounds[i])
            {
                var rematch = new PrematchDB(
                    matchIdCount,
                    m.away_team_id, // đảo sân
                    m.home_team_id,
                    m.stadium_id,
                    matchDate.ToString(SonConst.DateFormat),
                    m.ticket_price,
                    m.tournament_round + total, // tăng số vòng để phân biệt
                    m.is_actived
                );

                ret.Add(rematch);
                matchIdCount++;

                // tăng ngày ngay sau khi gán trận
                // matchDate = matchDate.AddDays(1);
            }

            matchDate = matchDate.AddDays(1);
            rounds.Add(ret);
        }

        return rounds;
    }


    /// <summary>
    /// SEPERATOR
    /// </summary>
    /// <returns></returns>
    [Button]
    public void OnTaoGiaiDauClick()
    {
        if (!MySQLManager.Instance.ValidateTeamInSession1())
        {
            UIManager.Instance.ShowToast("Chưa đủ điểu kiện để tạo giải đấu");
            return;
        }

        // if (!_datePicker.SelectedDate.HasValue)
        // {
        //     UIManager.Instance.ShowToast("Hãy nhập ngày hiện tại của giải đấu");
        //     return;
        // }

        DateTime today = DateTime.Now.AddDays(-20);
        if (_datePicker.SelectedDate.HasValue)
        {
            today = _datePicker.SelectedDate.Date;
        }

        _taoGiaiDauBtn.gameObject.SetActive(false);
        StartCoroutine(TaoGiaiDau());
        return;

        IEnumerator TaoGiaiDau()
        {
            UIManager.Instance.ShowPermantCircle();
            List<List<PrematchDB>> rounds = GenerateRoundsAndMatches();

            var countInMatchRender = 0;

            // In lịch thi đấu
            int round = 1;
            for (var i = 0; i < rounds.Count; i++)
            {
                var listRef = RandomRefereeLineup();
                var lMatchesDB = rounds[i];
                VongDau vongDau = Instantiate(_vongDauPrefab, _content);

                foreach (var match in lMatchesDB)
                {
                    vongDau.AddTranDau(match, true,
                        (!_datePicker.SelectedDate.HasValue || DateTime.Parse(match.match_date) <= today));
                    // vongDau.AddTranDau(match, true, true);

                    var listFourRef = GetFourRef();
                    var a = new MatchRefereeLineupDB(match.match_id, listFourRef[0], listFourRef[1], listFourRef[2],
                        listFourRef[3]);
                    var b = a.ConvertToCsv();
                    // print(b);
                    MySQLManager.Instance.InsertOneRow(SonConst.MatchRefereeLineupTable, b, false, null);
                    countInMatchRender++;
                }

                _vongDaus.Add(vongDau);
                vongDau.SetVongDau(round);

                round++;
                yield return SonCache.WaitForEndOfFrame;
                continue;

                List<int> GetFourRef()
                {
                    var tempRef = new List<int>();
                    for (var i = 0; i < 4; i++)
                    {
                        var index = UnityEngine.Random.Range(0, listRef.Count);
                        var refID = int.Parse(listRef[index]);
                        listRef.RemoveAt(index);
                        tempRef.Add(refID);
                    }

                    return tempRef;
                }
            }

            UIManager.Instance.HideCircle();
        }
    }

    private List<string> RandomRefereeLineup()
    {
        var a = MySQLManager.Instance.GetValuesByColumn(SonConst.RefereeTable, "referee_id");
        var b = new List<string>();
        for (var i = 0; i < 16; i++)
        {
            var s = a[i];
            b.Add(s);
        }

        return b;
    }

    private int RandomTicketPrice()
    {
        return UnityEngine.Random.Range(0, 11) * 10000 + 100000;
    }

    public void OnXoaGiaiDauClick()
    {
        foreach (var vongDau in _vongDaus)
        {
            vongDau.TranDaus.Clear();
            Destroy(vongDau.gameObject);
        }

        _vongDaus.Clear();
        MySQLManager.Instance.ClearTable(SonConst.MatchPlayerLineupTable);
        MySQLManager.Instance.ClearTable(SonConst.MatchRefereeLineupTable);
        MySQLManager.Instance.ClearTable(SonConst.PostMatchTable);
        MySQLManager.Instance.ClearTable(SonConst.InMatchTable);
        MySQLManager.Instance.ClearTable(SonConst.PrematchTable);
        _taoGiaiDauBtn.gameObject.SetActive(true);
    }

    #region Search

    public async void OnSearchClick()
    {
        UIManager.Instance.ShowPermantCircle();
        var input = _thanhTimKiem.text;

        if (DateTime.TryParseExact(input, SonConst.DateFormat, null, System.Globalization.DateTimeStyles.None,
                out DateTime date))
        {
            print(date.ToString(SonConst.DateFormat));
            foreach (var tranDau in _vongDaus.SelectMany(vongDau => vongDau.TranDaus))
            {
                print(tranDau.NgayDau);
                tranDau.gameObject.SetActive(tranDau.NgayDau == date.ToString(SonConst.DateFormat));
                await Task.Delay(1);
            }
        }
        else
        {
            foreach (var tranDau in _vongDaus.SelectMany(vongDau => vongDau.TranDaus))
            {
                // Nhập tên team/ id team/ ngày thi đấu
                if (StringUtils.ContainsNormalized(input, tranDau.Team1) ||
                    StringUtils.ContainsNormalized(input, tranDau.Team2) ||
                    StringUtils.ContainsNormalized(input, tranDau.Team1ID.ToString()) ||
                    StringUtils.ContainsNormalized(input, tranDau.Team2ID.ToString()))
                {
                    tranDau.gameObject.SetActive(true);
                }
                else
                {
                    tranDau.gameObject.SetActive(false);
                }

                await Task.Delay(1);
            }
        }

        UIManager.Instance.HideCircle();
    }

    public async void OnSearchCancelClick()
    {
        _thanhTimKiem.text = "";
        foreach (var tranDau in _vongDaus.SelectMany(vongDau => vongDau.TranDaus))
        {
            tranDau.gameObject.SetActive(true);
            await Task.Delay(1);
        }
    }

    #endregion

    #region DatePicker

    public void OnDatePickerSave()
    {
        var a = (_datePicker.SelectedDate.HasValue)
            ? _datePicker.SelectedDate.Date.ToString(_datePicker.Config.Format.DateFormat)
            : "";
        EventUpdateTodayDate?.Invoke(a);
    }

    #endregion
}
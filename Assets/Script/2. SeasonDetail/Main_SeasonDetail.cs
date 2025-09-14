using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
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
    public bool is_active;

    public PrematchDB(int matchID, int homeTeamID, int awayTeamID, int stadiumID, string matchDate,
        int ticketPrice, int tournamentRound, bool isActive)
    {
        match_id = matchID;
        home_team_id = homeTeamID;
        away_team_id = awayTeamID;
        stadium_id = stadiumID;
        match_date = matchDate;
        ticket_price = ticketPrice;
        tournament_round = tournamentRound;
        is_active = isActive;
    }

    public string ConvertToCsv()
    {
        return
            $"{match_id},{home_team_id},{away_team_id},{stadium_id},{match_date},,{ticket_price},{tournament_round}";
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
    public int active;

    public Team(int teamID, string teamName, DateTime teamCreated, int countryID, int stadiumID, string homeKit,
        string awayKit, string thirdKit, int active)
    {
        team_id = teamID;
        team_name = teamName;
        team_created = teamCreated;
        country_id = countryID;
        stadium_id = stadiumID;
        home_kit = homeKit;
        away_kit = awayKit;
        third_kit = thirdKit;
        this.active = active;
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

    [SerializeField] private InputField _thanhTimKiem;

    [Header("DatePicker"), Space(10)] [SerializeField]
    private DatePicker _datePicker;

    public DatePicker DatePicker => _datePicker;

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
        if (MySQLManager.Instance.IsTableEmpty(SonConst.MatchTable))
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
        var rows = MySQLManager.Instance.GetAllRowsAsList(SonConst.MatchTable);
        if (rows.Count == 0) return;

        // Parse từng row thành MatchDb
        List<PrematchDB> matches = new List<PrematchDB>();
        for (var i = 0; i < 56; i++)
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
                    Convert.ToInt32(double.Parse(row[11])), // ticket_price
                    int.Parse(row[12]), // tournament_round
                    bool.Parse(row[13]) // is_active
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
            // Nhóm theo vòng đấu
            var grouped = matches.GroupBy(m => m.tournament_round).OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                VongDau vongDau = Instantiate(_vongDauPrefab, _content);

                foreach (var match in group)
                {
                    vongDau.AddTranDau(match);
                }

                _vongDaus.Add(vongDau);
                vongDau.SetVongDau(group.Key);
                yield return SonCache.WaitForEndOfFrame;
            }
        }
    }

    List<List<PrematchDB>> GenerateRoundsAndMatches()
    {
        var rowsTeam = MySQLManager.Instance.GetAllRowsAsList("team");

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
                var refs = RandomRefForOneGame();
                var m = new PrematchDB(0, home.team_id, away.team_id, home.stadium_id,
                    matchDate.ToString(SonConst.DateFormat)
                    , RandomTicketPrice(),
                    r + 1, true);
                matchDate = matchDate.AddDays(1);
                matches.Add(m);
            }

            rounds.Add(matches);

            // Xoay vòng: giữ arr[0], dịch phải đoạn [1..n-1]
            Team last = arr[n - 1];
            for (int i = n - 1; i >= 2; i--)
                arr[i] = arr[i - 1];
            arr[1] = last;
        }

        // Lượt về: đảo sân của từng cặp theo đúng thứ tự vòng
        int total = rounds.Count;
//        for (int i = 0; i < total; i++)
//        {
//            var ret = new List<MatchDb>();
//            foreach (var m in rounds[i])
//            {
//                ret.Add(m);
//                matchDate = matchDate.AddDays(1);
//            }
//
//            rounds.Add(ret);
//        }
        for (int i = 0; i < total; i++)
        {
            var ret = new List<PrematchDB>();
            foreach (var m in rounds[i])
            {
                var rematch = new PrematchDB(
                    0,
                    m.away_team_id, // đảo sân
                    m.home_team_id,
                    m.stadium_id,
                    matchDate.ToString(SonConst.DateFormat),
                    m.ticket_price,
                    m.tournament_round + total, // tăng số vòng để phân biệt
                    m.is_active
                );

                ret.Add(rematch);

                // tăng ngày ngay sau khi gán trận
                matchDate = matchDate.AddDays(1);
            }

            rounds.Add(ret);
        }

        return rounds;
    }


    /// <summary>
    /// SEPERATOR
    /// </summary>
    /// <returns></returns>
    public void OnTaoGiaiDauClick()
    {
        var rowsTeam = MySQLManager.Instance.GetAllRowsAsList(SonConst.TeamTable);
        var count = rowsTeam.Count(rowTeamData => rowTeamData[^1] == "1");
        if (count % 2 != 0 || count <= 0)
        {
            UIManager.Instance.ShowToast("Số đội bóng phải chẵn và lớn hơn hoặc bằng 6");
            return;
        }

        StartCoroutine(TaoGiaiDau());
        return;

        IEnumerator TaoGiaiDau()
        {
            List<List<PrematchDB>> rounds = GenerateRoundsAndMatches();


            // In lịch thi đấu
            int round = 1;
            foreach (var lMatchesDB in rounds)
            {
                VongDau vongDau = Instantiate(_vongDauPrefab, _content);

                foreach (var match in lMatchesDB)
                {
                    vongDau.AddTranDau(match);
                    // MySQLManager.Instance.InsertOneRow(SonConst.MatchTable, match.ConvertToCsv(), null);
                }

                _vongDaus.Add(vongDau);
                vongDau.SetVongDau(round);

                round++;
                yield return SonCache.WaitForEndOfFrame;
            }
        }
    }

    public int RandomTicketPrice()
    {
        return UnityEngine.Random.Range(0, 11) * 10000 + 100000;
    }

    (int, int, int, int) RandomRefForOneGame()
    {
        var a = GetRefNames();
        var b = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int index = UnityEngine.Random.Range(0, a.Count);
            b.Add(a[index]);
            a.RemoveAt(index);
        }

        return (b[0], b[1], b[2], b[3]);

        List<int> GetRefNames()
        {
            var allRowsAsList = MySQLManager.Instance.GetAllRowsAsList("referee");
            var t = allRowsAsList.Select(x => int.Parse(x[0])).ToList();
            return t;
        }
    }

    public void OnXoaGiaiDauClick()
    {
        foreach (var vongDau in _vongDaus)
        {
            vongDau.TranDaus.Clear();
            Destroy(vongDau.gameObject);
        }

        _vongDaus.Clear();
    }

    #region Search

    public async void OnSearchClick()
    {
        UIManager.Instance.ShowPermantCircle();
        string format = SonConst.DateFormat;
        var input = _thanhTimKiem.text;

        // TODO: Sửa code dùng event để invoke 
        if (DateTime.TryParseExact(input, format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
        {
            foreach (var tranDau in _vongDaus.SelectMany(vongDau => vongDau.TranDaus))
            {
                tranDau.gameObject.SetActive(tranDau.NgayDau == date.ToString(format));
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
                    StringUtils.ContainsNormalized(input, tranDau.NgayDau) ||
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
        print(a);
        EventUpdateTodayDate?.Invoke(a);
    }

    #endregion
}
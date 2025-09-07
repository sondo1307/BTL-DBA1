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
public class TranDauDetailClass
{
    public GameObject TranDauDetail;
    [FormerlySerializedAs("DataGridUI")] public DataGridUI MatchEvent;
    public DataGridUI Match;
    public Button SaveButton;
    public Button CancelButton;

    public void Open()
    {
        TranDauDetail.gameObject.SetActive(true);
        SaveButton.onClick.AddListener(OnSaveBtnClick);
        CancelButton.onClick.AddListener(OnCancelBtnClick);
        if (MatchEvent.columnData.Count == 0)
        {
            var matchEventsHeader = MySQLManager.Instance.GetTableHeaderAsCsv("match_event");
            var matchEventData = MySQLManager.Instance.GetTableDataAsCsv("match_event");
            var a = CSVDataHelper.CSVStringToColumnData(MatchEvent, matchEventsHeader);
            CSVDataHelper.DataFromCSV(MatchEvent, false, true, true, false, matchEventData);

            var matchHeader = MySQLManager.Instance.GetTableHeaderAsCsv("matchs");
            var matchData = MySQLManager.Instance.GetTableDataAsCsv("matchs");
            var b = CSVDataHelper.CSVStringToColumnData(Match, matchHeader);
            CSVDataHelper.DataFromCSV(Match, false, true, true, false, matchData);
        }
    }

    private void OnSaveBtnClick()
    {
        Debug.Log("Save 111111111");
        Close();
    }

    private void OnCancelBtnClick()
    {
        Close();
    }

    private void Close()
    {
        TranDauDetail.gameObject.SetActive(false);
        MatchEvent.RowClear();
        SaveButton.onClick.RemoveAllListeners();
        CancelButton.onClick.RemoveAllListeners();
    }
}

[Serializable]
public class VongDauDetailClass
{
    public GameObject VongDauDetail;
    public DataGridUI DataGridUI;
    public Button CancelButton;

    public void Open(string header)
    {
        VongDauDetail.gameObject.SetActive(true);
        CancelButton.onClick.AddListener(OnCancelBtnClick);
        if (DataGridUI.columnData.Count == 0)
        {
            var a = CSVDataHelper.CSVStringToColumnData(DataGridUI, header);
        }
    }

    private void OnCancelBtnClick()
    {
        Close();
    }

    public void Close()
    {
        VongDauDetail.gameObject.SetActive(false);
        DataGridUI.RowClear();
        CancelButton.onClick.RemoveAllListeners();
    }
}

[System.Serializable]
public class MatchDb
{
    public int match_id;
    public int home_team_id;
    public int away_team_id;
    public int stadium_id;
    public string match_date;
    public string referee_main;
    public string referee_assist1;
    public string referee_assist2;
    public string referee_assist_var;
    public string away_play_join;
    public string home_play_join;
    public int ticket_price;
    public int tourament_round;

    public MatchDb(int matchID, int homeTeamID, int awayTeamID, int stadiumID, string matchDate, string refereeMain,
        string refereeAssist1, string refereeAssist2, string refereeAssistVar, string awayPlayJoin, string homePlayJoin,
        int ticketPrice, int touramentRound)
    {
        match_id = matchID;
        home_team_id = homeTeamID;
        away_team_id = awayTeamID;
        stadium_id = stadiumID;
        match_date = matchDate;
        referee_main = refereeMain;
        referee_assist1 = refereeAssist1;
        referee_assist2 = refereeAssist2;
        referee_assist_var = refereeAssistVar;
        away_play_join = awayPlayJoin;
        home_play_join = homePlayJoin;
        ticket_price = ticketPrice;
        tourament_round = touramentRound;
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

    [SerializeField] private int _soDoi = 8;
    [SerializeField] private Transform _content;
    [SerializeField] private VongDau _vongDauPrefab;
    [SerializeField] private List<VongDau> _vongDaus = new List<VongDau>();

    [FormerlySerializedAs("_inputfield")] [SerializeField]
    private TMP_InputField _thanhTimKiem;

    [Header("DatePicker"), Space(10)] [SerializeField]
    private DatePicker _datePicker;

    public DatePicker DatePicker => _datePicker;

    [FormerlySerializedAs("TranDauDetailDataGrid")] [Header("TranDauDetail"), Space(10)]
    public TranDauDetailClass tranDauDetailClass;

    [Header("VongDauDetail"), Space(10)] public VongDauDetailClass vongDauDetailClass;

    public Action<string> EventUpdateTodayDate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void OnTaoGiaiDauClick()
    {
        var rowsTeam = MySQLManager.Instance.GetAllRowsAsList("team");
        _soDoi = rowsTeam.Count;
        if (_soDoi % 2 != 0)
        {
            UIManager.Instance.ShowToast("So doi bong phai chan");
            return;
        }

        StartCoroutine(TaoGiaiDau());
        return;

        IEnumerator TaoGiaiDau()
        {
            List<List<MatchDb>> rounds = GenerateRoundsAndMatches();


            // In lịch thi đấu
            int round = 1;
            foreach (var lMatchesDB in rounds)
            {
                VongDau vongDau = Instantiate(_vongDauPrefab, _content);

                foreach (var match in lMatchesDB)
                {
                    vongDau.AddTranDau(match);
                }

                _vongDaus.Add(vongDau);
                vongDau.SetVongDau(round);

                round++;
                yield return SonCache.WaitForEndOfFrame;
            }
        }
    }

    List<List<MatchDb>> GenerateRoundsAndMatches()
    {
        var rowsTeam = MySQLManager.Instance.GetAllRowsAsList("team");

        // Mảng làm việc: giữ arr[0] cố định, xoay các phần tử 1..n-1
        var arr = new List<Team>();
        var rounds = new List<List<MatchDb>>();
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
            var matches = new List<MatchDb>();
            for (int i = 0; i < n / 2; i++)
            {
                var home = arr[i];
                var away = arr[n - 1 - i];
                var refs = RandomRefForOneGame();
                var m = new MatchDb(0, home.team_id, away.team_id, 0, matchDate.ToString(SonConst.DateFormat),
                    refs.Item1, refs.Item2,
                    refs.Item3, refs.Item4, away.team_name, home.team_name, Random.Range(50000, 100000),
                    r + 1);
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
            var ret = new List<MatchDb>();
            foreach (var m in rounds[i])
            {
                var rematch = new MatchDb(
                    0,
                    m.away_team_id,   // đảo sân
                    m.home_team_id,
                    0,
                    matchDate.ToString(SonConst.DateFormat),
                    m.referee_main,
                    m.referee_assist1,
                    m.referee_assist2,
                    m.referee_assist_var,
                    m.home_play_join,
                    m.away_play_join,
                    m.ticket_price,
                    m.tourament_round + total // tăng số vòng để phân biệt
                );

                ret.Add(rematch);

                // tăng ngày ngay sau khi gán trận
                matchDate = matchDate.AddDays(1);
            }

            rounds.Add(ret);
        }

        return rounds;
    }

    (string, string, string, string) RandomRefForOneGame()
    {
        var a = GetRefNames();
        var b = new List<string>();
        for (int i = 0; i < 4; i++)
        {
            int index = UnityEngine.Random.Range(0, a.Count);
            b.Add(a[index]);
            a.RemoveAt(index);
        }

        return (b[0], b[1], b[2], b[3]);

        List<string> GetRefNames()
        {
            var allRowsAsList = MySQLManager.Instance.GetAllRowsAsList("referee");
            var t = allRowsAsList.Select(x => x[1]).ToList();
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
                if (StringUtils.ContainsNormalized(input, tranDau.Team1) ||
                    StringUtils.ContainsNormalized(input, tranDau.Team2))
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Maything.UI.DataGridUI;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class TranDauDetailDataGrid
{
    public GameObject TranDauDetail;
    public DataGridUI DataGridUI;
    public Button SaveButton;
    public Button CancelButton;

    public void Open(string header)
    {
        TranDauDetail.gameObject.SetActive(true);
        SaveButton.onClick.AddListener(OnSaveBtnClick);
        CancelButton.onClick.AddListener(OnCancelBtnClick);
        var a = CSVDataHelper.CSVStringToColumnData(DataGridUI, header);
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
        SaveButton.onClick.RemoveAllListeners();
        CancelButton.onClick.RemoveAllListeners();
    }
}

public class Main_SeasonDetail : MonoBehaviour
{
    public static Main_SeasonDetail Instance { get; set; }

    [SerializeField] private int _soDoi = 8;
    [SerializeField] private List<string> _teams = new List<string>();
    [SerializeField] private Transform _content;
    [SerializeField] private VongDau _vongDauPrefab;
    [SerializeField] private List<VongDau> _vongDaus = new List<VongDau>();
    [FormerlySerializedAs("_inputfield")] [SerializeField] private TMP_InputField _thanhTimKiem;
    [Header("DatePicker"), Space(10)]
    [SerializeField] private DatePicker _datePicker;
    public DatePicker DatePicker => _datePicker;
    
    [Header("TranDauDetailDataGrid"), Space(10)]
    public TranDauDetailDataGrid TranDauDetailDataGrid;

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
        if (_soDoi % 2 != 0)
        {
            UIManager.Instance.ShowToast("So doi bong phai chan");
            return;
        }

        StartCoroutine(TaoGiaiDau());
    }

    private IEnumerator TaoGiaiDau()
    {
        _teams.Clear();
        for (int i = 1; i <= _soDoi; i++)
        {
            _teams.Add("Đội " + i);
        }

        List<List<(string, string)>> schedule = GenerateSchedule(_teams);

        System.Random rnd = new System.Random();
        DateTime currentDate = DateTime.Now; // ngày bắt đầu

        // In lịch thi đấu
        int round = 1;
        foreach (var matchDay in schedule)
        {
            VongDau vongDau = Instantiate(_vongDauPrefab, _content);
            _vongDaus.Add(vongDau);
            vongDau.SetVongDau(round);

            // Random ngày bắt đầu của vòng này (tiến dần so với vòng trước)
            // currentDate = currentDate.AddDays(rnd.Next(4, 7));
            currentDate = currentDate.AddDays(matchDay.Count);

            DateTime matchDate = currentDate;
            foreach (var match in matchDay)
            {
                // Trong cùng vòng, các trận cách nhau 1–2 ngày
                matchDate = matchDate.AddDays(1);

                string ngayDau = matchDate.ToString("dd/MM/yyyy");

                // Nếu VongDau có hàm AddTranDau nhận thêm ngày thì thêm vào
                vongDau.AddTranDau(match.Item1, match.Item2, ngayDau);
                yield return SonCache.WaitForEndOfFrame;
            }

            round++;
        }
    }

// Tuple (string, string) = (team1, team2)
    List<List<(string, string)>> GenerateSchedule(List<string> teams)
    {
        int n = teams.Count;
        if (n % 2 != 0) throw new System.ArgumentException("Số đội phải là số chẵn!");

        // Mảng làm việc: giữ arr[0] cố định, xoay các phần tử 1..n-1
        var arr = new List<string>(teams);
        var rounds = new List<List<(string, string)>>();

        // Lượt đi: n-1 vòng
        for (int r = 0; r < n - 1; r++)
        {
            var matches = new List<(string, string)>();
            for (int i = 0; i < n / 2; i++)
            {
                string home = arr[i];
                string away = arr[n - 1 - i];
                matches.Add((home, away));
            }

            rounds.Add(matches);

            // Xoay vòng: giữ arr[0], dịch phải đoạn [1..n-1]
            string last = arr[n - 1];
            for (int i = n - 1; i >= 2; i--)
                arr[i] = arr[i - 1];
            arr[1] = last;
        }

        // Lượt về: đảo sân của từng cặp theo đúng thứ tự vòng
        int total = rounds.Count;
        for (int i = 0; i < total; i++)
        {
            var ret = new List<(string, string)>();
            foreach (var m in rounds[i])
                ret.Add((m.Item2, m.Item1));
            rounds.Add(ret);
        }

        return rounds;
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
        string format = "dd/MM/yyyy";
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
        var a = (_datePicker.SelectedDate.HasValue) ? _datePicker.SelectedDate.Date.ToString(_datePicker.Config.Format.DateFormat) : "";
        print(a);
        EventUpdateTodayDate?.Invoke(a);
    }

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Maything.UI.DataGridUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _tenCapDau;
    [SerializeField] private TMP_Text _ngayThiDau;
    [SerializeField] private TMP_Text _tiSo;
    public string Team1 { get; private set; }
    public string Team2 { get; private set; }
    public string NgayDau { get; private set; }

    private Image _img;
    [SerializeField] private MatchDb _matchDb;

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

    public void SetCapDau(MatchDb matchDb)
    {
        _matchDb = matchDb;
        _tenCapDau.text = _matchDb.home_team_id + "." + _matchDb.home_play_join + "\n" + _matchDb.away_team_id + "." +
                          _matchDb.away_play_join;
        _tiSo.text = "0" + "\n" + "0";
        _ngayThiDau.text = _matchDb.match_date;
    }

    public void SetTiSo(int team1, int team2)
    {
        _tiSo.text = team1 + " - " + team2;
    }

    public void OnBtnClick()
    {
        Main_SeasonDetail.Instance.tranDauDetailClass.Open(_matchDb.match_id);
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TranDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _tenCapDau;
    [SerializeField] private TMP_Text _ngayThiDau;
    [SerializeField] private TMP_Text _tiSo;
    public string Team1 { get; private set; }
    public string Team2 {get; private set;}
    public string NgayDau {get; private set;}

    private Image _img;

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

    public void SetCapDau(string team1, string team2, string ngayDau)
    {
        _tenCapDau.text = team1 + " - " + team2;
        _tiSo.text = "0" + " - " + "0";
        _ngayThiDau.text = ngayDau;
        Team1 = team1;
        Team2 = team2;
        NgayDau = ngayDau;
    }

    public void SetTiSo(int team1, int team2)
    {
        _tiSo.text = team1 + " - " + team2;
    }

    public void OnBtnClick()
    {
        Main_SeasonDetail.Instance.TranDauDetailDataGrid.Open(
            StringUtils.ConvertHeaderToDataGridHeader("ID, Tên, Số lượng, Giá, Ngày nhập, Gio nhap"));
    }

    private void UpdateDate(string date)
    {
        var myDate = DateTime.ParseExact(_ngayThiDau.text, SonConst.DateFormat, CultureInfo.InvariantCulture);
        var today = DateTime.ParseExact(date, SonConst.DateFormat, CultureInfo.InvariantCulture);
        _img.color = myDate.Date < today.Date ? Color.red : Color.white;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TranDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _tenCapDau;
    [SerializeField] private TMP_Text _ngayThiDau;
    [SerializeField] private TMP_Text _tiSo;
    public string Team1 { get; private set; }
    public string Team2 {get; private set;}
    public string NgayDau {get; private set;}

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
}

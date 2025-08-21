using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VongDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _txt;
    [SerializeField] private TranDau _tranDauPrefab;
    public List<TranDau> TranDaus { get; private set; } = new List<TranDau>();

    public void SetVongDau(int vongDau)
    {
        _txt.text = "Vòng " + vongDau;
    }
    
    public void AddTranDau(string team1, string team2, string ngayDau)
    {
        TranDau tranDau = Instantiate(_tranDauPrefab, transform);
        tranDau.SetCapDau(team1, team2, ngayDau);
        TranDaus.Add(tranDau);
    }
}

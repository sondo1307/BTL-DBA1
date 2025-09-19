using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VongDau : MonoBehaviour
{
    [SerializeField] private TMP_Text _txt;
    [SerializeField] private TranDau _tranDauPrefab;
    public List<TranDau> TranDaus { get; private set; } = new List<TranDau>();
    public int VongDauID { get; private set; }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        Main_SeasonDetail.Instance.vongDauDetailClass.Open(VongDauID);
    }

    public void SetVongDau(int vongDau)
    {
        _txt.text = "Vòng " + vongDau;
        VongDauID = vongDau;
    }

    public void AddTranDau(PrematchDB prematchDB, bool insertData, bool isInsertInMatch)
    {
        TranDau tranDau = Instantiate(_tranDauPrefab, transform);
        tranDau.SetCapDau(prematchDB, insertData, isInsertInMatch);
        TranDaus.Add(tranDau);
    }
}
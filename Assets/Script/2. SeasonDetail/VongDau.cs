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


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        Main_SeasonDetail.Instance.vongDauDetailClass.Open(
            StringUtils.ConvertHeaderToDataGridHeader("ID, Tên, Số lượng, Giá, Ngày nhập, Gio nhap"));
        CSVDataHelper.DataFromCSV(Main_SeasonDetail.Instance.vongDauDetailClass.DataGridUI, false, true, false, false,
            "1,\"Bút bi\",100,5000,110");
    }
    
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

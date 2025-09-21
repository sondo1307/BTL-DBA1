using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Main_RefSalary : MonoBehaviour
{
    [SerializeField] private int _mainRefSalary = 30000000;
    [SerializeField] private int _lineRefSalary = 20000000;
    [SerializeField] private int _tableRefSalary = 10000000;
    [SerializeField] private InputField _mainRefInput;
    [SerializeField] private InputField _lineRefInput;
    [SerializeField] private InputField _tableRefInput;
    [SerializeField] private DataGridUI _dg;
    [SerializeField] private DatePicker _startDate;
    [SerializeField] private DatePicker _endDate;
    [SerializeField] private InputField _tenTrongTaiInputField;
    [SerializeField] private Button _searchBtn;
    [SerializeField] private Button _clearBtn;


    private string _sql =
        @"
SELECT 
    r.referee_id AS ref_id,
    r.full_name AS ref_name,
    SUM(CASE WHEN mrl.referee_main_id  = r.referee_id THEN 1 ELSE 0 END) AS so_tran_tt_chinh,
    SUM(CASE WHEN mrl.referee_assit_1_id = r.referee_id THEN 1 ELSE 0 END) AS so_tran_tt1,
    SUM(CASE WHEN mrl.referee_assit_2_id = r.referee_id THEN 1 ELSE 0 END) AS so_tran_tt2,
    SUM(CASE WHEN mrl.referee_var_id   = r.referee_id THEN 1 ELSE 0 END) AS so_tran_tt_var
FROM referee r
LEFT JOIN match_referee_lineup mrl
    ON r.referee_id IN (
        mrl.referee_main_id, 
        mrl.referee_assit_1_id, 
        mrl.referee_assit_2_id, 
        mrl.referee_var_id
    )
GROUP BY r.referee_id, r.full_name;
";

    private string _sql2;

    private void Start()
    {
        _searchBtn.onClick.AddListener(OnSearchBtnClick);
        _clearBtn.onClick.AddListener(OnClearBtnClick);

        _mainRefInput.text = _mainRefSalary.ToString();
        _lineRefInput.text = _lineRefSalary.ToString();
        _tableRefInput.text = _tableRefSalary.ToString();

        print("open4");
        Load();
    }

    private void Load()
    {
        var a = MySQLManager.Instance.ExecuteQueryToCsv(_sql);

        var totalMain = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt_chinh");
        var totalAssist1 = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt1");
        var totalAssist2 = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt2");
        var totalVar = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt_var");

        List<object> salary = new List<object>();
        for (int i = 0; i < totalMain.Count; i++)
        {
            salary.Add(int.Parse(totalMain[i]) * _mainRefSalary +
                       int.Parse(totalAssist1[i]) * _lineRefSalary +
                       int.Parse(totalAssist2[i]) * _lineRefSalary +
                       int.Parse(totalVar[i]) * _tableRefSalary
            );
        }

        List<string> salary2 = salary.ConvertAll(x => x.ToString());

        var newA = CSVDataHelper.AddNewColumnToCsv(a, "salary", salary2);
        var (b, c) = StringUtils.SplitCsvFromString(newA);
        if (_dg.columnData.Count == 0)
        {
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            CSVDataHelper.CSVStringToColumnData(_dg, h1);
        }

        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, c);
    }

    private void OnSearchBtnClick()
    {
        if (_startDate.SelectedDate.HasValue == false || _endDate.SelectedDate.HasValue == false ||
            string.IsNullOrEmpty(_tenTrongTaiInputField.text) == false)
        {
            UIManager.Instance.ShowToast("Please fill all fields");
            return;
        }

        //TODO: tinh luong trong tai trong 1 thang
        var a = MySQLManager.Instance.ExecuteQueryToCsv(_sql2);

        var totalMain = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt_chinh");
        var totalAssist1 = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt1");
        var totalAssist2 = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt2");
        var totalVar = MySQLManager.Instance.GetColumnValuesFromCsv(a, "so_tran_tt_var");

        List<object> salary = new List<object>();
        for (int i = 0; i < totalMain.Count; i++)
        {
            salary.Add(int.Parse(totalMain[i]) * _mainRefSalary +
                       int.Parse(totalAssist1[i]) * _lineRefSalary +
                       int.Parse(totalAssist2[i]) * _lineRefSalary +
                       int.Parse(totalVar[i]) * _tableRefSalary
            );
        }

        List<string> salary2 = salary.ConvertAll(x => x.ToString());

        var newA = CSVDataHelper.AddNewColumnToCsv(a, "salary", salary2);
        var (b, c) = StringUtils.SplitCsvFromString(newA);
        if (_dg.columnData.Count == 0)
        {
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            CSVDataHelper.CSVStringToColumnData(_dg, h1);
        }

        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, c);
    }

    private void OnClearBtnClick()
    {
        // clear
        _tenTrongTaiInputField.text = "";
        Load();
    }
}
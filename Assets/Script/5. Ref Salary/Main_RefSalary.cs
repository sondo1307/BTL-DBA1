using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

/*
 SELECT
       r.referee_id,
       r.full_name,

       -- Số trận làm trọng tài chính
       SUM(CASE WHEN mrl.referee_main_id = r.referee_id THEN 1 ELSE 0 END) AS referee_main,

       -- Số trận làm trợ lý 1
       SUM(CASE WHEN mrl.referee_assist_1_id = r.referee_id THEN 1 ELSE 0 END) AS referee_assist_1,

       -- Số trận làm trợ lý 2
       SUM(CASE WHEN mrl.referee_assist_2_id = r.referee_id THEN 1 ELSE 0 END) AS referee_assist_2,

       -- Số trận làm VAR
       SUM(CASE WHEN mrl.referee_var_id = r.referee_id THEN 1 ELSE 0 END) AS referee_var

   FROM referee r
   LEFT JOIN match_referee_lineup mrl
       ON r.referee_id IN (
           mrl.referee_main_id,
           mrl.referee_assist_1_id,
           mrl.referee_assist_2_id,
           mrl.referee_var_id
       )
   GROUP BY r.referee_id, r.full_name
   ORDER BY r.full_name;

 */
public class Main_RefSalary : MonoBehaviour
{
    [SerializeField] private int _mainRefSalary = 30000000;
    [SerializeField] private int _lineRefSalary = 20000000;
    [SerializeField] private int _tableRefSalary = 10000000;
    [SerializeField] private InputField _mainRefInput;
    [SerializeField] private InputField _lineRefInput;
    [SerializeField] private InputField _tableRefInput;
    [SerializeField] private DataGridUI _dg;


    private string _sql =
        @"SELECT 
    r.referee_id,
    r.full_name,

    -- Số trận làm trọng tài chính
    SUM(CASE WHEN mrl.referee_main_id = r.referee_id THEN 1 ELSE 0 END) AS referee_main,

    -- Số trận làm trợ lý 1
    SUM(CASE WHEN mrl.referee_assist_1_id = r.referee_id THEN 1 ELSE 0 END) AS referee_assist_1,

    -- Số trận làm trợ lý 2
    SUM(CASE WHEN mrl.referee_assist_2_id = r.referee_id THEN 1 ELSE 0 END) AS referee_assist_2,

    -- Số trận làm VAR
    SUM(CASE WHEN mrl.referee_var_id = r.referee_id THEN 1 ELSE 0 END) AS referee_var

FROM referee r
LEFT JOIN match_referee_lineup mrl
    ON r.referee_id IN (
        mrl.referee_main_id,
        mrl.referee_assist_1_id,
        mrl.referee_assist_2_id,
        mrl.referee_var_id
    )
GROUP BY r.referee_id, r.full_name
ORDER BY r.full_name;
";

    private void Start()
    {
        _mainRefInput.text = _mainRefSalary.ToString();
        _lineRefInput.text = _lineRefSalary.ToString();
        _tableRefInput.text = _tableRefSalary.ToString();

        print("open4");
        var a = MySQLManager.Instance.ExecuteQueryToCsv(_sql);
        var (b, c) = StringUtils.SplitCsvFromString(a);
        if (_dg.columnData.Count == 0)
        {
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            print(h2);
            CSVDataHelper.CSVStringToColumnData(_dg, h2);
        }

        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, c);
    }
}
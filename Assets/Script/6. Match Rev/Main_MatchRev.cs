using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;

/*
SELECT
    s.stadium_id,
    s.stadium_name,
    SUM(m.audience * m.ticket_price) AS total_income
FROM stadium s
JOIN matchs m ON s.stadium_id = m.stadium_id
GROUP BY s.stadium_id, s.stadium_name
ORDER BY total_income DESC;
*/

public class Main_MatchRev : MonoBehaviour
{
    [SerializeField] private DataGridUI _dg1;
    [SerializeField] private DataGridUI _dg2;
    [SerializeField] private Button _btn1;
    [SerializeField] private Button _btn2;
    
    
    private string _sql1 =
        @"
SELECT 
    pm.match_id,
    pm.stadium_id,
    s.stadium_name,
    pm.ticket_price,
    po.attendance,
    (pm.ticket_price * po.attendance) AS total_rev
FROM pre_match pm
JOIN post_match po 
    ON pm.match_id = po.match_id
JOIN stadium s 
    ON pm.stadium_id = s.stadium_id;
";

    private string _sql2 =
        @"
SELECT 
    pm.stadium_id,
    s.stadium_name,
    SUM(pm.ticket_price * po.attendance) AS total_rev
FROM pre_match pm
JOIN post_match po 
    ON pm.match_id = po.match_id
JOIN stadium s 
    ON pm.stadium_id = s.stadium_id
GROUP BY pm.stadium_id, s.stadium_name;
";

    private void Awake()
    {
        _btn1.onClick.AddListener(() =>
        {
            _dg1.gameObject.SetActive(true);
            _dg2.gameObject.SetActive(false);
        });
        _btn2.onClick.AddListener(() =>
        {
            _dg1.gameObject.SetActive(false);
            _dg2.gameObject.SetActive(true);
        });
    }


    // Start is called before the first frame update
    void Start()
    {
        print("open5");
        var a = MySQLManager.Instance.ExecuteQueryToCsv(_sql1);
        var (b, c) = StringUtils.SplitCsvFromString(a);
        if (_dg1.columnData.Count == 0)
        {
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            // var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            // print(h2);
            CSVDataHelper.CSVStringToColumnData(_dg1, h1);
        }

        CSVDataHelper.DataFromCSV(_dg1, false, true, true, false, c);

        var a2 = MySQLManager.Instance.ExecuteQueryToCsv(_sql2);
        var (b2, c2) = StringUtils.SplitCsvFromString(a2);
        if (_dg2.columnData.Count == 0)
        {
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b2);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            print(h2);
            CSVDataHelper.CSVStringToColumnData(_dg2, h1);
        }
        
        CSVDataHelper.DataFromCSV(_dg2, false, true, true, false, c2);
    }
}

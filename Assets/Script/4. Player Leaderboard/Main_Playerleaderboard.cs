using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;


/*
SELECT 
    p.player_id,
    p.player_name,
    COUNT(g.goal_id) AS total_goals
FROM players p
JOIN goals g ON p.player_id = g.player_id
GROUP BY p.player_id, p.player_name
ORDER BY total_goals DESC
LIMIT 10;
*/

/*
@SELECT 
    p.player_id,
    p.full_name,
    COUNT(*) AS total_goals
FROM in_match im
JOIN team_player tp ON im.team_player_id = tp.id
JOIN player p ON tp.player_id = p.player_id
WHERE im.event_type = 'goal'
GROUP BY p.player_id, p.full_name
ORDER BY total_goals DESC
LIMIT 10;
*/
public class Main_Playerleaderboard : MonoBehaviour
{
    [SerializeField] private DataGridUI _dg;

    private string _sql =
        @"SELECT 
    p.player_id,
    p.full_name,
    COUNT(*) AS total_goals
    FROM in_match im
        JOIN team_player tp ON im.team_player_id = tp.id
        JOIN player p ON tp.player_id = p.player_id
        WHERE im.event_type = 'goal'
    GROUP BY p.player_id, p.full_name
        ORDER BY total_goals DESC
        LIMIT 10;";
    
    private void Start()
    {
        print("open3");
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

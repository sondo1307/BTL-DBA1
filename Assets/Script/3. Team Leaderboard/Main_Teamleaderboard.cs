using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;


// Điểm, Hiệu số, tổng số bàn thắng ghi được, số lần thủng lưới, số trận thắng, số trận thắng sân khách, tổng số thẻ đỏ, tổng số thẻ vàng.
/*
SELECT 
    t.team_id,
    t.team_name,

    -- Tổng số bàn thắng
    SUM(
        CASE 
            WHEN pm.home_team_id = t.team_id THEN po.home_score
            WHEN pm.away_team_id = t.team_id THEN po.away_score
            ELSE 0
        END
    ) AS total_goals,

    -- Số trận thắng sân khách
    SUM(
        CASE 
            WHEN pm.away_team_id = t.team_id AND po.away_score > po.home_score THEN 1
            ELSE 0
        END
    ) AS away_wins,

    -- Tổng số thẻ đỏ
    SUM(
        CASE 
            WHEN im.event_type = 'red_card' AND tp.team_id = t.team_id THEN 1
            ELSE 0
        END
    ) AS total_red_cards,

    -- Tổng số thẻ vàng
    SUM(
        CASE 
            WHEN im.event_type = 'yellow_card' AND tp.team_id = t.team_id THEN 1
            ELSE 0
        END
    ) AS total_yellow_cards

FROM team t
LEFT JOIN pre_match pm 
    ON t.team_id IN (pm.home_team_id, pm.away_team_id)
LEFT JOIN post_match po 
    ON pm.match_id = po.match_id
LEFT JOIN in_match im
    ON pm.match_id = im.match_id
LEFT JOIN team_player tp
    ON im.team_player_id = tp.id

GROUP BY t.team_id, t.team_name
ORDER BY total_goals DESC, away_wins DESC, total_red_cards ASC, total_yellow_cards ASC;


*/

public class Main_Teamleaderboard : MonoBehaviour
{
    [SerializeField] private int _winPoint = 3;
    [SerializeField] private InputField _winInput;
    [SerializeField] private int _drawPoint = 1;
    [SerializeField] private InputField _drawInput;
    [SerializeField] private int _losePoint = 0;
    [SerializeField] private InputField _loseInput;

    [SerializeField] private DataGridUI _dg;
    
    private void Start()
    {
        _winInput.text = _winPoint.ToString();
        _drawInput.text = _drawPoint.ToString();
        _loseInput.text = _losePoint.ToString();

        print("open2");
        var a = MySQLManager.Instance.GetTeamRankingAsCsv();
        var (b,c) = StringUtils.SplitCsvFromString(a);
        print(b);
        print(c);
        if (_dg.columnData.Count == 0)
        {
            // CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dg, UpdateOrInsert.Update,
                // SonConst.TeamTable);
            
            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            print(h2);
            CSVDataHelper.CSVStringToColumnData(_dg, h2);
        }

        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, c);
    }
}

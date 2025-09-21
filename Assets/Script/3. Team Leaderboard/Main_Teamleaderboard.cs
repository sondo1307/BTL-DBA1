using System;
using System.Collections;
using System.Collections.Generic;
using Maything.UI.DataGridUI;
using UnityEngine;
using UnityEngine.UI;


// Điểm, Hiệu số, tổng số bàn thắng ghi được, số lần thủng lưới, số trận thắng, số trận thắng sân khách, tổng số thẻ đỏ, tổng số thẻ vàng.

public class Main_Teamleaderboard : MonoBehaviour
{
    [SerializeField] private int _winPoint = 3;
    [SerializeField] private InputField _winInput;
    [SerializeField] private int _drawPoint = 1;
    [SerializeField] private InputField _drawInput;
    [SerializeField] private int _losePoint = 0;
    [SerializeField] private InputField _loseInput;

    [SerializeField] private DataGridUI _dg;

    private string _sql =
        @"
WITH match_stats AS (
  SELECT team_id,
         SUM(win)      AS total_wins,
         SUM(away_win) AS away_wins,
         SUM(draw)     AS total_draws
  FROM (
    -- Home team
    SELECT pm.home_team_id AS team_id,
           CASE WHEN po.home_score > po.away_score THEN 1 ELSE 0 END AS win,
           0 AS away_win,
           CASE WHEN po.home_score = po.away_score THEN 1 ELSE 0 END AS draw
    FROM pre_match pm
    JOIN post_match po ON pm.match_id = po.match_id

    UNION ALL

    -- Away team
    SELECT pm.away_team_id AS team_id,
           CASE WHEN po.away_score > po.home_score THEN 1 ELSE 0 END AS win,
           CASE WHEN po.away_score > po.home_score THEN 1 ELSE 0 END AS away_win,
           CASE WHEN po.away_score = po.home_score THEN 1 ELSE 0 END AS draw
    FROM pre_match pm
    JOIN post_match po ON pm.match_id = po.match_id
  ) x
  GROUP BY team_id
),

goal_stats AS (
  SELECT tp.team_id,
         COUNT(*) AS total_goals
  FROM in_match im
  JOIN match_player_lineup mpl
    ON mpl.match_id = im.match_id
   AND mpl.team_player_id = im.team_player_id
  JOIN team_player tp ON tp.team_player_id = mpl.team_player_id
  WHERE im.event_type = 0
  GROUP BY tp.team_id
),

card_stats AS (
  SELECT tp.team_id,
         SUM(CASE WHEN im.event_type = 2 THEN 1 ELSE 0 END) AS yellow_cards,
         SUM(CASE WHEN im.event_type = 1 THEN 1 ELSE 0 END) AS red_cards
  FROM in_match im
  JOIN match_player_lineup mpl
    ON mpl.match_id = im.match_id
   AND mpl.team_player_id = im.team_player_id
  JOIN team_player tp ON tp.team_player_id = mpl.team_player_id
  GROUP BY tp.team_id
),

conceded_stats AS (
  SELECT team_id, SUM(conceded) AS total_conceded
  FROM (
    SELECT pm.home_team_id AS team_id, po.away_score AS conceded
    FROM pre_match pm
    JOIN post_match po ON pm.match_id = po.match_id

    UNION ALL

    SELECT pm.away_team_id AS team_id, po.home_score AS conceded
    FROM pre_match pm
    JOIN post_match po ON pm.match_id = po.match_id
  ) x
  GROUP BY team_id
)

SELECT
  t.team_id,
  t.team_name,
  COALESCE(gs.total_goals,0)   AS total_goals,
  COALESCE(gs.total_goals,0) - COALESCE(cs2.total_conceded,0) AS goal_difference
  COALESCE(cs.yellow_cards,0)  AS total_yellow_cards,	
  COALESCE(cs.red_cards,0)     AS total_red_cards,
  COALESCE(ms.away_wins,0)     AS away_wins,
  COALESCE(ms.total_wins,0)    AS total_wins,
  COALESCE(ms.total_draws,0)   AS total_draws,
  COALESCE(cs2.total_conceded,0) AS total_conceded,
FROM team t
LEFT JOIN goal_stats gs      ON gs.team_id = t.team_id
LEFT JOIN match_stats ms     ON ms.team_id = t.team_id
LEFT JOIN card_stats cs      ON cs.team_id = t.team_id
LEFT JOIN conceded_stats cs2 ON cs2.team_id = t.team_id
ORDER BY t.team_name;
        ";

    private void Start()
    {
        _winInput.text = _winPoint.ToString();
        _drawInput.text = _drawPoint.ToString();
        _loseInput.text = _losePoint.ToString();

        print("open2");
        var a = MySQLManager.Instance.ExecuteQueryToCsv(_sql);

        var totalWins = MySQLManager.Instance.GetColumnValuesFromCsv(a, "total_wins");
        var totalDraws = MySQLManager.Instance.GetColumnValuesFromCsv(a, "total_draws");
        var totalConceded = MySQLManager.Instance.GetColumnValuesFromCsv(a, "total_conceded");
        List<object> scores = new List<object>();
        for (int i = 0; i < totalWins.Count; i++)
        {
            scores.Add(int.Parse(totalWins[i]) * _winPoint +
                       int.Parse(totalDraws[i]) * _drawPoint +
                       int.Parse(totalConceded[i]) * _losePoint
            );
        }

        List<string> scores2 = scores.ConvertAll(x => x.ToString());

        var newA = CSVDataHelper.AddNewColumnToCsv(a, "total_points", scores2);
        var (b, c) = StringUtils.SplitCsvFromString(newA);

        if (_dg.columnData.Count == 0)
        {
            // CSVDataHelper.GetTableHeaderAndConvertToInputFieldAndSetToDGColumnData(_dg, UpdateOrInsert.Update,
            // SonConst.TeamTable);

            var h1 = StringUtils.ConvertHeaderToDataGridHeader(b);
            var h2 = StringUtils.ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(h1);
            CSVDataHelper.CSVStringToColumnData(_dg, h1);
        }

        CSVDataHelper.DataFromCSV(_dg, false, true, true, false, c);
    }
}
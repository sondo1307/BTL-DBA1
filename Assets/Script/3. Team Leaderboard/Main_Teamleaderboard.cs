using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Điểm, Hiệu số, tổng số bàn thắng ghi được, số trận thắng sân khách, tổng số thẻ đỏ, tổng số thẻ vàng.
/*
SELECT 
    t.team_id,
    t.team_name,

    -- Tổng điểm
    SUM(
        CASE 
            WHEN t.team_id = pm.home_team_id AND am.home_score > am.away_score THEN 3
            WHEN t.team_id = pm.away_team_id AND am.away_score > am.home_score THEN 3
            WHEN am.home_score = am.away_score THEN 1
            ELSE 0
        END
    ) AS diem,

    -- Hiệu số
    SUM(
        CASE WHEN t.team_id = pm.home_team_id THEN am.home_score - am.away_score
             WHEN t.team_id = pm.away_team_id THEN am.away_score - am.home_score
             ELSE 0 END
    ) AS hieu_so,

    -- Tổng số bàn thắng ghi được
    SUM(
        CASE WHEN t.team_id = pm.home_team_id THEN am.home_score
             WHEN t.team_id = pm.away_team_id THEN am.away_score
             ELSE 0 END
    ) AS tong_ban_thang,

    -- Số trận thắng sân khách
    SUM(
        CASE WHEN t.team_id = pm.away_team_id AND am.away_score > am.home_score THEN 1 ELSE 0 END
    ) AS tran_thang_san_khach,

    -- Tổng số thẻ đỏ
    SUM(
        CASE WHEN ev.event_type = 'red_card' THEN 1 ELSE 0 END
    ) AS tong_the_do,

    -- Tổng số thẻ vàng
    SUM(
        CASE WHEN ev.event_type = 'yellow_card' THEN 1 ELSE 0 END
    ) AS tong_the_vang

FROM team t
LEFT JOIN pre_match pm 
    ON t.team_id IN (pm.home_team_id, pm.away_team_id)
LEFT JOIN after_match am 
    ON pm.match_id = am.match_id
LEFT JOIN in_match ev
    ON pm.match_id = ev.match_id

GROUP BY t.team_id, t.team_name
ORDER BY diem DESC, hieu_so DESC, tong_ban_thang DESC;
*/

/*
SELECT 
    t.team_id,
    t.team_name,

    -- tổng số trận thắng
    SUM(
        (m.home_team_id = t.team_id AND m.home_score > m.away_score) OR
        (m.away_team_id = t.team_id AND m.away_score > m.home_score)
    ) AS total_wins,

    -- tổng số trận thua
    SUM(
        (m.home_team_id = t.team_id AND m.home_score < m.away_score) OR
        (m.away_team_id = t.team_id AND m.away_score < m.home_score)
    ) AS total_losses,

    -- tổng số bàn thắng
    SUM(
        CASE 
            WHEN m.home_team_id = t.team_id THEN m.home_score
            WHEN m.away_team_id = t.team_id THEN m.away_score
            ELSE 0
        END
    ) AS total_goals,

    -- số trận thắng sân khách
    SUM(
        (m.away_team_id = t.team_id AND m.away_score > m.home_score)
    ) AS away_wins,

    -- tổng số thẻ đỏ
    SUM(CASE WHEN c.card_type = 'red' THEN 1 ELSE 0 END) AS total_red_cards,

    -- tổng số thẻ vàng
    SUM(CASE WHEN c.card_type = 'yellow' THEN 1 ELSE 0 END) AS total_yellow_cards

FROM team t
LEFT JOIN matchs m 
    ON t.team_id IN (m.home_team_id, m.away_team_id)
LEFT JOIN card c 
    ON c.team_id = t.team_id AND c.match_id = m.match_id
GROUP BY t.team_id, t.team_name
ORDER BY total_wins DESC;
*/

public class Main_Teamleaderboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
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
public class Main_Playerleaderboard : MonoBehaviour
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

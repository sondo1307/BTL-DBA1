using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

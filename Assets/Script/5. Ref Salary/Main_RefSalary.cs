using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
SELECT 
    r.referee_id,
    r.referee_name,
    COUNT(mr.match_id) AS total_matches,
    SUM(
        CASE mr.role
            WHEN 'MAIN'  THEN 3000000
            WHEN 'LINE'  THEN 2000000
            WHEN 'TABLE' THEN 1000000
            ELSE 0
        END
    ) AS total_salary
FROM referee r
JOIN match_referee mr ON r.referee_id = mr.referee_id
JOIN matchs m ON mr.match_id = m.match_id
WHERE MONTH(m.match_date) = 9 AND YEAR(m.match_date) = 2025
GROUP BY r.referee_id, r.referee_name
ORDER BY total_salary DESC;
*/

/*
SELECT 
    r.referee_id,
    r.referee_name,
    COUNT(mr.match_id) AS total_matches
FROM referee r
JOIN match_referee mr ON r.referee_id = mr.referee_id
JOIN matchs m ON mr.match_id = m.match_id
WHERE MONTH(m.match_date) = 9 
  AND YEAR(m.match_date) = 2025
GROUP BY r.referee_id, r.referee_name
ORDER BY total_matches DESC;
*/

public class Main_RefSalary : MonoBehaviour
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

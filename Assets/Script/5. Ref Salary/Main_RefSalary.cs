using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private int _mainRefSalary = 30000000;
    [SerializeField] private int _lineRefSalary = 20000000;
    [SerializeField] private int _tableRefSalary = 10000000;
    [SerializeField] private InputField _mainRefInput;
    [SerializeField] private InputField _lineRefInput;
    [SerializeField] private InputField _tableRefInput;

    private void Start()
    {
        _mainRefInput.text = _mainRefSalary.ToString();
        _lineRefInput.text = _lineRefSalary.ToString();
        _tableRefInput.text = _tableRefSalary.ToString();
    }
}

using UnityEngine;
using MySql.Data.MySqlClient; // cần DLL MySql.Data.dll
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;

public class MySQLManager : MonoBehaviour
{
    public static MySQLManager Instance;

    // Connection string Railway hoặc MySQL Workbench
    // 👉 Railway: thay Server, Port, User, Password, Database theo config của Railway
    // 👉 Workbench local: thường là Server=localhost;Port=3306;User=root;Password=;Database=test;
    private string connectionString = "Server=metro.proxy.rlwy.net;Port=18250;" +
                                      "Database=railway;" +
                                      "User=root;" +
                                      "Password=MxzpoKnKIutJSeZaEdjKSjcsQGsvtdmB;" +
                                      "SslMode=None;" +
                                      "AllowPublicKeyRetrieval=True;CharSet=utf8mb4;";

    public MySqlConnection Conn;
    [SerializeField] private decimal _aa;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Khởi tạo và mở kết nối một lần
        try
        {
            Conn = new MySqlConnection(connectionString);
            Conn.Open();
            Debug.Log("✅ Connected to MySQL!");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ MySQL Connection Error: " + ex.Message);
        }
    }

    [Button]
    private void Test()
    {
        ClearTable(SonConst.MatchPlayerLineupTable, false);
        ClearTable(SonConst.MatchRefereeLineupTable, false);
        ClearTable(SonConst.InMatchTable, false);
        ClearTable(SonConst.PostMatchTable, false);
        ClearTable(SonConst.PrematchTable, false);
    }

    void OnDestroy()
    {
        if (Conn != null && Conn.State == System.Data.ConnectionState.Open)
        {
            Conn.Close();
            Conn.Dispose();
            Debug.Log("✅ Connection closed.");
        }
    }

    public bool IsTableEmpty(string table)
    {
        try
        {
            string query = $"SELECT EXISTS(SELECT 1 FROM `{table}` LIMIT 1);";
            using var cmd = new MySqlCommand(query, Conn);
            object result = cmd.ExecuteScalar();
            // if result = 1 → table has rows, if 0 → empty
            return Convert.ToInt32(result) == 0;
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return true; // assume empty on error
        }
    }

    public string GetCellDataByRowId(string table, string targetColumn, string idColumn, object idValue)
    {
        try
        {
            string query = $"SELECT `{targetColumn}` FROM `{table}` WHERE `{idColumn}` = @id LIMIT 1;";

            using var cmd = new MySqlCommand(query, Conn);
            cmd.Parameters.AddWithValue("@id", idValue);

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return string.Empty;

            if (result is DateTime dt)
                return dt.ToString(SonConst.DateFormat);
            if (result is bool b)
                return b ? "1" : "0";

            return result.ToString();
        }
        catch (MySqlException ex)
        {
            Debug.LogError($"❌ MySQL Error: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Lấy tất cả row trong bảng theo điều kiện column = value,
    /// trả về dưới dạng CSV (mỗi row = 1 dòng, các cột cách nhau bằng dấu phẩy)
    /// </summary>
    /// <param name="table">Tên bảng</param>
    /// <param name="column">Tên cột so sánh</param>
    /// <param name="value">Giá trị cần so sánh</param>
    /// <returns>CSV string (nhiều dòng)</returns>
    public string GetRowsByColumnValueAsCsv(string table, string column, object value)
    {
        var csvLines = new List<string>();

        try
        {
            string query = $"SELECT * FROM `{table}` WHERE `{column}` = @value;";
            using var cmd = new MySqlCommand(query, Conn);
            cmd.Parameters.AddWithValue("@value", value);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string val;

                    if (rawValue is DateTime dt)
                        val = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        val = b ? "1" : "0";
                    else
                        val = rawValue?.ToString() ?? "";

                    // Escape dấu phẩy
                    val = val.Replace(",", "&");

                    row.Add(val);
                }

                csvLines.Add(string.Join(",", row));
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
        }

        return string.Join(Environment.NewLine, csvLines);
    }


    /// <summary>
    /// Choose value in 1 column in 1 table to get row data
    /// </summary>
    /// <param name="table"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <returns>string = csv</returns>
    public string GetRowByColumnValueAsCsv(string table, string column, object value)
    {
        try
        {
            string query = $"SELECT * FROM `{table}` WHERE `{column}` = @value LIMIT 1;";
            using var cmd = new MySqlCommand(query, Conn);
            cmd.Parameters.AddWithValue("@value", value);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string val;

                    if (rawValue is DateTime dt)
                        val = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        val = b ? "1" : "0";
                    else
                        val = rawValue?.ToString() ?? "";

                    // Escape dấu phẩy để không phá format CSV
                    val = val.Replace(",", "&");

                    row.Add(val);
                }

                return string.Join(",", row);
            }
            else
            {
                return string.Empty; // Không tìm thấy row
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return string.Empty;
        }
    }

    /// <summary>
    /// Choose value in 1 column in 1 table to get row data
    /// </summary>
    /// <param name="table"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <returns>List of row values (as strings)</returns>
    public List<string> GetRowByColumnValueAsList(string table, string column, object value)
    {
        try
        {
            string query = $"SELECT * FROM `{table}` WHERE `{column}` = @value LIMIT 1;";
            using var cmd = new MySqlCommand(query, Conn);
            cmd.Parameters.AddWithValue("@value", value);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string val;

                    if (rawValue is DateTime dt)
                        val = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        val = b ? "1" : "0";
                    else
                        val = rawValue?.ToString() ?? "";

                    row.Add(val);
                }

                return row;
            }
            else
            {
                return new List<string>(); // Không tìm thấy row
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return new List<string>();
        }
    }

    /// <summary>
    /// Lấy danh sách giá trị của 1 cột khi 1 cột khác có giá trị bằng tham số truyền vào
    /// </summary>
    /// <param name="tableName">Tên bảng</param>
    /// <param name="columnFind">Cột cần lấy giá trị</param>
    /// <param name="columnSoSanh">Cột dùng để so sánh</param>
    /// <param name="giaTriSoSanh">Giá trị cần so sánh</param>
    /// <returns>List<string> chứa các giá trị của columnFind</returns>
    public List<string> GetValuesByCondition(
        string tableName,
        string columnFind,
        string columnSoSanh,
        object giaTriSoSanh)
    {
        var values = new List<string>();

        try
        {
            string query = $"SELECT `{columnFind}` FROM `{tableName}` WHERE `{columnSoSanh}` = @value;";
            using var cmd = new MySqlCommand(query, Conn);
            cmd.Parameters.AddWithValue("@value", giaTriSoSanh);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                object rawValue = reader[columnFind];
                string val;

                if (rawValue is DateTime dt)
                    val = dt.ToString(SonConst.DateFormat); // format ngày
                else if (rawValue is bool b)
                    val = b ? "1" : "0"; // boolean thành 1/0
                else
                    val = rawValue?.ToString() ?? "";

                values.Add(val);
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
        }

        return values;
    }

    public List<List<string>> GetAllRowsAsList(string table)
    {
        var allRows = new List<List<string>>();

        try
        {
            string query = $"SELECT * FROM `{table}`;";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string value;

                    if (rawValue is DateTime dt)
                        value = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        value = b ? "1" : "0";
                    else
                        value = rawValue?.ToString() ?? "";

                    row.Add(value);
                }

                allRows.Add(row);
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
        }

        return allRows;
    }

    public List<string> GetValuesByColumn(string table, string column)
    {
        var values = new List<string>();
        string query = $"SELECT {column} FROM {table}";

        using (var conn = new MySqlConnection(connectionString))
        using (var cmd = new MySqlCommand(query, conn))
        {
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    values.Add(reader[column].ToString());
                }
            }
        }

        return values;
    }

    public string GetTableAsCsv(string table)
    {
        try
        {
            string query = $"SELECT * FROM `{table}`;";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            var csv = new StringBuilder();

            // Write header
            for (int i = 0; i < reader.FieldCount; i++)
            {
                csv.Append(reader.GetName(i));
                if (i < reader.FieldCount - 1) csv.Append(",");
            }

            csv.AppendLine();

            // Write data rows
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string value;

                    if (rawValue is DateTime dt) // Format datetime
                        value = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b) // Format boolean
                        value = b ? "1" : "0";
                    else
                        value = rawValue?.ToString() ?? "";

                    // Escape commas to avoid breaking CSV
                    value = value.Replace(",", "&");

                    csv.Append(value);
                    if (i < reader.FieldCount - 1) csv.Append(",");
                }

                csv.AppendLine();
            }

            return csv.ToString();
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return string.Empty;
        }
    }

    public string GetTableHeaderAsCsv(string table)
    {
        try
        {
            string query = $"SELECT * FROM {table};";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();
            StringBuilder csv = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                csv.Append(reader.GetName(i));
                if (i < reader.FieldCount - 1) csv.Append(",");
            }

            return csv.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    public List<string> GetTableHeaderAsList(string table)
    {
        var headers = new List<string>();
        try
        {
            string query = $"SELECT * FROM {table} LIMIT 1;"; // chỉ cần 1 row là đủ lấy schema
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                headers.Add(reader.GetName(i));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi lấy header: {ex.Message}");
        }

        return headers;
    }

    public List<List<string>> GetTableAsListString(string table)
    {
        var allRows = new List<List<string>>();

        try
        {
            string query = $"SELECT * FROM `{table}`;";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var row = new List<string>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string value;

                    if (rawValue is DateTime dt) // Format date
                    {
                        value = dt.ToString(SonConst.DateFormat);
                    }
                    else if (rawValue is bool b) // Convert bool → 1/0
                    {
                        value = b ? "1" : "0";
                    }
                    else if (rawValue is sbyte sb) // tinyint → sbyte
                    {
                        value = sb.ToString();
                    }
                    else
                    {
                        value = rawValue?.ToString() ?? "";
                    }

                    // Escape dấu phẩy để giống CSV
                    value = value.Replace(",", "&");

                    row.Add(value);
                }

                allRows.Add(row);
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
        }

        return allRows;
    }

    public string GetTableDataAsCsv(string table)
    {
        try
        {
            string query = $"SELECT * FROM {table};";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            StringBuilder csv = new StringBuilder();

            // Data rows
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string value;

                    if (rawValue is DateTime dt) // Nếu là DateTime thì format yyyy-MM-dd
                    {
                        value = dt.ToString(SonConst.DateFormat);
                    }
                    else if (rawValue is bool b) // Nếu là boolean thì chuyển thành 1/0
                    {
                        value = b ? "1" : "0";
                    }
                    else if (rawValue is sbyte sb) // Một số driver trả tinyint là sbyte
                    {
                        value = sb.ToString();
                    }
                    else
                    {
                        value = rawValue.ToString();
                    }

                    // Escape dấu phẩy trong dữ liệu
                    value = value.Replace(",", "&");

                    csv.Append(value);
                    if (i < reader.FieldCount - 1) csv.Append(",");
                }

                csv.AppendLine();
            }

            return csv.ToString();
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return string.Empty;
        }
    }


    public void UpdateOneRow(string tableName, string csvLine, Action callback)
    {
        try
        {
            var columns = GetTableHeaderAsList(tableName);

            // Tách các giá trị từ CSV line
            string[] values = csvLine.Split(',');

            if (values.Length != columns.Count)
            {
                UIManager.Instance.ShowToast("❌ CSV không hợp lệ: số cột không khớp!");
                return;
            }

            // Cột đầu tiên là id
            int id = int.Parse(values[0]);

            // Tạo câu SQL động
            List<string> setClauses = new List<string>();
            // i = 1 bo qua column id (column 0)
            for (int i = 1; i < columns.Count; i++)
            {
                setClauses.Add($"{columns[i]}=@{columns[i]}");
            }

            string sql = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE {columns[0]}=@id";
            print(sql);

            using var cmd = new MySqlCommand(sql, Conn);
            cmd.Parameters.AddWithValue("@id", id);

            // Gán param động
            for (int i = 1; i < columns.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + columns[i], values[i]);
            }

            int rows = cmd.ExecuteNonQuery();
            Debug.Log($"✅ Update row id={id}, affected {rows} rows");

            callback?.Invoke();
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            UIManager.Instance.ShowToast(e.Message);
            throw;
        }
    }

    public void InsertOneRow(string tableName, string csvLine, bool ignoreFirstColumn, Action callback)
    {
        try
        {
            var columns = GetTableHeaderAsList(tableName);

            string[] values = csvLine.Split(',');

            if (values.Length != columns.Count)
            {
                UIManager.Instance.ShowToast("❌ CSV không hợp lệ: số cột không khớp!");
                return;
            }

            // Tạo list tên cột và list placeholder @param
            List<string> colNames = new List<string>();
            List<string> paramNames = new List<string>();

            // Start từ 1 vì id luôn tăng dần auto
            for (int i = ignoreFirstColumn ? 1 : 0; i < columns.Count; i++)
            {
                colNames.Add(columns[i]);
                paramNames.Add("@" + columns[i]);
            }

            string sql =
                $"INSERT INTO {tableName} ({string.Join(", ", colNames)}) VALUES ({string.Join(", ", paramNames)})";

            using var cmd = new MySqlCommand(sql, Conn);

            // Gán param động
            for (int i = ignoreFirstColumn ? 1 : 0; i < columns.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + columns[i], values[i]);
            }

            // foreach (MySqlParameter param in cmd.Parameters)
            // {
            // print($"{param.ParameterName} = {param.Value}");
            // }

            int rows = cmd.ExecuteNonQuery();

            callback?.Invoke();
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            UIManager.Instance.ShowToast(e.Message);
            throw;
        }
    }

    public void DeleteOneRow(string tableName, string idValue)
    {
        var columns = GetTableHeaderAsList(tableName);

        // Cột đầu tiên là khóa chính ID
        string sql = $"DELETE FROM {tableName} WHERE {columns[0]}=@id";

        using var cmd = new MySqlCommand(sql, Conn);
        cmd.Parameters.AddWithValue("@id", idValue);

        int rows = cmd.ExecuteNonQuery();
        Debug.Log($"🗑️ Delete row id={idValue}, affected {rows} rows");
    }

    public void DeleteMultipleRows(string tableName, List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            Debug.LogWarning("⚠️ Không có ID nào để xóa!");
            return;
        }

        var columns = GetTableHeaderAsList(tableName);
        string idColumn = columns[0]; // giả sử cột đầu tiên là ID

        // Tạo parameter cho từng id: @id0, @id1, ...
        List<string> paramNames = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
            paramNames.Add($"@id{i}");
        }

        string sql = $"DELETE FROM {tableName} WHERE {idColumn} IN ({string.Join(", ", paramNames)})";

        using var cmd = new MySqlCommand(sql, Conn);

        // Gán giá trị cho từng param
        for (int i = 0; i < ids.Count; i++)
        {
            cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
        }

        int rows = cmd.ExecuteNonQuery();
        Debug.Log($"🗑️ Delete multiple rows, affected {rows} rows");
    }

    public string SearchTableAsCsv(string tableName, string keyword)
    {
        try
        {
            var columns = GetTableHeaderAsList(tableName);
            if (columns == null || columns.Count == 0)
            {
                Debug.LogWarning($"⚠️ Không tìm thấy cột nào trong bảng {tableName}");
                return string.Empty;
            }

            string sql = $"SELECT * FROM `{tableName}`";
            using var cmd = new MySqlCommand(sql, Conn);
            using var reader = cmd.ExecuteReader();

            var csv = new StringBuilder();

            // // Header
            // for (int i = 0; i < reader.FieldCount; i++)
            // {
            //     csv.Append(reader.GetName(i));
            //     if (i < reader.FieldCount - 1) csv.Append(",");
            // }
            // csv.AppendLine();

            // Data
            while (reader.Read())
            {
                // Gom cả row thành 1 string để check ContainsNormalized
                var rowBuilder = new StringBuilder();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowBuilder.Append(reader[i]?.ToString() ?? "");
                    rowBuilder.Append("|"); // phân cách tạm
                }

                string rowString = rowBuilder.ToString();
                if (!StringUtils.ContainsNormalized(rowString, keyword))
                    continue; // bỏ qua nếu không match

                // Nếu match thì xuất ra CSV
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object raw = reader[i];
                    string value;

                    if (raw is DateTime dt)
                        value = dt.ToString(SonConst.DateFormat);
                    else if (raw is bool b)
                        value = b ? "1" : "0";
                    else
                        value = raw?.ToString() ?? "";

                    value = value.Replace(",", "&"); // tránh phá CSV
                    csv.Append(value);
                    if (i < reader.FieldCount - 1) csv.Append(",");
                }

                csv.AppendLine();
            }

            return csv.ToString();
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return string.Empty;
        }
    }

    public bool ClearTable(string tableName, bool useTruncate = false)
    {
        try
        {
            string query = useTruncate
                ? $"TRUNCATE TABLE {tableName};"
                : $"DELETE FROM {tableName};";

            using var cmd = new MySqlCommand(query, Conn);
            cmd.ExecuteNonQuery();
            Debug.Log($"✅ Cleared table {tableName} (method: {(useTruncate ? "TRUNCATE" : "DELETE")})");
            return true;
        }
        catch (MySqlException ex)
        {
            Debug.LogError($"❌ Error clearing table {tableName}: " + ex.Message);
            return false;
        }
    }

    public bool ValidateTeamInSession1()
    {
// validate_team_in_seesion
        using var cmd = new MySqlCommand("validate_team_in_seesion", Conn);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        // OUT param (BOOLEAN trong MySQL thực chất = tinyint(1))
        var outParam = new MySqlParameter("p_result", MySqlDbType.Int32)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        cmd.Parameters.Add(outParam);

        // Đảm bảo connection đã set charset hợp lệ
        using (var setCmd = new MySqlCommand("SET NAMES utf8mb4;", Conn))
        {
            setCmd.ExecuteNonQuery();
        }

        cmd.ExecuteNonQuery();

        return Convert.ToInt32(outParam.Value) == 1;
    }

    /// <summary>
    /// Gọi procedure sumary_post_match trong MySQL
    /// </summary>
    /// <param name="matchId">Giá trị match_id truyền vào</param>
    /// <returns>Giá trị p_result trả về</returns>
    public int CallSumaryPostMatch(int matchId)
    {
        try
        {
            using var cmd = new MySqlCommand("sumary_post_match", Conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            // Tham số IN
            cmd.Parameters.AddWithValue("@match_id", matchId);

            // Tham số OUT
            var resultParam = new MySqlParameter("@p_result", MySqlDbType.Int32);
            resultParam.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(resultParam);

            // Thực thi
            cmd.ExecuteNonQuery();

            // Lấy kết quả OUT
            return Convert.ToInt32(resultParam.Value);
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return -1;
        }
    }

    /// <summary>
    /// Trả về header CSV của join match_player_lineup với team_player.team_id
    /// </summary>
    public string GetMatchPlayerLineupHeaderCsv()
    {
        string query = @"
        SELECT mpl.*, tp.team_id
        FROM match_player_lineup AS mpl
        JOIN team_player AS tp
          ON mpl.team_player_id = tp.id
        WHERE 1=0;"; // trả metadata, không lấy row

        try
        {
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            var headerCols = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                headerCols.Add(reader.GetName(i));

            return ConvertListToCsv(headerCols);
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error (header): " + ex.Message);
            return string.Empty;
        }
    }

/*
SELECT mpl.*, tp.team_id
FROM match_player_lineup AS mpl
JOIN team_player AS tp
  ON mpl.team_player_id = tp.id;
*/
    /// <summary>
    /// Trả về các dòng data CSV (không gồm header)
    /// </summary>
    public string GetMatchPlayerLineupDataCsv()
    {
        var csvLines = new List<string>();
        string query = @"
        SELECT mpl.*, tp.team_id
        FROM match_player_lineup AS mpl
        JOIN team_player AS tp
          ON mpl.team_player_id = tp.id;";

        try
        {
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader.GetValue(i);
                    string val;

                    if (rawValue is DateTime dt)
                        val = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        val = b ? "1" : "0";
                    else
                        val = rawValue?.ToString() ?? "";

                    val = val.Replace("\r", " ").Replace("\n", " ");
                    row.Add(val);
                }

                csvLines.Add(ConvertListToCsv(row));
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error (data): " + ex.Message);
            return string.Empty;
        }

        return string.Join(Environment.NewLine, csvLines);
    }

    /// <summary>
    /// Lấy tất cả các row của join match_player_lineup <> team_player
    /// và trả về 1 CSV string (dòng header + các dòng dữ liệu)
    /// </summary>
    public string GetMatchPlayerLineupWithTeamIdAsCsv()
    {
        var csvLines = new List<string>();
        string query = @"
        SELECT mpl.*, tp.team_id
        FROM match_player_lineup AS mpl
        JOIN team_player AS tp
          ON mpl.team_player_id = tp.id;";

        try
        {
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            // --- Header ---
            var headerCols = Enumerable.Range(0, reader.FieldCount)
                .Select(i => reader.GetName(i))
                .ToList();
            csvLines.Add(ConvertListToCsv(headerCols));

            // --- Data rows ---
            while (reader.Read())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader.GetValue(i);
                    string val;

                    if (rawValue is DateTime dt)
                        val = dt.ToString(SonConst.DateFormat);
                    else if (rawValue is bool b)
                        val = b ? "1" : "0";
                    else
                        val = rawValue?.ToString() ?? "";

                    // giữ CSV sạch sẽ
                    val = val.Replace(",", "&")
                        .Replace("\r", " ")
                        .Replace("\n", " ");

                    row.Add(val);
                }

                csvLines.Add(ConvertListToCsv(row));
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
            return string.Empty;
        }

        return string.Join(Environment.NewLine, csvLines);
    }


    /// <summary>
    /// Convert list string thành 1 dòng CSV
    /// </summary>
    private string ConvertListToCsv(List<string> values)
    {
        if (values == null || values.Count == 0)
            return string.Empty;

        var safeValues = new List<string>(values.Count);
        foreach (var v in values)
        {
            string val = v ?? "";

            if (val.Contains("\""))
                val = val.Replace("\"", "\"\"");

            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n") || val.Contains("\r"))
                val = $"\"{val}\"";

            safeValues.Add(val);
        }

        return string.Join(",", safeValues);
    }


    /// <summary>
    /// Chạy 1 câu SQL bất kỳ và trả về kết quả dạng CSV
    /// </summary>
    public string ExecuteQueryToCsv(string sql)
    {
        try
        {
            using var conn = new MySqlConnection(connectionString);
            using var cmd = new MySqlCommand(sql, conn);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            StringBuilder csv = new StringBuilder();

            // Header
            for (int i = 0; i < reader.FieldCount; i++)
            {
                csv.Append(reader.GetName(i));
                if (i < reader.FieldCount - 1)
                    csv.Append(",");
            }

            csv.AppendLine();

            // Data rows
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string val = reader[i]?.ToString() ?? "";

                    // escape line breaks để CSV gọn
                    val = val.Replace("\r", " ").Replace("\n", " ");

                    // escape dấu phẩy
                    if (val.Contains(","))
                        val = $"\"{val}\"";

                    csv.Append(val);

                    if (i < reader.FieldCount - 1)
                        csv.Append(",");
                }

                csv.AppendLine();
            }

            return csv.ToString();
        }
        catch (MySqlException ex)
        {
            UnityEngine.Debug.LogError("❌ SQL Error: " + ex.Message);
            return string.Empty;
        }
    }

// #region Leaderboard
//     public string GetTeamRankingAsCsv()
//     {
// string query = @"
//             SELECT 
//                 t.team_id,
//                 t.team_name,
//
//                 -- Tổng số bàn thắng
//                 SUM(
//                     CASE 
//                         WHEN pm.home_team_id = t.team_id THEN po.home_score
//                         WHEN pm.away_team_id = t.team_id THEN po.away_score
//                         ELSE 0
//                     END
//                 ) AS total_goals,
//
//                 -- Số trận thắng sân khách
//                 SUM(
//                     CASE 
//                         WHEN pm.away_team_id = t.team_id AND po.away_score > po.home_score THEN 1
//                         ELSE 0
//                     END
//                 ) AS away_wins,
//
//                 -- Tổng số thẻ đỏ
//                 SUM(
//                     CASE 
//                         WHEN im.event_type = 'red_card' AND tp.team_id = t.team_id THEN 1
//                         ELSE 0
//                     END
//                 ) AS total_red_cards,
//
//                 -- Tổng số thẻ vàng
//                 SUM(
//                     CASE 
//                         WHEN im.event_type = 'yellow_card' AND tp.team_id = t.team_id THEN 1
//                         ELSE 0
//                     END
//                 ) AS total_yellow_cards
//
//             FROM team t
//             LEFT JOIN pre_match pm 
//                 ON t.team_id IN (pm.home_team_id, pm.away_team_id)
//             LEFT JOIN post_match po 
//                 ON pm.match_id = po.match_id
//             LEFT JOIN in_match im
//                 ON pm.match_id = im.match_id
//             LEFT JOIN team_player tp
//                 ON im.team_player_id = tp.id
//
//             GROUP BY t.team_id, t.team_name
//             ORDER BY total_goals DESC, away_wins DESC, total_red_cards ASC, total_yellow_cards ASC;
//         ";
//
//         using (var conn = new MySqlConnection(connectionString))
//         using (var cmd = new MySqlCommand(query, conn))
//         {
//             conn.Open();
//             using (var reader = cmd.ExecuteReader())
//             {
//                 StringBuilder csv = new StringBuilder();
//
//                 // Lấy header
//                 for (int i = 0; i < reader.FieldCount; i++)
//                 {
//                     csv.Append(reader.GetName(i));
//                     if (i < reader.FieldCount - 1)
//                         csv.Append(",");
//                 }
//                 csv.AppendLine();
//
//                 // Lấy data
//                 while (reader.Read())
//                 {
//                     for (int i = 0; i < reader.FieldCount; i++)
//                     {
//                         csv.Append(reader[i].ToString());
//                         if (i < reader.FieldCount - 1)
//                             csv.Append(",");
//                     }
//                     csv.AppendLine();
//                 }
//
//                 return csv.ToString();
//             }
//         }
//     }
// #endregion
}
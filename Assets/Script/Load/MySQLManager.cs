using UnityEngine;
using MySql.Data.MySqlClient; // cần DLL MySql.Data.dll
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class MySQLManager : MonoBehaviour
{
    public static MySQLManager Instance;

    // Connection string Railway hoặc MySQL Workbench
    // 👉 Railway: thay Server, Port, User, Password, Database theo config của Railway
    // 👉 Workbench local: thường là Server=localhost;Port=3306;User=root;Password=;Database=test;
    private string connectionString = "Server=turntable.proxy.rlwy.net;Port=24456;" +
                                      "Database=railway;" +
                                      "User=root;" +
                                      "Password=sUKWlixYrZDiqovdNpWetWOxCZQfXraj;" +
                                      "SslMode=None;" +
                                      "AllowPublicKeyRetrieval=True;";

    public MySqlConnection Conn;

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

        // print(ExportPlayersToCSV(true));
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

    /// <summary>
    /// For Test Purpose
    /// </summary>
    /// <param name="includeHeader"></param>
    /// <returns></returns>
    private string ExportPlayersToCSV(bool includeHeader)
    {
        try
        {
            string query = "SELECT * FROM player;";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            StringBuilder csv = new StringBuilder();

            // Header
            if (includeHeader)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    csv.Append(reader.GetName(i));
                    if (i < reader.FieldCount - 1) csv.Append(",");
                }
            }

            csv.AppendLine();

            // Data rows
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string value = reader[i].ToString().Replace(",", "&");
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

    public string GetRowByIndexAsCsv(string table, string column, object value)
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
                        val = dt.ToString("yyyy-MM-dd");
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


    public List<string> GetRowAsList(string table, int rowIndex)
    {
        var row = new List<string>();

        try
        {
            string query = $"SELECT * FROM `{table}` LIMIT 1 OFFSET {rowIndex};";
            using var cmd = new MySqlCommand(query, Conn);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object rawValue = reader[i];
                    string value;

                    if (rawValue is DateTime dt)
                        value = dt.ToString("yyyy-MM-dd");
                    else if (rawValue is bool b)
                        value = b ? "1" : "0";
                    else
                        value = rawValue?.ToString() ?? "";

                    row.Add(value);
                }
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("❌ MySQL Error: " + ex.Message);
        }

        return row;
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

            foreach (MySqlParameter param in cmd.Parameters)
            {
                print($"{param.ParameterName} = {param.Value}");
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

    public void InsertOneRow(string tableName, string csvLine, Action callback)
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
            for (int i = 1; i < columns.Count; i++)
            {
                colNames.Add(columns[i]);
                paramNames.Add("@" + columns[i]);
            }

            string sql =
                $"INSERT INTO {tableName} ({string.Join(", ", colNames)}) VALUES ({string.Join(", ", paramNames)})";
            print(sql);

            using var cmd = new MySqlCommand(sql, Conn);

            // Gán param động
            for (int i = 1; i < columns.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + columns[i], values[i]);
            }

            foreach (MySqlParameter param in cmd.Parameters)
            {
                print($"{param.ParameterName} = {param.Value}");
            }

            int rows = cmd.ExecuteNonQuery();
            Debug.Log($"✅ Insert row, affected {rows} rows");

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
        print(sql);

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
        print(sql);

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
}
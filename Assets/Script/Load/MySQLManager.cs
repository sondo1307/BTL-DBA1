using UnityEngine;
using MySql.Data.MySqlClient; // cần DLL MySql.Data.dll
using System;
using System.Collections.Generic;
using System.IO;
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

    public List<string> GetTableHeader(string table)
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

                    if (rawValue is DateTime dt) // Nếu là DateTime thì format dd/MM/yyyy
                    {
                        value = dt.ToString("yyyy-MM-dd");
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


    public void UpdateOneRow(string tableName, string csvLine)
    {
        var columns = GetTableHeader(tableName);
        
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
    }

}

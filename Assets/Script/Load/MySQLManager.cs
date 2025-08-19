using UnityEngine;
using MySql.Data.MySqlClient; // cần DLL MySql.Data.dll
using System;
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

        print(ExportPlayersToCSV(true));
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
    public string ExportPlayersToCSV(bool includeHeader)
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

    public string ReadTableHeaderAsCsv(string table)
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
    
    public string ReadTableDataAsCsv(string table)
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
}
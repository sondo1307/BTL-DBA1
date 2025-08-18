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

    private MySqlConnection conn;

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
            conn = new MySqlConnection(connectionString);
            conn.Open();
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
        if (conn != null && conn.State == System.Data.ConnectionState.Open)
        {
            conn.Close();
            conn.Dispose();
            Debug.Log("✅ Connection closed.");
        }
    }


    public string ExportPlayersToCSV(bool includeHeader)
    {
        try
        {
            string query = "SELECT * FROM player;";
            using var cmd = new MySqlCommand(query, conn);
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

    /// <summary>
    /// Đọc danh sách users
    /// </summary>
    public void ReadUsers()
    {
        const string query = "SELECT * FROM player;";
        using MySqlCommand cmd = new MySqlCommand(query, conn);
        using MySqlDataReader reader = cmd.ExecuteReader();

        // Print column names
        string header = "";
        for (int i = 0; i < reader.FieldCount; i++)
        {
            header += reader.GetName(i) + "\t";
        }

        Debug.Log("📋 Columns: " + header);

        // Print each row
        while (reader.Read())
        {
            string row = "";
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row += reader[i].ToString() + "\t";
            }

            Debug.Log(row);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class SonConst
{
    public const string DateFormat = "yyyy-MM-dd";
    public const string PlayerTable = "player";
    public const string TeamTable = "team";
    public const string TeamPlayerTable = "team_player";
    public const string RefereeTable = "referee";
    public const string StadiumTable = "stadium";
    public const string CountryTable = "country";
    public const string PrematchTable = "pre_match";
    public const string InMatchTable = "in_match";
    public const string PostMatchTable = "post_match";
    public const string MatchPlayerLineupTable = "match_player_lineup";
    public const string MatchRefereeLineupTable = "match_referee_lineup";
}

public static class SonCache
{
    public static WaitForSeconds WaitSeconds = new WaitForSeconds(1);
    public static WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
}

public static class UIHelper
{
    public static void ShowCg(this CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.blocksRaycasts = true;
        cg.interactable = true;
        cg.gameObject.SetActive(true);
    }

    public static void HideCg(this CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        cg.gameObject.SetActive(false);
    }
}


public static class StringUtils
{
    // Convert multiple rows (List<List<string>>) to CSV string
    public static string ListOfListToCsv(List<List<string>> rows)
    {
        if (rows == null || rows.Count == 0) return string.Empty;

        var csvLines = rows.Select(ListToCsv);
        return string.Join("\n", csvLines);

        // Convert a single row (List<string>) to CSV line
        string ListToCsv(List<string> row)
        {
            if (row == null || row.Count == 0) return string.Empty;

            return string.Join(",", row.Select(v =>
            {
                // Escape commas and quotes
                if (v.Contains(",") || v.Contains("\""))
                    return $"\"{v.Replace("\"", "\"\"")}\"";
                return v;
            }));
        }
    }
    
    /// <summary>
    /// Convert a list of strings into a single CSV row.
    /// </summary>
    /// <param name="values">List of string values</param>
    /// <returns>CSV string</returns>
    public static string ConvertListToCsv(List<string> values)
    {
        if (values == null || values.Count == 0)
            return string.Empty;

        var safeValues = new List<string>();

        foreach (var v in values)
        {
            string val = v ?? "";

            // Escape quotes by doubling them
            if (val.Contains("\""))
                val = val.Replace("\"", "\"\"");

            // If contains comma, quote, or newline -> wrap with quotes
            if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                val = $"\"{val}\"";

            safeValues.Add(val);
        }

        return string.Join(",", safeValues);
    }


    // Hàm bỏ dấu + về lowercase, xử lý thêm chữ 'đ'
    private static string NormalizeNoDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        text = text.ToLowerInvariant();

        // chuẩn hóa Unicode -> tách ký tự + dấu
        string normalized = text.Normalize(NormalizationForm.FormD);

        var sb = new StringBuilder();
        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark) // bỏ dấu
                sb.Append(c);
        }

        var result = sb.ToString().Normalize(NormalizationForm.FormC);

        // Chuyển 'đ' (và 'Đ' nếu có) về 'd'
        result = result.Replace('\u0111', 'd').Replace('\u0110', 'D');

        return result;
    }

    // Hàm Contains không phân biệt hoa/thường + dấu
    public static bool ContainsNormalized(string source, string substring)
    {
        string s1 = NormalizeNoDiacritics(source);
        string s2 = NormalizeNoDiacritics(substring);

        // dùng IndexOf với Ordinal để so sánh chính xác sau normalize
        return s1.IndexOf(s2, StringComparison.Ordinal) >= 0;
    }

    public static string ConvertHeaderToDataGridHeader(string header)
    {
        // Tách theo dấu phẩy
        var parts = header.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var output = parts.Select(part => part.Trim()).Select(column => $"{column}|200|Text").ToList();

        return "[" + string.Join(",", output) + "]";
    }

    public static string ConvertDGHeaderStringToDGHeaderInputFieldForUpdate(string headerDG)
    {
        int matchIndex = 0;
        return Regex.Replace(headerDG, "Text", m =>
        {
            matchIndex++;
            return matchIndex == 1 ? m.Value : "InputField";
        });
    }

    public static string ConvertDGHeaderStringToDGHeaderInputFieldForInsert(string headerDG)
    {
        return Regex.Replace(headerDG, "Text", m => "InputField");
    }
}
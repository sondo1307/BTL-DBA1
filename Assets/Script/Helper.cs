using System;
using System.Globalization;
using System.Text;
using UnityEngine;


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
    // Hàm bỏ dấu + về lowercase, xử lý thêm chữ 'đ'
    public static string NormalizeNoDiacritics(string text)
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
}
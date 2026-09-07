using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static class DSV 
{
    private static readonly Regex CsvRegex = new (@"
        (?:^|,)
        (?:
            ""(?<Text>(?:[^""]|"""")*)""
            |
            (?<Text>[^,]*) 
        )
        (?=,|$)",
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
    );
    
    private static readonly Regex TsvRegex = new (
        @"\G(?:^|\t)(?:""(?<Text>(?:[^""]|"""")*)""|(?<Text>[^\t\r\n]*))",
        RegexOptions.Compiled
    );
    
    public static string[] ParseCsv(string record) => ParseWith(CsvRegex, record);
    public static string[] ParseTsv(string record) => ParseWith(TsvRegex, record);
    
    public static string[] SplitRecords(string content)
    {
        if (string.IsNullOrEmpty(content)) return Array.Empty<string>();
        if (content[0] == '\uFEFF') content = content.Substring(1);

        var records = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < content.Length && content[i + 1] == '"')
                {
                    // "" => 따옴표 두 개 그대로 보존 (디코딩 금지)
                    sb.Append('"');
                    sb.Append('"');
                    i++;
                }
                else
                {
                    // 경계 따옴표도 그대로 보존 + 상태 토글
                    sb.Append('"');
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (!inQuotes && (c == '\r' || c == '\n'))
            {
                if (c == '\r' && i + 1 < content.Length && content[i + 1] == '\n') i++; // CRLF
                records.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            sb.Append(c);
        }

        records.Add(sb.ToString());

        // 파일 끝의 완전 빈 줄 정리(옵션)
        int last = records.Count - 1;
        while (last >= 0 && records[last].Length == 0) last--;
        if (last < records.Count - 1)
            records.RemoveRange(last + 1, records.Count - (last + 1));

        return records.ToArray();
    }

    
    private static string[] ParseWith(Regex rx, string record)
    {
        if (record == null) return System.Array.Empty<string>();
        var fields = new List<string>();

        foreach (Match m in rx.Matches(record))
        {
            string value = m.Groups["Text"].Value;
            value = value.Replace("\"\"", "\"");
            fields.Add(value);
        }

        if (fields.Count == 0) return new[] { "" };
        return fields.ToArray();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace VoloAbp.OTel.Authors;

public static class AuthorExtensions
{
    public const string ScoringPropertyName = "Scoring";

    public const string RemarkPropertyName = "Remark";

    public const string EnableStatePropertyName = "EnableState";

    public static void SetScoring(this Author author, double scoring)
    {
        author.SetProperty(ScoringPropertyName, scoring);
    }
    public static double GetScoring(this Author author)
    {
        return author.GetProperty<int>(ScoringPropertyName);
    }

    public static void SetRemark(this Author author, string remark)
    {
        author.SetProperty(RemarkPropertyName, remark);
    }
    public static string? GetRemark(this Author author)
    {
        return author.GetProperty<string>(RemarkPropertyName);
    }

    public static void SetEnableState(this Author author, bool enableState)
    {
        author.SetProperty(EnableStatePropertyName, enableState);
    }
    public static bool GetEnableState(this Author author)
    {
        return author.GetProperty<bool>(EnableStatePropertyName);
    }
}

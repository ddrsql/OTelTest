using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace VoloAbp.OTel.EntityFrameworkCore;


public class MyExtraPropertyDictionaryValueComparer : ValueComparer<ExtraPropertyDictionary>
{
    public MyExtraPropertyDictionaryValueComparer()
        : base(
            (a, b) => Compare(a, b),
            d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())),
            d => new ExtraPropertyDictionary(d))
    {
    }

    private static bool Compare(ExtraPropertyDictionary? a, ExtraPropertyDictionary? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        if (a is null)
        {
            return b is null;
        }

        if (b is null)
        {
            return false;
        }

        return a!.SequenceEqual(b!);
    }
}

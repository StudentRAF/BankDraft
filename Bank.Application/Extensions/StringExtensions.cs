using System;

namespace Bank.Application.Extensions;

public static class StringExtensions
{
    public static bool TryParseUINToDate(this string value, out DateOnly date)
    {
        var day       = value.Substring(0, 2);
        var month     = value.Substring(2, 2);
        var year      = value.Substring(4, 3);
        var yearDigit = year[0] == '9' ? "1" : "2";

        var result = DateOnly.TryParse($"{yearDigit}{year}-{month}-{day}", out var dateOnly);

        date = dateOnly;

        return result;
    }
}

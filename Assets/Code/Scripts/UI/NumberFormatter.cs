using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberFormatter
{
    public static string Format(double num)
    {
        return FormatToEngineering(num);    //Here can be inserted controlling format with settings
    }

    private static string FormatToEngineering(double num)
    {
        if (num == 0 || (Math.Abs(num) >= 1 && Math.Abs(num) <= 1e3))
        {
            return string.Format("{0:###.##}",num);
        }

        int exponent = 0;
        while (Math.Abs(num) > 1e3)
        {
            num = num / 1000.0;
            exponent += 3;
        }
        while (num != 0 && Math.Abs(num) < 1)
        {
            num = num * 1000.0;
            exponent -= 3;
        }
        String rounded = RoundToSignificantDigits(num, 3).ToString();
        if (rounded.Length == 1) { rounded = rounded + ".00"; }
        if (rounded.Length == 3 && rounded.Contains(".")) { rounded = rounded + "0"; }
        return String.Format("{0}e{1}", rounded, exponent);
    }

    private static string naukowa1(double num)
    {
        return num.ToString("e2").Replace("+00", "").Replace("+0", "").Replace("+", "").Replace("-0", "-").Replace("-0", "-").Replace("+", "").Replace("e0", "");
    }

    private static string FormatToScientific(double num)
    {
        string[] parts = num.ToString("e2").Split('e');
        parts[1] = double.Parse(parts[1]).ToString();
        if (parts[1] != "0")
        {
            return string.Format("{0}e{1}", parts[0], parts[1]);
        }
        return string.Format("{0}", parts[0], parts[1]);
    }

    private static double RoundToSignificantDigits(double d, int digits)
    {
        if (d == 0)
            return 0;

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
        return scale * Math.Round(d / scale, digits);
    }
}

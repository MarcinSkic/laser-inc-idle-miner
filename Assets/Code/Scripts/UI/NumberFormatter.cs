using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberFormatter
{
    public static string Format(double num)
    {
        if (SettingsModel.Instance.UseAlternativeNotation)
        {
            return FormatToEngineering(num);
        }
        return naukowa1(num);    //Here can be inserted controlling format with settings
    }

    public static string FormatSecondsToReadable(double seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);
        return string.Format(t.Days > 3 ? "{0:D}days" : "{1:D2}h:{2:D2}m:{3:D2}s",
            t.Days,
            t.Hours,
            t.Minutes,
            t.Seconds);
    }

    public static string FormatSecondsToHours(double seconds)
    {
        TimeSpan t= TimeSpan.FromSeconds(seconds);
        return string.Format(t.TotalHours <= 1 ? "{0} hour" : "{0} hours",t.TotalHours);
    }

    private static string FormatToEngineering(double num)
    {
        if ((Math.Abs(num) > 1e-3 && Math.Abs(num) < 1) || num == 0 || (Math.Abs(num) >= 1 && Math.Abs(num) <= 1e3))
        {
            return string.Format("{0:##0.##}",num);
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
        string suffix = exponent.ToString();
        switch (suffix)
        {
            case "3":
                suffix = "K";
                break;
            case "6":
                suffix = "M";
                break;
            case "9":
                suffix = "B";
                break;
            case "12":
                suffix = "T";
                break;
            case "15":
                suffix = "Qa";
                break;
            case "18":
                suffix = "Qi";
                break;
            case "21":
                suffix = "Sx";
                break;
            case "24":
                suffix = "Sp";
                break;
            case "27":
                suffix = "Oc";
                break;
            case "30":
                suffix = "N";
                break;
            default:
                return string.Format("{0}e{1}", rounded, suffix);
        }
        return string.Format("{0}{1}", rounded, suffix);
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

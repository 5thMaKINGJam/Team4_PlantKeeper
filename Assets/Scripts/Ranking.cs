using System;
using UnityEngine;
using System.Globalization;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class DateTimeConverter
{
    public static string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public static DateTime StringToDateTime(string dateTimeString)
    {
        try
        {
            return DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            Debug.Log("Invalid date format. Expected format: yyyy-MM-dd HH:mm:ss");
            return DateTime.MaxValue;
        }
    }
}


[System.Serializable]
public class Ranking : IComparable<Ranking>
{
    public string _id;
    public string name;
    public int time;
    public DateTime createdAt;
    public string createdAtStr;
    public Ranking() { }

    public Ranking(string name, int time)
    {
        this.name = name;
        this.time = time;
        this.createdAt = DateTime.Now;
        this.createdAtStr = DateTimeConverter.DateTimeToString(this.createdAt);
    }
    public Ranking(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
        createdAt = DateTimeConverter.StringToDateTime(createdAtStr);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public int CompareTo(Ranking other)
    {
        int timeComparison = this.time.CompareTo(other.time);
        if (timeComparison != 0)
        {
            return timeComparison;
        }
        // time이 같다면 createdAt을 기준으로 비교
        return this.createdAt.CompareTo(other.createdAt);
    }
}

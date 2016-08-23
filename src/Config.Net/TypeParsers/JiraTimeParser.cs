using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Config.Net.TypeParsers
{
   class JiraTimeParser : ITypeParser
   {
      private static readonly Regex TimeRegex = new Regex("^((?<days>\\d+)d){0,1}((?<hours>\\d+)h){0,1}((?<minutes>\\d+)m){0,1}((?<seconds>\\d+)s){0,1}((?<milliseconds>\\d+)ms){0,1}$");

      private static int GetValue(Match m, string groupName)
      {
         Group g = m.Groups[groupName];
         if (!g.Success) return 0;
         int r;
         int.TryParse(g.ToString(), out r);
         return r;
      }

      public bool TryParse(string value, Type t, out object result)
      {
         if (value == null)
         {
            result = null;
            return false;
         }

         Match m = TimeRegex.Match(value);
         if (!m.Success)
         {
            result = null;
            return false;
         }

         int days = GetValue(m, "days");
         int hours = GetValue(m, "hours");
         int minutes = GetValue(m, "minutes");
         int seconds = GetValue(m, "seconds");
         int milliseconds = GetValue(m, "milliseconds");
         var timeSpan = new TimeSpan(days, hours, minutes, seconds, milliseconds);

         result = new JiraTime(timeSpan);
         return true;

      }
      /// <summary>
      /// This method returns human readable string for a given JiraTime.
      /// Currently we have no way of identifying string values which are parsed in TryParse method so we
      /// can only return basic human readable string value from ToRawString method.
      /// </summary>
      /// <param name="value">JiraTime value</param>
      /// <returns>Basic human readable string value. eg. 1d/2h/3m/4s/5ms/1d2h3m4s</returns>
      public string ToRawString(object value)
      {
         if (value == null) return null;

         return ToDetailedString(((JiraTime)value).TimeSpan);
      }

      internal static string ToDetailedString(TimeSpan timeSpan)
      {
         if (timeSpan == null)
            throw new ArgumentNullException("timeSpan");

         var sb = new StringBuilder(30);

         string current = GetFormatString(timeSpan, TimeSpanComponents.Days);

         if (!string.IsNullOrEmpty(current)) sb.Append(current);

         current = GetFormatString(timeSpan, TimeSpanComponents.Hours);

         if (!string.IsNullOrEmpty(current)) sb.Append(current);

         current = GetFormatString(timeSpan, TimeSpanComponents.Minutes);

         if (!string.IsNullOrEmpty(current)) sb.Append(current);

         current = GetFormatString(timeSpan, TimeSpanComponents.Seconds);

         if (!string.IsNullOrEmpty(current)) sb.Append(current);

         current = GetFormatString(timeSpan, TimeSpanComponents.Milliseconds);

         if (!string.IsNullOrEmpty(current)) sb.Append(current);

         return sb.ToString();
      }

      private static string GetFormatString(TimeSpan formatTimeSpan, TimeSpanComponents components)
      {
         if (formatTimeSpan == null)
            throw new ArgumentNullException("formatTimeSpan");

         switch (components)
         {
            case TimeSpanComponents.Days: 
               return formatTimeSpan.Days == 0 ? string.Empty : formatTimeSpan.Days + "d";

            case TimeSpanComponents.Hours: 
               return formatTimeSpan.Hours == 0 ? string.Empty : formatTimeSpan.Hours + "h";

            case TimeSpanComponents.Minutes: 
               return formatTimeSpan.Minutes == 0 ? string.Empty : formatTimeSpan.Minutes + "m";

            case TimeSpanComponents.Seconds: 
               return formatTimeSpan.Seconds == 0 ? string.Empty : formatTimeSpan.Seconds + "s";

            case TimeSpanComponents.Milliseconds: 
               return formatTimeSpan.Milliseconds == 0 ? string.Empty : formatTimeSpan.Milliseconds + "ms";

            default:
               return string.Empty;
         }
      }
      private enum TimeSpanComponents
      {
         Days,
         Hours,
         Minutes,
         Seconds,
         Milliseconds
      }

   }
}

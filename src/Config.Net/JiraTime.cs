using System;
using Config.Net.TypeParsers;

namespace Config.Net
{
   /// <summary>
   /// Jira compatible time expression, allowed modifiers
   /// d - day
   /// h - hour
   /// m - minutes
   /// s - seconds
   /// ms - milliseconds
   /// </summary>
   /// <example>
   /// 1d1h2m3s - 1 day 1 hour 2 minutes and 3 seconds
   /// 5m3ms - 5 minutes 3 milliseconds
   /// </example>
   public class JiraTime : IComparable
   {
      public JiraTime(TimeSpan timeSpan)
      {
         this.TimeSpan = timeSpan;
      }

      private JiraTime(string humanReadableExpression)
      {
         if (humanReadableExpression == null) throw new ArgumentNullException(nameof(humanReadableExpression));

         JiraTime r;
         if (!new JiraTimeParser().TryParse(humanReadableExpression, out r))
            throw new ArgumentException("unparseable expression: " + humanReadableExpression, nameof(humanReadableExpression));
         TimeSpan = r.TimeSpan;
      }

      public static JiraTime FromHumanReadableString(string s)
      {
         if (s == null) return null;
         return new JiraTime(s);
      }

      public TimeSpan TimeSpan { get; private set; }

      public override string ToString()
      {
         return TimeSpan == TimeSpan.Zero ? "<zero>" : JiraTimeParser.ToDetailedString(TimeSpan);
      }

      public int CompareTo(object obj)
      {
         var that = obj as JiraTime;
         if (that == null) throw new ArgumentOutOfRangeException("expected " + typeof(JiraTime).FullName);
         return TimeSpan.CompareTo(that.TimeSpan);
      }
   }
}

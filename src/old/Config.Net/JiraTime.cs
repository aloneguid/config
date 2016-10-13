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
      /// <summary>
      /// Constructs an instance
      /// </summary>
      /// <param name="timeSpan"></param>
      public JiraTime(TimeSpan timeSpan)
      {
         this.TimeSpan = timeSpan;
      }

      private JiraTime(string humanReadableExpression)
      {
         if (humanReadableExpression == null) throw new ArgumentNullException(nameof(humanReadableExpression));

         object robj;
         JiraTime r;
         if (!new JiraTimeParser().TryParse(humanReadableExpression, typeof(JiraTime), out robj))
            throw new ArgumentException("unparseable expression: " + humanReadableExpression, nameof(humanReadableExpression));
         r = (JiraTime)robj;
         TimeSpan = r.TimeSpan;
      }

      /// <summary>
      /// Constructs an instance from human readable representation
      /// </summary>
      /// <param name="s"></param>
      /// <returns></returns>
      public static JiraTime FromHumanReadableString(string s)
      {
         if (s == null) return null;
         return new JiraTime(s);
      }

      /// <summary>
      /// Value as <see cref="TimeSpan"/>
      /// </summary>
      public TimeSpan TimeSpan { get; private set; }

      /// <summary>
      /// Converts to string
      /// </summary>
      public override string ToString()
      {
         return TimeSpan == TimeSpan.Zero ? "<zero>" : JiraTimeParser.ToDetailedString(TimeSpan);
      }

      /// <summary>
      /// Comparison
      /// </summary>
      public int CompareTo(object obj)
      {
         var that = obj as JiraTime;
         if (that == null) throw new ArgumentOutOfRangeException("expected " + typeof(JiraTime).FullName);
         return TimeSpan.CompareTo(that.TimeSpan);
      }
   }
}

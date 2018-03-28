using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Config.Net
{
   static class Utils
   {
      public static NetworkCredential ToNetworkCredential(this string s)
      {
         if (s == null) return null;

         var credsAndDomain = SplitByDelimiter(s, "@");
         string creds = credsAndDomain.Item1;
         string domain = credsAndDomain.Item2;

         var usernameAndPassword = SplitByDelimiter(creds, ":");
         string username = usernameAndPassword.Item1;
         string password = usernameAndPassword.Item2;

         return new NetworkCredential(username, password, domain);
      }

      public static string ToFriendlyString(NetworkCredential credential)
      {
         if (credential == null) return null;

         string usernameAndPassword;

         if (string.IsNullOrEmpty(credential.UserName) && string.IsNullOrEmpty(credential.Password))
         {
            usernameAndPassword = string.Empty;
         }
         else if (string.IsNullOrEmpty(credential.UserName))
         {
            usernameAndPassword = $":{credential.Password}";
         }
         else if (string.IsNullOrEmpty(credential.Password))
         {
            usernameAndPassword = credential.UserName;
         }
         else
         {
            usernameAndPassword = $"{credential.UserName}:{credential.Password}";
         }

         return string.IsNullOrEmpty(credential.Domain)
            ? usernameAndPassword
            : $"{usernameAndPassword}@{credential.Domain}";
      }

      public static Tuple<string, string> SplitByDelimiter(string s, params string[] delimiter)
      {
         if (s == null) return null;

         string key, value;

         if (delimiter == null || delimiter.Length == 0)
         {
            key = s.Trim();
            value = null;
         }
         else
         {

            List<int> indexes = delimiter.Where(d => d != null).Select(d => s.IndexOf(d)).Where(d => d != -1).ToList();

            if (indexes.Count == 0)
            {
               key = s.Trim();
               value = null;
            }
            else
            {
               int idx = indexes.OrderBy(i => i).First();
               key = s.Substring(0, idx);
               value = s.Substring(idx + 1);
            }
         }

         return new Tuple<string, string>(key, value);
      }
   }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Config.Net.Tests
{
   public class PerformanceExperimentTest
   {
      public class Poco
      {
         public string S { get; set; }

         public int I { get; set; }
      }

      [Fact]
      public void Reflect_and_assign_plain()
      {
         Type t = typeof(Poco);
         PropertyInfo[] pi = t.GetTypeInfo().GetProperties();

         for(int i = 0; i < 100000; i++)
         {
            string s = i.ToString();

            object o = Activator.CreateInstance(typeof(Poco));
            pi[0].SetValue(o, s);
            pi[1].SetValue(o, i);
         }
      }

   }

}

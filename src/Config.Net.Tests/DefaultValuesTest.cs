using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Config.Net.Tests
{
   /// <summary>
   /// Test related to DefaultValueAttribute
   /// </summary>
   public class DefaultValuesTest
   {
      public interface IFixtureSettings
      {
         [DefaultValue("not set")]
         string UnitTestName { get; set; }

         [DefaultValue((sbyte)10)]
         sbyte NumberOfYears { get; set; }

         [DefaultValue((short)1000)]
         short NumberOfHours { get; set; }

         [DefaultValue(10)]
         int NumberOfMinutes { get; set; }

         [DefaultValue((long)1000000000)]
         long NumberOfSeconds { get; set; }

         [DefaultValue((byte)24)]
         byte HoursOfDay { get; set; }

         [DefaultValue((ushort)1440)]
         ushort MinutesOfDay { get; set; }

         [DefaultValue((uint)86400)]
         uint SecondsOfDay { get; set; }

         [DefaultValue((ulong)86400000000)]
         ulong MicroSecondsOfDay { get; set; }

         [DefaultValue("Japan Denmark Australia")]
         string[] Regions { get; set; }

         [DefaultValue(12345.67f)]
         float TotalSeconds7d { get; set; }

         [DefaultValue(12345678.1234567)]
         double TotalSeconds15d { get; set; }

         [DefaultValue("00:00:01")]
         TimeSpan PingInterval { get; set; }


         [DefaultValue(LogicTest.Grid.ZA)]
         LogicTest.Grid ActiveGrid { get; set; }

         [Option(DefaultValue = "me")]
         [DefaultValue("not me")]
         string DefaultValuePriority { get; set; }
      }

      private readonly IFixtureSettings _settings;

      public DefaultValuesTest()
      {
         var store = new TestStore();

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(store)
            .Build();
      }

      [Fact]
      public void Read_DefaultString_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.UnitTestName));
      }

      [Fact]
      public void Read_DefaultSByte_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.NumberOfYears));
      }

      [Fact]
      public void Read_DefaultShort_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.NumberOfHours));
      }

      [Fact]
      public void Read_DefaultInt_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.NumberOfMinutes));
      }

      [Fact]
      public void Read_DefaultLong_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.NumberOfSeconds));
      }

      [Fact]
      public void Read_DefaultByte_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.HoursOfDay));
      }

      [Fact]
      public void Read_DefaultUShort_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.MinutesOfDay));
      }

      [Fact]
      public void Read_DefaultUInt_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.SecondsOfDay));
      }

      [Fact]
      public void Read_DefaultULong_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.MicroSecondsOfDay));
      }

      [Fact]
      public void Read_DefaultStringArray_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.Regions), new[]{ "Japan", "Denmark", "Australia" });
      }

      [Fact]
      public void Read_DefaultFloat_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.TotalSeconds7d));
      }

      [Fact]
      public void Read_DefaultDouble_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.TotalSeconds15d));
      }

      [Fact]
      public void Read_DefaultTimeSpan_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.PingInterval), TimeSpan.Parse("00:00:01"));
      }

      [Fact]
      public void Read_DefaultGrid_Reads()
      {
         CheckProperty(nameof(IFixtureSettings.ActiveGrid), LogicTest.Grid.ZA);
      }

      [Fact]
      public void Read_TwoDefaultValueAttributes_ReadsOptionAttribute()
      {
         CheckProperty(nameof(IFixtureSettings.DefaultValuePriority), "me");
      }

      private void CheckProperty(string propertyName, object expectedValue = null)
      {
         object value = _settings.GetType().GetProperty(propertyName)?.GetValue(_settings);
         expectedValue = expectedValue ?? typeof(IFixtureSettings).GetProperty(propertyName)?.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault()?.Value;
         Assert.Equal(expectedValue, value);
      }
   }
}

using System;
using System.Net;
using Xunit;

namespace Config.Net.Tests
{
   /// <summary>
   /// These are the old tests from v1, still valuable!
   /// </summary>
   public class LogicTest
   {
      public enum Grid
      {
         IT,
         AC,
         UK,
         US,
         ZA
      }

      public interface IFixtureSettings
      {
         int NoAttributesInt { get; }

         [Option(DefaultValue = "not set")]
         string UnitTestName { get; set; }

         [Option(DefaultValue = 10)]
         int NumberOfMinutes { get; set; }

         [Option(DefaultValue = new[] { "Japan", "Denmark", "Australia" })]
         string[] Regions { get; set; }

         [Option(Name = "log-xml", DefaultValue = true)]
         bool LogXml { get; set;  }

         int? NumberOfMinutesMaybe { get; set;  }

         [Option(Name = "ping-interval", DefaultValue = "00:00:01")]
         TimeSpan PingInterval { get; set;  }

         [Option(Name = "ping-interval-nullable")]
         TimeSpan? NullablePingInterval { get; set;  }

         [Option(Name = "estimate", DefaultValue = "1hm")]
         JiraTime IssueEstimate { get; set;  }

         [Option(DefaultValue = Grid.ZA)]
         Grid ActiveGrid { get; set; }

         Grid? ActiveGridNullable { get; set; }

         Guid GuidNotSupported { get; }

         NetworkCredential SomeCreds { get; }
      }

      private TestStore _store;
      private IFixtureSettings _settings;

      public LogicTest()
      {
         _store = new TestStore();

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .Build();
      }


      [Fact]
      public void Read_ConfiguredValue_Returns()
      {
         _store.Map[nameof(IFixtureSettings.UnitTestName)] = "configured value";
         Assert.Equal("configured value", _settings.UnitTestName);
      }

      [Fact]
      public void Read_Integer_Reads()
      {
         _store.Map["NumberOfMinutes"] = "78";

         int minutes = _settings.NumberOfMinutes;
         Assert.Equal(78, minutes);
      }

      [Fact]
      public void Read_DefaultInteger_Reads()
      {
         Assert.Equal(10, (int)_settings.NumberOfMinutes);
      }

      [Fact]
      public void Read_StringArray_Reads()
      {
         _store.Map["Regions"] = "IT, UK, US";

         string[] regions = _settings.Regions;

         Assert.Equal(3, regions.Length);
      }

      [Fact]
      public void ReadBooleanTrueFalseTest()
      {
         _store.Map["log-xml"] = "true";
         Assert.True(_settings.LogXml);

         _store.Map["log-xml"] = "false";
         Assert.False(_settings.LogXml);
      }

      [Fact]
      public void ReadBooleanYesNoTest()
      {
         _store.Map["log-xml"] = "yes";
         Assert.True(_settings.LogXml);

         _store.Map["log-xml"] = "no";
         Assert.False(_settings.LogXml);         
      }

      [Fact]
      public void Read_PropertySyntax_Reads()
      {
         _store.Map["log-xml"] = "no";
         Assert.False(_settings.LogXml);
      }

      [Fact]
      public void ReadBoolean10Test()
      {
         _store.Map["log-xml"] = "1";
         Assert.True(_settings.LogXml);

         _store.Map["log-xml"] = "0";
         Assert.False(_settings.LogXml);
      }

      [Fact]
      public void TimeSpanParserTest()
      {
         _store.Map["ping-interval"] = "01:02:03";
         TimeSpan v = _settings.PingInterval;
         Assert.Equal(1, v.Hours);
         Assert.Equal(2, v.Minutes);
         Assert.Equal(3, v.Seconds);
      }

      [Fact]
      public void JiraTimeParserTest()
      {
         _store.Map["estimate"] = "1d4h";
         JiraTime time = _settings.IssueEstimate;
         Assert.Equal(1, time.TimeSpan.Days);
         Assert.Equal(4, time.TimeSpan.Hours);
         Assert.Equal(0, time.TimeSpan.Minutes);
         Assert.Equal(0, time.TimeSpan.Seconds);
         Assert.Equal(0, time.TimeSpan.Milliseconds);
      }

      [Fact]
      public void ReadEnum_NotInConfig_DefaultValue()
      {
         Grid grid = _settings.ActiveGrid;
         Assert.Equal(Grid.ZA, grid);
      }

      [Fact]
      public void ReadEnum_InConfig_ConfigValue()
      {
         _store.Map["ActiveGrid"] = "UK";
         Grid grid = _settings.ActiveGrid;
         Assert.Equal(Grid.UK, grid);
      }

      [Fact]
      public void ReadEnum_InConfigInWrongCase_ConfigValue()
      {
         _store.Map["ActiveGrid"] = "uK";
         Grid grid = _settings.ActiveGrid;
         Assert.Equal(Grid.UK, grid);
      }

      [Fact]
      public void ReadEnum_OutOfRange_DefaultValue()
      {
         _store.Map["ActiveGrid"] = "dfdsfdsfdsf";
         Grid grid = _settings.ActiveGrid;
         Assert.Equal(Grid.ZA, grid);
      }

      [Fact]
      public void ReadEnum_Null_DefaultValue()
      {
         _store.Map["ActiveGrid"] = null;
         Grid grid = _settings.ActiveGrid;
         Assert.Equal(Grid.ZA, grid);
      }

      [Fact]
      public void ReadNullableEnum_Null_Null()
      {
         Grid? grid = _settings.ActiveGridNullable;
         Assert.Null(grid);
      }

      [Fact]
      public void ReadNullableEnum_NotNull_CorrectValue()
      {
         _store.Map[nameof(IFixtureSettings.ActiveGridNullable)] = Grid.ZA.ToString();
         Assert.Equal(Grid.ZA, (Grid)_settings.ActiveGridNullable);
      }

      [Fact]
      public void ReadNullableEnum_OutOfRange_Null()
      {
         _store.Map[nameof(IFixtureSettings.ActiveGridNullable)] = "Out Of Range";
         Assert.Null(_settings.ActiveGridNullable);
      }

      [Fact]
      public void ReadNullableInt_Null_Null()
      {
         int? value = _settings.NumberOfMinutesMaybe;
         Assert.Null(value);
      }

      [Fact]
      public void ReadNullableInt_NotNull_CorrectValue()
      {
         _store.Map[nameof(IFixtureSettings.NumberOfMinutesMaybe)] = "9";
         Assert.Equal(9, (int)_settings.NumberOfMinutesMaybe);
      }

      /// <summary>
      /// Previously this operation would fail because ConfigManager would compare the cached value to
      /// a newly read one and fail because string arrays don't implement IComparable
      /// </summary>
      [Fact]
      public void ReadStringArray_Twice_DoesntFail()
      {
         _store.Map["Regions"] = "IT, UK, US";

         string[] v = _settings.Regions;
         v = _settings.Regions;
      }

      [Fact]
      public void WriteStringTest()
      {
         const string writeValue = "SomeValue";
         _settings.UnitTestName = writeValue;
         
         Assert.Equal(writeValue, (string)_settings.UnitTestName);
      }

      [Fact]
      public void WriteStringArrayTest()
      {
         string[] writeValue = {"Japan", "Denmark", "Australia"};
         _settings.Regions = writeValue;
         
         Assert.Equal(writeValue, (string[])_settings.Regions);
      }

      [Fact]
      public void WriteIntTest()
      {
         const int writeValue = 23;
         _settings.NumberOfMinutes = writeValue;

         Assert.Equal(writeValue, (int)_settings.NumberOfMinutes);
      }

      [Fact]
      public void WriteBoolTest()
      {
         const bool writeValue = false;
         _settings.LogXml = writeValue;

         Assert.Equal(writeValue, (bool)_settings.LogXml);
      }

      [Fact]
      public void WriteTimeSpanTest()
      {
         TimeSpan writeValue = TimeSpan.FromDays(23);
         _settings.PingInterval = writeValue;

         Assert.Equal(writeValue, (TimeSpan)_settings.PingInterval);
      }

      [Fact]
      public void WriteJiraTimeTest()
      {
         var writeValue = new JiraTime(TimeSpan.FromDays(17));
         _settings.IssueEstimate = writeValue;

         Assert.Equal(writeValue.ToString(), ((JiraTime)_settings.IssueEstimate).ToString());
      }

      [Fact]
      public void WriteEnumTest()
      {
         const Grid writeValue = Grid.UK;
         _settings.ActiveGrid = writeValue;

         Assert.Equal(writeValue, (Grid)_settings.ActiveGrid);
      }

      [Fact]
      public void WriteNullableIntTest()
      {
         _settings.NumberOfMinutesMaybe = null;
         
         Assert.Equal(null, (int?)_settings.NumberOfMinutesMaybe);
         _store.Map["NumberOfMinutesMaybe"] = "34";
         int? newWriteValue = 34;

         _settings.NumberOfMinutesMaybe = newWriteValue;

         Assert.Equal(newWriteValue, (int?)_settings.NumberOfMinutesMaybe);
      }

      [Fact]
      public void WriteNullableEnumTest()
      {
         _settings.ActiveGridNullable = null;
         Grid? value = _settings.ActiveGridNullable;

         Assert.Equal(null, (Grid?)_settings.ActiveGridNullable);
         
         Grid? newWriteValue = Grid.AC;

         _settings.ActiveGridNullable = newWriteValue;

         Assert.Equal(newWriteValue, (Grid?)_settings.ActiveGridNullable);
      }

      [Fact]
      public void WriteNullableTimeSpanTest()
      {
         _settings.NullablePingInterval = null;

         Assert.Equal(null, (TimeSpan?)_settings.NullablePingInterval);
      }

      [Fact]
      public void Reading_int_with_no_attributes_returns_zero()
      {
         Assert.Equal(0, _settings.NoAttributesInt);
      }
   }
}

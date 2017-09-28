using System;
using System.Net;
using Xunit;

namespace Config.Net.Tests
{
   /// <summary>
   /// These are the old tests from v1, still valuable!
   /// </summary>
   public class ConfigManagerTest
   {
      enum Grid
      {
         IT,
         AC,
         UK,
         US,
         ZA
      }

      class FixtureSettings : SettingsContainer
      {
         private IConfigStore _store;
         public FixtureSettings(IConfigStore store)
         {
            _store = store;
         }

         public readonly Option<string> UnitTestName = new Option<string>("UnitTestName", "not set");
         public readonly Option<int> NumberOfMinutes = new Option<int>("NumberOfMinutes", 10);
         public Option<string[]> Regions { get; } = new Option<string[]>("Regions", new[] { "Japan", "Denmark", "Australia" });
         public Option<bool> LogXml { get; } = new Option<bool>("log-xml", true);
         public Option<int?> NumberOfMinutesMaybe { get; } = new Option<int?>("NumberOfMinutesMaybe", null);
         public Option<TimeSpan> PingInterval { get; } = new Option<TimeSpan>("ping-interval", TimeSpan.FromMinutes(1));
         public Option<TimeSpan?> NullablePingInterval { get; } = new Option<TimeSpan?>("ping-interval-nullable", null);
         public Option<JiraTime> IssueEstimate { get; } = new Option<JiraTime>("estimate", JiraTime.FromHumanReadableString("1h2m"));
         public Option<Grid> ActiveGrid { get; } = new Option<Grid>("ActiveGrid", Grid.ZA);
         public Option<Grid?> ActiveGridNullable { get; } = new Option<Grid?>("ActiveGridMaybe", null);
         public Option<Guid> GuidNotSupported { get; } = new Option<Guid>("GuidSetting", Guid.Empty);
         public Option<NetworkCredential> SomeCreds { get; } = new Option<NetworkCredential>(new NetworkCredential("ivan", "pass32"));

         protected override void OnConfigure(IConfigConfiguration configuration)
         {
            configuration.AddStore(_store);
            configuration.CacheTimeout = TimeSpan.Zero;
         }
      }

      private TestStore _store;
      private FixtureSettings _settings;

      public ConfigManagerTest()
      {
         _store = new TestStore();
         _settings = new FixtureSettings(_store);
      }

      [Fact]
      public void Read_DefaultValue_Returns()
      {
         string v = _settings.UnitTestName;
         Assert.Equal(_settings.UnitTestName.DefaultValue, v);
      }

      [Fact]
      public void Read_ConfiguredValue_Returns()
      {
         _store.Map[_settings.UnitTestName.Name] = "configured value";
         Assert.Equal("configured value", (string)_settings.UnitTestName);
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
         _store.Map[_settings.ActiveGridNullable.Name] = Grid.ZA.ToString();
         Assert.Equal(Grid.ZA, (Grid)_settings.ActiveGridNullable);
      }

      [Fact]
      public void ReadNullableEnum_OutOfRange_Null()
      {
         _store.Map[_settings.ActiveGridNullable.Name] = "Out Of Range";
         Assert.Null((Grid?)_settings.ActiveGridNullable);
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
         _store.Map[_settings.NumberOfMinutesMaybe.Name] = "9";
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
         _settings.UnitTestName.Write(writeValue);
         
         Assert.Equal(writeValue, (string)_settings.UnitTestName);
      }

      [Fact]
      public void WriteStringArrayTest()
      {
         string[] writeValue = {"Japan", "Denmark", "Australia"};
         _settings.Regions.Write(writeValue);
         
         Assert.Equal(writeValue, (string[])_settings.Regions);
      }

      [Fact]
      public void WriteIntTest()
      {
         const int writeValue = 23;
         _settings.NumberOfMinutes.Write(writeValue);

         Assert.Equal(writeValue, (int)_settings.NumberOfMinutes);
      }

      [Fact]
      public void WriteBoolTest()
      {
         const bool writeValue = false;
         _settings.LogXml.Write(writeValue);

         Assert.Equal(writeValue, (bool)_settings.LogXml);
      }

      [Fact]
      public void WriteTimeSpanTest()
      {
         TimeSpan writeValue = TimeSpan.FromDays(23);
         _settings.PingInterval.Write(writeValue);

         Assert.Equal(writeValue, (TimeSpan)_settings.PingInterval);
      }

      [Fact]
      public void WriteJiraTimeTest()
      {
         var writeValue = new JiraTime(TimeSpan.FromDays(17));
         _settings.IssueEstimate.Write(writeValue);

         Assert.Equal(writeValue.ToString(), ((JiraTime)_settings.IssueEstimate).ToString());
      }

      [Fact]
      public void WriteEnumTest()
      {
         const Grid writeValue = Grid.UK;
         _settings.ActiveGrid.Write(writeValue);

         Assert.Equal(writeValue, (Grid)_settings.ActiveGrid);
      }

      [Fact]
      public void WriteNullableIntTest()
      {
         _settings.NumberOfMinutesMaybe.Write(null);
         
         Assert.Equal(null, (int?)_settings.NumberOfMinutesMaybe);
         _store.Map["NumberOfMinutesMaybe"] = "34";
         int? newWriteValue = 34;

         _settings.NumberOfMinutesMaybe.Write(newWriteValue);

         Assert.Equal(newWriteValue, (int?)_settings.NumberOfMinutesMaybe);
      }

      [Fact]
      public void WriteNullableEnumTest()
      {
         _settings.ActiveGridNullable.Write(null);
         Grid? value = _settings.ActiveGridNullable;

         Assert.Equal(null, (Grid?)_settings.ActiveGridNullable);
         
         Grid? newWriteValue = Grid.AC;

         _settings.ActiveGridNullable.Write(newWriteValue);

         Assert.Equal(newWriteValue, (Grid?)_settings.ActiveGridNullable);
      }

      [Fact]
      public void WriteNullableTimeSpanTest()
      {
         _settings.NullablePingInterval.Write(null);

         Assert.Equal(null, (TimeSpan?)_settings.NullablePingInterval);
      }

      [Fact]
      public void Write_SetSomeArrayValueAndThenSetToDefault_ReadsNull()
      {
         //Act
         string[] newValue = {"UK", "US"};

         //Arrange
         _settings.Regions.Write(newValue); //This is the first step so we write a non-default value
         _settings.Regions.Write(_settings.Regions.DefaultValue);

         //Assert
         Assert.Null(_store.Read("Regions"));
      }

      [Fact]
      public void Write_SetSomeIntValueAndThenSetToDefault_ReadsNull()
      {
         int newValue = 12;

         _settings.NumberOfMinutes.Write(newValue); //This is the first step so we write a non-default value
         _settings.NumberOfMinutes.Write(_settings.NumberOfMinutes.DefaultValue);

         Assert.Null(_store.Read("NumberOfMinutes"));
      }

      [Fact]
      public void Write_SetSomeEnumValueAndThenSetToDefault_ReadsNull()
      {
         Grid newValue = Grid.IT;

         _settings.ActiveGrid.Write(newValue); //This is the first step so we write a non-default value
         _settings.ActiveGrid.Write(_settings.ActiveGrid.DefaultValue);

         Assert.Null(_store.Read("ActiveGrid"));
      }

      [Fact]
      public void Write_SetSomeTimeSpanValueAndThenSetToDefault_ReadsNull()
      {
         TimeSpan newValue = new TimeSpan(1, 1, 1);

         _settings.PingInterval.Write(newValue); //This is the first step so we write a non-default value
         _settings.PingInterval.Write(_settings.PingInterval.DefaultValue);

         Assert.Null(_store.Read("ping-interval"));
      }

      [Fact]
      public void Read_StoreContainsEmptyString_ReadsDefaultValue()
      {
         _store.Write("key1", string.Empty);

         string value = _settings.UnitTestName;

         Assert.Equal(_settings.UnitTestName.DefaultValue, value);
      }
   }
}

using System;
using Config.Net.Stores;
using NUnit.Framework;

namespace Config.Net.Tests
{
   [TestFixture]
   class ConfigManagerTest
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

         public Option<string> UnitTestName = new Option<string>("UnitTestName", "not set");
         public Option<int> NumberOfMinutes = new Option<int>("NumberOfMinutes", 10);
         public Option<string[]> Regions = new Option<string[]>("Regions", new[] { "Japan", "Denmark", "Australia" });
         public Option<bool> LogXml = new Option<bool>("log-xml", true);
         public Option<int?> NumberOfMinutesMaybe = new Option<int?>("NumberOfMinutesMaybe", null);
         public Option<TimeSpan> PingInterval = new Option<TimeSpan>("ping-interval", TimeSpan.FromMinutes(1));
         public Option<JiraTime> IssueEstimate = new Option<JiraTime>("estimate", JiraTime.FromHumanReadableString("1h2m"));
         public Option<Grid> ActiveGrid = new Option<Grid>("ActiveGrid", Grid.ZA);
         public Option<Grid?> ActiveGridMaybe = new Option<Grid?>("ActiveGridMaybe", null);

         protected override void OnConfigure(IConfigConfiguration configuration)
         {
            configuration.AddStore(_store);
         }
      }

      private TestStore _store;
      private FixtureSettings _settings;

      [SetUp]
      public void SetUp()
      {
         _store = new TestStore();
         _settings = new FixtureSettings(_store);
      }

      [Test]
      public void Read_DefaultValue_Returns()
      {
         string v = _settings.UnitTestName;
         Assert.AreEqual(_settings.UnitTestName.DefaultValue, v);
      }

      [Test]
      public void Read_ConfiguredValue_Returns()
      {
         _store.Map[_settings.UnitTestName.Name] = "configured value";
         Assert.AreEqual("configured value", _settings.UnitTestName);
      }

      [Test]
      public void Read_Integer_Reads()
      {
         _store.Map["NumberOfMinutes"] = "78";

         int minutes = _settings.NumberOfMinutes;
         Assert.AreEqual(78, minutes);
      }

      [Test]
      public void Read_DefaultInteger_Reads()
      {
         Assert.AreEqual(10, _settings.NumberOfMinutes);
      }

      [Test]
      public void Read_StringArray_Reads()
      {
         _store.Map["Regions"] = "IT, UK, US";

         string[] regions = _settings.Regions;

         Assert.AreEqual(3, regions.Length);
      }

      [Test]
      public void ReadBooleanTrueFalseTest()
      {
         _store.Map["log-xml"] = "true";
         Assert.IsTrue(_settings.LogXml);

         _store.Map["log-xml"] = "false";
         Assert.IsFalse(_settings.LogXml);
      }

      [Test]
      public void ReadBooleanYesNoTest()
      {
         _store.Map["log-xml"] = "yes";
         Assert.IsTrue(_settings.LogXml);

         _store.Map["log-xml"] = "no";
         Assert.IsFalse(_settings.LogXml);         
      }

      [Test]
      public void Read_PropertySyntax_Reads()
      {
         _store.Map["log-xml"] = "no";
         Assert.IsFalse(_settings.LogXml);
      }

      [Test]
      public void ReadBoolean10Test()
      {
         _store.Map["log-xml"] = "1";
         Assert.IsTrue(_settings.LogXml);

         _store.Map["log-xml"] = "0";
         Assert.IsFalse(_settings.LogXml);
      }

      [Test]
      public void TimeSpanParserTest()
      {
         _store.Map["ping-interval"] = "01:02:03";
         TimeSpan v = _settings.PingInterval;
         Assert.AreEqual(1, v.Hours);
         Assert.AreEqual(2, v.Minutes);
         Assert.AreEqual(3, v.Seconds);
      }

      [Test]
      public void JiraTimeParserTest()
      {
         _store.Map["estimate"] = "1d4h";
         JiraTime time = _settings.IssueEstimate;
         Assert.AreEqual(1, time.TimeSpan.Days);
         Assert.AreEqual(4, time.TimeSpan.Hours);
         Assert.AreEqual(0, time.TimeSpan.Minutes);
         Assert.AreEqual(0, time.TimeSpan.Seconds);
         Assert.AreEqual(0, time.TimeSpan.Milliseconds);
      }

      [Test]
      public void ReadEnum_NotInConfig_DefaultValue()
      {
         Grid grid = _settings.ActiveGrid;
         Assert.AreEqual(Grid.ZA, grid);
      }

      [Test]
      public void ReadEnum_InConfig_ConfigValue()
      {
         _store.Map["ActiveGrid"] = "UK";
         Grid grid = _settings.ActiveGrid;
         Assert.AreEqual(Grid.UK, grid);
      }

      [Test]
      public void ReadEnum_InConfigInWrongCase_ConfigValue()
      {
         _store.Map["ActiveGrid"] = "uK";
         Grid grid = _settings.ActiveGrid;
         Assert.AreEqual(Grid.UK, grid);
      }

      [Test]
      public void ReadEnum_OutOfRange_DefaultValue()
      {
         _store.Map["ActiveGrid"] = "dfdsfdsfdsf";
         Grid grid = _settings.ActiveGrid;
         Assert.AreEqual(Grid.ZA, grid);
      }

      [Test]
      public void ReadEnum_Null_DefaultValue()
      {
         _store.Map["ActiveGrid"] = null;
         Grid grid = _settings.ActiveGrid;
         Assert.AreEqual(Grid.ZA, grid);
      }

      [Test]
      public void ReadNullableEnum_Null_Null()
      {
         Grid? grid = _settings.ActiveGridMaybe;
         Assert.IsNull(grid);
      }

      [Test]
      public void ReadNullableEnum_NotNull_CorrectValue()
      {
         _store.Map[_settings.ActiveGridMaybe.Name] = Grid.ZA.ToString();
         Assert.AreEqual(Grid.ZA, _settings.ActiveGridMaybe);
      }

      [Test]
      public void ReadNullableEnum_OutOfRange_Null()
      {
         _store.Map[_settings.ActiveGridMaybe.Name] = "Out Of Range";
         Assert.IsNull(_settings.ActiveGridMaybe);
      }

      [Test]
      public void ReadNullableInt_Null_Null()
      {
         int? value = _settings.NumberOfMinutesMaybe;
         Assert.IsNull(value);
      }

      [Test]
      public void ReadNullableInt_NotNull_CorrectValue()
      {
         _store.Map[_settings.NumberOfMinutesMaybe.Name] = "9";
         Assert.AreEqual(9, _settings.NumberOfMinutesMaybe);
      }

      [Test]
      public void ReadProperty_TwoInsances_BothUpdateValue()
      {
         _store.Map["NumberOfMinutes"] = "78";
         int minutes1 = _settings.NumberOfMinutes;
         Assert.AreEqual(78, minutes1);

         //now change property value and check it's updated in first and second instance
         _store.Map["NumberOfMinutes"] = "79";
         int minutes2 = _settings.NumberOfMinutes;
         Assert.AreEqual(79, (int)minutes2);
         Assert.AreEqual(79, (int)minutes1);
      }

      /// <summary>
      /// Previously this operation would fail because ConfigManager would compare the cached value to
      /// a newly read one and fail because string arrays don't implement IComparable
      /// </summary>
      [Test]
      public void ReadStringArray_Twice_DoesntFail()
      {
         _store.Map["Regions"] = "IT, UK, US";

         string[] v = _settings.Regions;
         v = _settings.Regions;
      }

      [Test]
      public void Write_WhenTypeNotSupported_ThrowsException()
      {
         Option<Guid> someGuidSetting = new Option<Guid>("MseTestGUID", Guid.NewGuid());

         Assert.Throws(typeof (ArgumentException), () => Cfg.Write(someGuidSetting, Guid.NewGuid()));
      }

      [Test]
      public void Write_WhenKeyNull_ThrowsException()
      {
         Assert.Throws(typeof(ArgumentNullException), () => Cfg.Write(null, "KeyIsNull"));
      }

      [Test]
      public void Write_Nullable_WhenTypeNotSupported_ThrowsException()
      {
         Option<Guid?> someNullableGuidSetting = new Option<Guid?>("MseTestNullableGUID", null);

         Assert.Throws(typeof(ArgumentException), () => Cfg.Write(someNullableGuidSetting, Guid.NewGuid()));
      }

      [Test]
      public void WriteStringTest()
      {
         const string writeValue = "SomeValue";
         Cfg.Write(UnitTestName, writeValue);
         
         Assert.AreEqual(writeValue, _settings.UnitTestName);
      }

      [Test]
      public void WriteStringArrayTest()
      {
         string[] writeValue = {"Japan", "Denmark", "Australia"};
         Cfg.Write(Regions, writeValue);
         
         Assert.AreEqual(writeValue, _settings.Regions);
      }

      [Test]
      public void WriteIntTest()
      {
         const int writeValue = 23;
         Cfg.Write(NumberOfMinutes, writeValue);

         Assert.AreEqual(writeValue, _settings.NumberOfMinutes);
      }

      [Test]
      public void WriteBoolTest()
      {
         const bool writeValue = false;
         Cfg.Write(LogXml, writeValue);

         Assert.AreEqual(writeValue, _settings.LogXml);
      }

      [Test]
      public void WriteTimeSpanTest()
      {
         TimeSpan writeValue = TimeSpan.FromDays(23);
         Cfg.Write(PingInterval, writeValue);

         Assert.AreEqual(writeValue, _settings.PingInterval);
      }

      [Test]
      public void WriteJiraTimeTest()
      {
         var writeValue = new JiraTime(TimeSpan.FromDays(17));
         Cfg.Write(IssueEstimate, writeValue);

         Assert.AreEqual(writeValue.ToString(), _settings.IssueEstimate.ToString());
      }

      [Test]
      public void WriteEnumTest()
      {
         const Grid writeValue = Grid.UK;
         Cfg.Write(ActiveGrid, writeValue);

         Assert.AreEqual(writeValue, _settings.ActiveGrid);
      }

      [Test]
      public void WriteNullableIntTest()
      {
         Cfg.Write(NumberOfMinutesMaybe, null);
         
         Assert.AreEqual(null, _settings.NumberOfMinutesMaybe);
         _store.Map["NumberOfMinutesMaybe"] = "34";
         int? newWriteValue = 34;

         Cfg.Write(NumberOfMinutesMaybe, newWriteValue);

         Assert.AreEqual(newWriteValue, _settings.NumberOfMinutesMaybe);
      }

      [Test]
      public void WriteNullableEnumTest()
      {
         Cfg.Write(ActiveGridMaybe, null);

         Assert.AreEqual(null, _settings.ActiveGridMaybe);
         
         Grid? newWriteValue = Grid.AC;

         Cfg.Write(ActiveGridMaybe, newWriteValue);

         Assert.AreEqual(newWriteValue, _settings.ActiveGridMaybe);
      }

      [Test]
      public void WriteNullableTimeSpanTest()
      {
         Option<TimeSpan?> someGuidSetting = new Option<TimeSpan?>("MseTestNullableTimeSpan", new TimeSpan(3, 5, 58));

         Cfg.Write(someGuidSetting, null);

         Assert.AreEqual(null, _settings.someGuidSetting);
      }

      [Test]
      public void Write_SetSomeArrayValueAndThenSetToDefault_ReadsNull()
      {
         //Act
         string[] newValue = {"UK", "US"};

         //Arrange
         Cfg.Write(Regions, newValue); //This is the first step so we write a non-default value
         Cfg.Write(Regions, Regions.DefaultValue);

         //Assert
         Assert.IsNull(_store.Read("Regions"));
      }

      [Test]
      public void Write_SetSomeIntValueAndThenSetToDefault_ReadsNull()
      {
         int newValue = 12;

         Cfg.Write(_settings.NumberOfMinutes, newValue); //This is the first step so we write a non-default value
         Cfg.Write(_settings.NumberOfMinutes, NumberOfMinutes.DefaultValue);

         Assert.IsNull(_store.Read("NumberOfMinutes"));
      }

      [Test]
      public void Write_SetSomeEnumValueAndThenSetToDefault_ReadsNull()
      {
         Grid newValue = Grid.IT;

         Cfg.Write(ActiveGrid, newValue); //This is the first step so we write a non-default value
         Cfg.Write(ActiveGrid, ActiveGrid.DefaultValue);

         Assert.IsNull(_store.Read("ActiveGrid"));
      }

      [Test]
      public void Write_SetSomeTimeSpanValueAndThenSetToDefault_ReadsNull()
      {
         TimeSpan newValue = new TimeSpan(1, 1, 1);

         Cfg.Write(PingInterval, newValue); //This is the first step so we write a non-default value
         Cfg.Write(PingInterval, PingInterval.DefaultValue);

         Assert.IsNull(_store.Read("ping-interval"));
      }

      [Test]
      public void RemoveStores_AddOneStoreReadsKey_RemoveStoreKeyWontRead()
      {
         var store = new InMemoryConfigStore();
         string keyName = Guid.NewGuid().ToString();
         var setting = new Option<string>(keyName, null);

         Assert.IsNull(_settings.setting);

         store.Write(keyName, "12345");
         Cfg.Configuration.AddStore(store);
         Assert.AreEqual("12345", _settings.setting);

         Cfg.Configuration.RemoveAllStores();
         Assert.IsNull(_settings.setting);
      }
   }
}

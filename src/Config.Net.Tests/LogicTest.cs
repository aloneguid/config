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
      #region Type Defines
      public enum Grid
      {
         IT,
         AC,
         UK,
         US,
         ZA
      }

      public interface IInvalidDefaultTypeAnnotationSettings
      {
         [Option(DefaultValue = (double)123)]
         int Value { get; }
      }

      public interface IDefaultValueAsStringSettings
      {
         [Option(DefaultValue = "123")]
         int Value { get; }
      }

      public interface IFixtureSettings
      {
         int NoAttributesInt { get; }

         [Option(DefaultValue = "not set")]
         string UnitTestName { get; set; }

         [Option(DefaultValue = (sbyte)10)]
         sbyte NumberOfYears { get; set; }

         [Option(DefaultValue = (short)1000)]
         short NumberOfHours { get; set; }

         [Option(DefaultValue = 10)]
         int NumberOfMinutes { get; set; }

         [Option(DefaultValue = (long)1000000000)]
         long NumberOfSeconds { get; set; }

         [Option(DefaultValue = (byte)24)]
         byte HoursOfDay { get; set; }

         [Option(DefaultValue = (ushort)1440)]
         ushort MinutesOfDay { get; set; }

         [Option(DefaultValue = (uint)86400)]
         uint SecondsOfDay { get; set; }

         [Option(DefaultValue = (ulong)86400000000)]
         ulong MicroSecondsOfDay { get; set; }

         [Option(DefaultValue = "Japan Denmark Australia")]
         string[] Regions { get; set; }

         [Option(Alias = "log-xml", DefaultValue = true)]
         bool LogXml { get; set; }

         byte? NumberOfYearsMaybe { get; set; }
         short? NumberOfHoursMaybe { get; set; }
         int? NumberOfMinutesMaybe { get; set; }
         long? NumberOfSecondsMaybe { get; set; }

         byte? HoursOfDayMaybe { get; set; }
         ushort? MinutesOfDayMaybe { get; set; }
         uint? SecondsOfDayMaybe { get; set; }
         ulong? MicroSecondsOfDayMaybe { get; set; }

         [Option(DefaultValue = 12345.67f)]
         float TotalSeconds7d { get; set; }
         [Option(DefaultValue = 12345678.1234567)]
         double TotalSeconds15d { get; set; }
         [Option(DefaultValue = "1234567.1234567890")]
         decimal TotalSeconds28d { get; set; }

         [Option(Alias = "ping-interval", DefaultValue = "00:00:01")]
         TimeSpan PingInterval { get; set; }

         [Option(Alias = "ping-interval-nullable")]
         TimeSpan? NullablePingInterval { get; set; }

         [Option(DefaultValue = Grid.ZA)]
         Grid ActiveGrid { get; set; }

         Grid? ActiveGridNullable { get; set; }

         Guid GuidNotSupported { get; }

         NetworkCredential SomeCreds { get; }
      }
      #endregion

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

      #region Integer Types Tests
      #region SByte Tests
      [Fact]
      public void Read_SByte_Reads()
      {
         _store.Map["NumberOfYears"] = "78";

         sbyte years = _settings.NumberOfYears;
         Assert.Equal(78, years);
      }

      [Fact]
      public void Read_Cached_SByte()
      {
         _store.Map["NumberOfYears"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal(78, _settings.NumberOfYears);

         _store.Map["NumberOfYears"] = "79";
         Assert.Equal(78, _settings.NumberOfYears); //still cached
      }

      [Fact]
      public void Read_DefaultSByte_Reads()
      {
         Assert.Equal(10, (sbyte)_settings.NumberOfYears);
      }
      #endregion

      #region Short Tests
      [Fact]
      public void Read_Short_Reads()
      {
         _store.Map["NumberOfHours"] = "78";

         short years = _settings.NumberOfHours;
         Assert.Equal(78, years);
      }

      [Fact]
      public void Read_Cached_Short()
      {
         _store.Map["NumberOfHours"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal(78, _settings.NumberOfHours);

         _store.Map["NumberOfHours"] = "79";
         Assert.Equal(78, _settings.NumberOfHours); //still cached
      }

      [Fact]
      public void Read_DefaultShort_Reads()
      {
         Assert.Equal(1000, (short)_settings.NumberOfHours);
      }
      #endregion

      #region Int Tests
      [Fact]
      public void Read_Integer_Reads()
      {
         _store.Map["NumberOfMinutes"] = "78";

         int minutes = _settings.NumberOfMinutes;
         Assert.Equal(78, minutes);
      }

      [Fact]
      public void Read_Cached_Integer()
      {
         _store.Map["NumberOfMinutes"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal(78, _settings.NumberOfMinutes);

         _store.Map["NumberOfMinutes"] = "79";
         Assert.Equal(78, _settings.NumberOfMinutes); //still cached
      }

      [Fact]
      public void Read_DefaultInteger_Reads()
      {
         Assert.Equal(10, (int)_settings.NumberOfMinutes);
      }
      #endregion

      #region Long Tests
      [Fact]
      public void Read_Long_Reads()
      {
         _store.Map["NumberOfSeconds"] = "78";

         long seconds = _settings.NumberOfSeconds;
         Assert.Equal(78, seconds);
      }

      [Fact]
      public void Read_Cached_Long()
      {
         _store.Map["NumberOfSeconds"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal(78, _settings.NumberOfSeconds);

         _store.Map["NumberOfSeconds"] = "79";
         Assert.Equal(78, _settings.NumberOfSeconds); //still cached
      }

      [Fact]
      public void Read_DefaultLong_Reads()
      {
         Assert.Equal(1000000000, (long)_settings.NumberOfSeconds);
      }
      #endregion

      #region SByte Tests
      [Fact]
      public void Read_Byte_Reads()
      {
         _store.Map["HoursOfDay"] = "78";

         byte hours = _settings.HoursOfDay;
         Assert.Equal<byte>(78, hours);
      }

      [Fact]
      public void Read_Cached_Byte()
      {
         _store.Map["HoursOfDay"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal<byte>(78, _settings.HoursOfDay);

         _store.Map["HoursOfDay"] = "79";
         Assert.Equal<byte>(78, _settings.HoursOfDay); //still cached
      }

      [Fact]
      public void Read_DefaultByte_Reads()
      {
         Assert.Equal(24, (sbyte)_settings.HoursOfDay);
      }
      #endregion

      #region Short Tests
      [Fact]
      public void Read_UShort_Reads()
      {
         _store.Map["MinutesOfDay"] = "78";

         ushort minutes = _settings.MinutesOfDay;
         Assert.Equal<ushort>(78, minutes);
      }

      [Fact]
      public void Read_Cached_UShort()
      {
         _store.Map["MinutesOfDay"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal<ushort>(78, _settings.MinutesOfDay);

         _store.Map["MinutesOfDay"] = "79";
         Assert.Equal<ushort>(78, _settings.MinutesOfDay); //still cached
      }

      [Fact]
      public void Read_DefaultUShort_Reads()
      {
         Assert.Equal<ushort>(1440, _settings.MinutesOfDay);
      }
      #endregion

      #region Int Tests
      [Fact]
      public void Read_UInt_Reads()
      {
         _store.Map["SecondsOfDay"] = "78";

         uint seconds = _settings.SecondsOfDay;
         Assert.Equal<uint>(78, seconds);
      }

      [Fact]
      public void Read_Cached_UInt()
      {
         _store.Map["SecondsOfDay"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal<uint>(78, _settings.SecondsOfDay);

         _store.Map["SecondsOfDay"] = "79";
         Assert.Equal<uint>(78, _settings.SecondsOfDay); //still cached
      }

      [Fact]
      public void Read_DefaultUInt_Reads()
      {
         Assert.Equal<uint>(86400, _settings.SecondsOfDay);
      }
      #endregion

      #region ULong Tests
      [Fact]
      public void Read_ULong_Reads()
      {
         _store.Map["MicroSecondsOfDay"] = "78";

         ulong microSeconds = _settings.MicroSecondsOfDay;
         Assert.Equal<ulong>(78, microSeconds);
      }

      [Fact]
      public void Read_Cached_ULong()
      {
         _store.Map["MicroSecondsOfDay"] = "78";

         _settings = new ConfigurationBuilder<IFixtureSettings>()
            .UseConfigStore(_store)
            .CacheFor(TimeSpan.FromMinutes(1))
            .Build();

         Assert.Equal<ulong>(78, _settings.MicroSecondsOfDay);

         _store.Map["MicroSecondsOfDay"] = "79";
         Assert.Equal<ulong>(78, _settings.MicroSecondsOfDay); //still cached
      }

      [Fact]
      public void Read_DefaultULong_Reads()
      {
         Assert.Equal<ulong>(86400000000, (ulong)_settings.MicroSecondsOfDay);
      }
      #endregion
      #endregion

      [Fact]
      public void Read_StringArray_Reads()
      {
         _store.Map["Regions"] = "IT UK US";

         string[] regions = _settings.Regions;

         Assert.Equal(3, regions.Length);
      }

      #region Bool Test
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
      #endregion

      [Fact]
      public void TimeSpanParserTest()
      {
         _store.Map["ping-interval"] = "01:02:03";
         TimeSpan v = _settings.PingInterval;
         Assert.Equal(1, v.Hours);
         Assert.Equal(2, v.Minutes);
         Assert.Equal(3, v.Seconds);
      }

      #region Enum Tests
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
      #endregion

      #region Nullable Int Tests
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
      #endregion

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
         string[] writeValue = { "Japan", "Denmark", "Australia" };
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

         Assert.Null((int?)_settings.NumberOfMinutesMaybe);
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

         Assert.Null((Grid?)_settings.ActiveGridNullable);

         Grid? newWriteValue = Grid.AC;

         _settings.ActiveGridNullable = newWriteValue;

         Assert.Equal(newWriteValue, (Grid?)_settings.ActiveGridNullable);
      }

      [Fact]
      public void WriteNullableTimeSpanTest()
      {
         _settings.NullablePingInterval = null;

         Assert.Null((TimeSpan?)_settings.NullablePingInterval);
      }

      [Fact]
      public void Reading_int_with_no_attributes_returns_zero()
      {
         Assert.Equal(0, _settings.NoAttributesInt);
      }

      [Fact]
      public void Setting_default_value_different_from_original_type_throws_exception()
      {
         var builder = new ConfigurationBuilder<IInvalidDefaultTypeAnnotationSettings>();

         Assert.Throws<InvalidCastException>(() => builder.Build());
      }

      [Fact]
      public void Setting_default_value_as_string_to_non_string_type_parses_it_out_correctly()
      {
         IDefaultValueAsStringSettings settings = new ConfigurationBuilder<IDefaultValueAsStringSettings>().Build();

         int value = settings.Value;

         Assert.Equal(123, value);
      }
   }
}

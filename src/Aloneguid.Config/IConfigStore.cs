namespace Aloneguid.Config
{
   public interface IConfigStore
   {
      string Name { get; }

      bool CanRead { get; }

      bool CanWrite { get; }

      string Read(string key);

      void Write(string key, string value);
   }
}

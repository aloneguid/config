using Config.Net.TypeParsers;
using System.Globalization;
using System.Net;
using System.Threading;
using Xunit;

namespace Config.Net.Tests.TypeParsers
{
   public class NetworkCredentialsParserTest
   {
      private static readonly ITypeParser TypeParser = new NetworkCredentialParser();

      [Theory]
      [InlineData("user:pass@domain")]
      public void ToNetworkCredential_WhenInputIsValid_ReturnValidStringCredentials(string rawValue)
      {
         object creds = null;
         Assert.True(TypeParser.TryParse(rawValue, typeof(NetworkCredential), out creds));
         var networkCredentials = (NetworkCredential) creds;

         Assert.Equal("user", networkCredentials.UserName);
         Assert.Equal("pass", networkCredentials.Password);
         Assert.Equal("domain", networkCredentials.Domain);
      }
   }
}

using System;
using System.Net;

namespace Config.Net.Tests.Virtual
{
   public interface ITestCredential
   {
      Uri AzureKeyVaultUri { get; }

      string AzureKeyVaultTenantId { get; }

      string AzureKeyVaultClientId { get; }

      string AzureKeyVaultSecret { get; }

      string AzDevOpsOrg { get; }

      string AzDevOpsProject { get; }

      string AzDevOpsPat { get; }

      string AzDeOpsVarId { get; }
   }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Config.Net.Core;

namespace Config.Net.Stores
{
   class AzureDevOpsVariableSetConfigStore : IConfigStore
   {
      private readonly HttpClient _http;
      private readonly string _groupUri;

      public AzureDevOpsVariableSetConfigStore(
         string organisationName, string projectName, string personalAccessToken, string variableGroupId)
      {
         if (organisationName is null)
         {
            throw new ArgumentNullException(nameof(organisationName));
         }

         if (projectName is null)
         {
            throw new ArgumentNullException(nameof(projectName));
         }

         if (personalAccessToken is null)
         {
            throw new ArgumentNullException(nameof(personalAccessToken));
         }

         if (variableGroupId is null)
         {
            throw new ArgumentNullException(nameof(variableGroupId));
         }

         _http = new HttpClient();
         _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));

         _groupUri = $"https://dev.azure.com/{organisationName}/{projectName}/_apis/distributedtask/variablegroups/{variableGroupId}?api-version=5.1-preview.1";
      }

      public bool CanRead => true;

      public bool CanWrite => false;

      public void Dispose()
      {

      }

      public string Read(string key)
      {
         if (key == null) return null;

         if (FlatArrays.IsArrayLength(key, k => GetRawValue(k), out int length))
         {
            return length.ToString();
         }

         if (FlatArrays.IsArrayElement(key, k => GetRawValue(k), out string element))
         {
            return element;
         }

         return GetRawValue(key);
      }

      private string GetRawValue(string key)
      {
         VariableSetModel vset = GetVariableSetModel();
         if (vset.variables == null) return null;

         VariableSetValue value = null;

         var dic = new Dictionary<string, VariableSetValue>(StringComparer.InvariantCultureIgnoreCase);
         foreach(KeyValuePair<string, VariableSetValue> item in vset.variables)
         {
            dic[item.Key] = item.Value;
         }

         dic.TryGetValue(key, out value);

         return value?.value;
      }

      public void Write(string key, string value)
      {
         throw new NotSupportedException();
      }

      private VariableSetModel GetVariableSetModel()
      {
         string json = _http.GetStringAsync(_groupUri).ConfigureAwait(false).GetAwaiter().GetResult();

         return JsonSerializer.Deserialize<VariableSetModel>(json);
      }


      #region Serialisation Models


      public class VariableSetModel
      {
         public Dictionary<string, VariableSetValue> variables { get; set; }
         public int id { get; set; }
         public string type { get; set; }
         public string name { get; set; }
         public string description { get; set; }
         public bool isShared { get; set; }
      }

      public class VariableSetValue
      {
         public string value { get; set; }

         public override string ToString() => value;
      }

      #endregion
   }
}

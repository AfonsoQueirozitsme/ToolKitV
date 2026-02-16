using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace ToolKitV.Models
{
    public class AuthService
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_URL = "https://setcqfbeelfkovrbnkrz.supabase.co/functions/v1/validate-license";
        private const string ANON_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNldGNxZmJlZWxma292cmJua3J6Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjY4NzkzMTMsImV4cCI6MjA4MjQ1NTMxM30.oySXIORA_kjwrFQGDO9iAsDg7u75yPsmKAOx0eWUYyw";
        private const string APP_NAME = "toolkit";
        private static readonly string LicenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.txt");

        public class LicenseRequest
        {
            public string license_key { get; set; }
            public string app_name { get; set; }
        }

        public class LicenseResponse
        {
            public bool valid { get; set; }
            public string message { get; set; }
            public OwnerData owner { get; set; }
        }

        public class OwnerData
        {
            public string username { get; set; }
            public string discord_id { get; set; }
        }

        public static string GetStoredLicense()
        {
            if (File.Exists(LicenseFilePath))
            {
                return File.ReadAllText(LicenseFilePath).Trim();
            }
            return null;
        }

        public static void SaveLicense(string key)
        {
            File.WriteAllText(LicenseFilePath, key);
        }

        public static async Task<LicenseResponse> ValidateLicenseAsync(string key)
        {
            try
            {
                var requestBody = new LicenseRequest
                {
                    license_key = key,
                    app_name = APP_NAME
                };

                var json = JsonSerializer.Serialize(requestBody);
                using (var request = new HttpRequestMessage(HttpMethod.Post, API_URL))
                {
                    request.Headers.Add("Authorization", "Bearer " + ANON_KEY);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.SendAsync(request);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        try 
                        {
                            return JsonSerializer.Deserialize<LicenseResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch
                        {
                            return new LicenseResponse { valid = false, message = "Erro ao processar resposta do servidor." };
                        }
                    }
                    else
                    {
                        return new LicenseResponse { valid = false, message = "Licença inválida ou erro no servidor." };
                    }
                }
            }
            catch (Exception ex)
            {
                return new LicenseResponse { valid = false, message = "Falha na ligação: " + ex.Message };
            }
        }
    }
}

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Supervisor.Models;

namespace Supervisor.Controllers
{
    [ApiController]
    [Route("Automation")]
    public class AutomationWebhookController : ControllerBase
    {
        private const string Sha1Prefix = "sha1=";

        private readonly byte[] _secretBytes;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public AutomationWebhookController(string secret)
        {
            _secretBytes = Encoding.ASCII.GetBytes(secret);
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        [Route("Webhook")]
        [HttpPost()]
        public async Task<IActionResult> ReceiveWebhookAsync()
        {
            string eventName = GetHeaderValue("X-GitHub-Event");

            if(eventName.ToString().Equals("check_run", StringComparison.OrdinalIgnoreCase))
            {
                string signature = GetHeaderValue("X-Hub-Signature");

                using var eventReader = new StreamReader(Request.Body);
                var eventPayload = await eventReader.ReadToEndAsync();

                if (IsGithubEventAllowed(eventPayload, eventName, signature))
                {
                    var gitHubAction = JsonSerializer.Deserialize<GitHubAction>(eventPayload);
                    NullGuard(gitHubAction, nameof(gitHubAction));
                    NullGuard(gitHubAction.CheckRun, nameof(gitHubAction.CheckRun));

                    // TODO: ... send to business logic
                }
                else
                {
                    //TODO: Notify home assistant. Could use the REST api of Home Assistant to start a script
                }
            }

            return Ok();
        }

        private bool IsGithubEventAllowed(string payload, string eventName, string signatureWithPrefix)
        {
            NullGuard(payload, nameof(payload));

            if (signatureWithPrefix.StartsWith(Sha1Prefix, StringComparison.OrdinalIgnoreCase))
            {
                var signature = signatureWithPrefix.Substring(Sha1Prefix.Length);

                var payloadBytes = Encoding.ASCII.GetBytes(payload);
                using var hmSha1 = new HMACSHA1(_secretBytes);
                var hash = hmSha1.ComputeHash(payloadBytes);
                var hashString = ToHexString(hash);

                return hashString.Equals(signature);
            }

            return false;
        }

        private static string ToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                builder.AppendFormat("{0:x2}", b);
            }

            return builder.ToString();
        }

        private string GetHeaderValue(string headerName)
        {
            Request.Headers.TryGetValue(headerName, out StringValues headerValues);
            string value = headerValues.ToString();
            NullGuard(value, headerName);
            return value;
        }

        private void NullGuard(object parameter, string parameterName)
        {
            if (parameter == null ||
                parameter is string stringParameter && string.IsNullOrWhiteSpace(stringParameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}

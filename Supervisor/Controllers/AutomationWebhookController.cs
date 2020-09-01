using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Supervisor.Controllers
{
    [ApiController]
    [Route("Automation")]
    public class AutomationWebhookController : ControllerBase
    {
        private const string Sha1Prefix = "sha1=";

        private readonly byte[] _secretBytes;

        public AutomationWebhookController(string secret)
        {
            _secretBytes = Encoding.ASCII.GetBytes(secret);
        }

        [Route("Webhook")]
        [HttpPost()]
        public async Task<IActionResult> ReceiveWebhookAsync()
        {
            Request.Headers.TryGetValue("X-GitHub-Event", out StringValues eventName);
            Request.Headers.TryGetValue("X-Hub-Signature", out StringValues signature);

            using var eventReader = new StreamReader(Request.Body);
            var eventPayload = await eventReader.ReadToEndAsync();

            if (IsGithubEventAllowed(eventPayload, eventName, signature))
            {
                Console.WriteLine("Received an approved event!");
            }
            else
            {
                 Console.WriteLine("Received an unapproved event :(");
            }

            return Ok();
        }

        private bool IsGithubEventAllowed(string payload, string eventName, string signatureWithPrefix)
        {
            NullGuard(payload, nameof(payload));
            NullGuard(eventName, nameof(eventName));
            NullGuard(signatureWithPrefix, nameof(signatureWithPrefix));

            if (!eventName.Equals("check_run", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

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

        private void NullGuard(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}

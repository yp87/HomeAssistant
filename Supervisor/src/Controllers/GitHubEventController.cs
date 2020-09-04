using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Supervisor.ActionHandlers;
using Supervisor.Models;

namespace Supervisor.Controllers
{
    [ApiController]
    [Route("Automation")]
    public class GitHubEventController : ControllerBase
    {
        private readonly byte[] _secretBytes;

        // If more then one type of action can be handled in the future,
        // This will be refactored to have an IActionDispatcher
        // Which will dispatch to the correct implementation of IActionHandler.
        // But for now, YAGNI!
        private readonly IActionHandler _actionHandler;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GitHubEventController(IActionHandler actionHandler, string secret)
        {
            _actionHandler = actionHandler;
            _secretBytes = Encoding.ASCII.GetBytes(secret);
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        [Route("Webhook")]
        [HttpPost()]
        public async Task<IActionResult> ReceiveEventAsync()
        {
            string eventName = GetHeaderValue(Constants.GitHubHeaderEvent);

            if (!_actionHandler.CanHandleAction(eventName))
            {
                throw new NotSupportedException($"The event {eventName} is not supported");
            }

            string signature = GetHeaderValue(Constants.GitHubHeaderSignature);

            var eventPayload = await ReadPayloadAsync();

            if (!IsGithubEventAllowed(eventPayload, eventName, signature))
            {
                //TODO: Notify home assistant. Could use the REST api of Home Assistant to start a script
                throw new UnauthorizedAccessException("The provided signature was not valid.");
            }

            return await ProcessActionAsync(eventPayload);
        }

        private async Task<IActionResult> ProcessActionAsync(string eventPayload)
        {
            var gitHubAction = JsonSerializer.Deserialize<GitHubAction>(eventPayload, _jsonSerializerOptions);
            NullGuard(gitHubAction, nameof(gitHubAction));
            await _actionHandler.HandleAsync(gitHubAction);
            return Ok();
        }

        private async Task<string> ReadPayloadAsync()
        {
            using var eventReader = new StreamReader(Request.Body);
            var eventPayload = await eventReader.ReadToEndAsync();
            NullGuard(eventPayload, nameof(eventPayload));
            return eventPayload;
        }

        private bool IsGithubEventAllowed(string payload, string eventName, string signatureWithPrefix)
        {
            if (!signatureWithPrefix.StartsWith(Constants.Sha1Prefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The signature does not start with the expected prefix.");
            }

            var signature = signatureWithPrefix.Substring(Constants.Sha1Prefix.Length);

            var payloadBytes = Encoding.ASCII.GetBytes(payload);
            using var hmSha1 = new HMACSHA1(_secretBytes);
            var hash = hmSha1.ComputeHash(payloadBytes);
            var hashString = StringHelpers.ToHexString(hash);

            return hashString.Equals(signature);
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

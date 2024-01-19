using AngleSharp.Common;
using AngleSharp.Html.Parser;
using Members.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.Result;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Members.Infrastructure.Services
{
    internal partial class GameVerificationService : IGameVerificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _gameProfileBaseAddress;
        private readonly ILogger _logger;

        public GameVerificationService(IHttpClientFactory aHttpClientFactory, IConfiguration aConfiguration, ILogger<GameVerificationService> aLogger)
        {
            _httpClientFactory = aHttpClientFactory;
            _logger = aLogger;
            _gameProfileBaseAddress = aConfiguration.GetValue<string>("GameProfileBaseResourceAddress")
                ?? throw new ArgumentNullException("Failed on fetching 'GameProfileBaseResourceAddress' from config, a value is required.");
        }

        public async Task<IHttpResult<Unit>> VerifyGameHandle(string aGameHandle, string aVerifyCode, CancellationToken aCancellationToken)
        {
            var lHttpClient = _httpClientFactory.CreateClient();
            lHttpClient.BaseAddress = new Uri(_gameProfileBaseAddress);

            var lResponse = await lHttpClient.GetAsync(aGameHandle, aCancellationToken);
            if (!lResponse.IsSuccessStatusCode)
                return Result.Failure<Unit>(InfrastructureErrors.GameHandleVerify.FetchHandleFailed);

            return await GetProfileBioFromResponse(lResponse, aCancellationToken)
                .Bind(bioContent => IsCodePresentInStringResult(bioContent, aVerifyCode));
        }

        private async Task<IHttpResult<string>> GetProfileBioFromResponse(HttpResponseMessage aHttpResponseMessage, CancellationToken aCancellationToken = default)
        {
            try
            {
                var lStringResponse = await aHttpResponseMessage.Content.ReadAsStringAsync(aCancellationToken);

                var lParser = new HtmlParser();
                var lHTMLDocument = await lParser.ParseDocumentAsync(lStringResponse, aCancellationToken);
                var lElementList = lHTMLDocument?.QuerySelector("div.entry.bio");
                var lDictionaryData = lElementList?.Children.Select(y => y.ToDictionary()).Last();

                return Result.SuccessHttp(lDictionaryData?["TextContent"])!;
            }
            catch (Exception lEx)
            {
                _logger.LogError(lEx, "An error occured while trying to get from the response the profile's bio under the provided game handle.");
                return Result.Failure<string>(InfrastructureErrors.GameHandleVerify.ErrorReadingBio);
            }
        }

        private static IHttpResult<Unit> IsCodePresentInStringResult(string? aBioText, string aVerifyCode)
        {
            if (string.IsNullOrEmpty(aVerifyCode) || aVerifyCode.Length != 6 || !HasSixDigitCodeRegex().IsMatch(aVerifyCode))
                throw new ArgumentException("Verification code was not a 6-digit number. This should never happen!");

            var lRegex = new Regex(aVerifyCode, RegexOptions.Compiled);
            return !string.IsNullOrEmpty(aBioText) && lRegex.IsMatch(aBioText)
                ? Result.SuccessHttp(Unit.Value)
                : Result.Failure<Unit>(InfrastructureErrors.GameHandleVerify.CodeNotFound);
        }

        [GeneratedRegex("^\\d{6}$")]
        private static partial Regex HasSixDigitCodeRegex();
    }
}

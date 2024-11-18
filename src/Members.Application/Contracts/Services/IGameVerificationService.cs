using TGF.Common.ROP;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.Contracts.Services
{
    public interface IGameVerificationService
    {
        /// <summary>
        /// Verifies the member ownership of a game account by checking if the verification code is present at the bio from "https://robertsspaceindustries.com/citizens/{Member.GameHandle}" under the member's GameHandle provided.
        /// </summary>
        /// <param name="aGameHandle">GameHandle used to find the user's game profile.</param>
        /// <param name="aVerifyCode">Verification code that should be present at the GameHandle profile's bio.</param>
        /// <returns>Result with success if the verification code was found in the GameHandle profile's bio, otherwise false.</returns>
        public Task<IHttpResult<Unit>> VerifyGameHandle(string aGameHandle, string aVerifyCode, CancellationToken aCancellationToken);
    }
}

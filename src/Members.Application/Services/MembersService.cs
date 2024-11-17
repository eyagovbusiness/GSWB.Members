using Common.Application.Contracts.Services;
using Common.Application.DTOs.Members;
using Common.Domain.ValueObjects;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories;
using Members.Domain.Entities;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Application.Services
{
    public class MembersService : IMembersService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ISwarmBotCommunicationService _SwarmBotCommunicationService;
        private readonly IGameVerificationService _gameVerificationService;
        public MembersService(
            IMemberRepository aMemberRepository,
            ISwarmBotCommunicationService aSwarmBotCommunicationService,
            IGameVerificationService aGameVerificationService)
        {
            _memberRepository = aMemberRepository;
            _SwarmBotCommunicationService = aSwarmBotCommunicationService;
            _gameVerificationService = aGameVerificationService;
        }

        #region IMembersService
        public async Task<IHttpResult<Member>> DeleteMember(MemberKey id, CancellationToken aCancellationToken = default)
         => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Bind(member => _memberRepository.DeleteAsync(member!, aCancellationToken));

        #region Verify

        public async Task<IHttpResult<MemberVerificationStateDTO>> Get_GetVerifyInfo(MemberKey id, CancellationToken aCancellationToken = default)
        => await _memberRepository.GetByIdAsync(id, aCancellationToken)
            .Bind(member => TryUpdateGameHandleVerifyCode(member!, aCancellationToken))
            .Map(member => new MemberVerificationStateDTO(member.IsGameHandleVerified, member!.GameHandleVerificationCode, member.VerificationCodeExpiryDate));

        public async Task<IHttpResult<MemberDetailDTO>> VerifyGameHandle(MemberKey id, CancellationToken aCancellationToken = default)
        {
            var lMemberResult = await _memberRepository.GetByIdAsync(id, aCancellationToken);
            return await lMemberResult.Verify(member => member?.GameHandle != null, ApplicationErrors.MemberValidation.GameHandleNotSet)
                .Verify(member => member!.VerificationCodeExpiryDate > DateTimeOffset.Now, ApplicationErrors.MemberValidation.GameHandleVerificationCodeExpired)
                .Bind(member => _gameVerificationService.VerifyGameHandle(member!.GameHandle!, member.GameHandleVerificationCode, aCancellationToken))
                .Bind(_ => UpdateIsVerified(lMemberResult.Value!, aCancellationToken));
        }

        #endregion

        #endregion

        #region Private

        private async Task<IHttpResult<Member>> TryUpdateGameHandleVerifyCode(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.TryRefreshGameHandleVerificationCode()
            ? await _memberRepository.UpdateAsync(aMember, aCancellationToken)
            : Result.SuccessHttp(aMember);

        private async Task<IHttpResult<MemberDetailDTO>> UpdateIsVerified(Member aMember, CancellationToken aCancellationToken = default)
        => aMember.Verify()
            ? await _memberRepository.UpdateAsync(aMember, aCancellationToken)
                .Map(member => member.ToDetailDto())
            : Result.Failure<MemberDetailDTO>(ApplicationErrors.MemberValidation.GameHandleVerificationFailed);

        #endregion

    }

}

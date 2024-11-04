using Members.Domain.Validation.Guild;
using System.Collections.Immutable;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Domain.Entities
{
    public partial class Member
    {
        public virtual ICollection<MemberRole> Roles { get; protected set; } = [];

        internal async Task<IHttpResult<Member>> AssignRoles(IEnumerable<ulong> roleIdList, GuildRoleIdValidator guildRoleIdValidator)
        {
            var lValidationResult = await guildRoleIdValidator.ValidateAsync(new GuildRoleIdValidationData(GuildId, roleIdList));

            if (!lValidationResult.IsValid)
                return Result.Failure<Member>(lValidationResult.Errors
                    .Select(e => ValidateSwitchExtensions.GetValidationError(e.ErrorCode, e.ErrorMessage))
                    .ToImmutableArray());

            roleIdList.Where(roleId => !Roles.Any(role => role.RoleId == roleId))
            .ToList()
            .ForEach(roleId => Roles.Add(new MemberRole() { Member = this, RoleId = roleId }));

            return Result.SuccessHttp(this);
        }

        internal IHttpResult<Member> RevokeRoles(IEnumerable<ulong> roleIdList)
        => Result.SuccessHttp(roleIdList)
            .Map(roleIdList => Roles.Where(memberRole => roleIdList.Contains(memberRole.RoleId)))
            .Tap(memberRoleList => Roles = Roles.Except(memberRoleList).ToList())
            .Map(_ => this);

    }
}

using FluentValidation;
using Members.Domain.Contracts.Repositories;

namespace Members.Domain.Validation.Guild
{
    public class GuildRoleIdValidator : AbstractValidator<GuildRoleIdValidationData>
    {
        private readonly IGuildRepository _guildRepository;
        public GuildRoleIdValidator(IGuildRepository guildRepository)
        {
            _guildRepository = guildRepository;
            RuleFor(roleIdListInfo => roleIdListInfo).MustAsync(AllRoleIdExistInGuild);
        }
        private async Task<bool> AllRoleIdExistInGuild(GuildRoleIdValidationData guildRoleIdValidationData, CancellationToken aCancellationToken = default)
        {
            var guildResult = await _guildRepository.GetGuildWithRoles(guildRoleIdValidationData.GuildId, aCancellationToken);
            if(!guildResult.IsSuccess)
                return false;
            return !guildRoleIdValidationData.RoleIdList
                .Except(guildResult.Value.Roles
                    .Select(role => role.RoleId))
                .Any();
        }

    }
    public record GuildRoleIdValidationData(ulong GuildId, IEnumerable<ulong> RoleIdList);
}

using Ardalis.Specification;
using Common.Domain.Validation;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TGF.CA.Application.Specifications;
using TGF.CA.Application.Validation;
using TGF.Common.ROP.HttpResult;
using TGF.Common.ROP.HttpResult.RailwaySwitches;
using TGF.Common.ROP.Result;

namespace Members.Application.Specifications
{
    public class GuildRolesPageSpecification : SortedAndPagedSpecification<Role>
    {
        public readonly string _guildId;
        private readonly DiscordIdValidator _discordIdValidator;

        public GuildRolesPageSpecification(
        string guildId,
        int? page, int? pageSize,
        string? sortBy, ListSortDirection? sortDirection,
        PaginationValidator paginationValidationRules, SortingValidator<Role> sortingValidationRules, DiscordIdValidator discordIdValidator,
        string? name
        )
            : base(page, pageSize, sortBy, sortDirection, paginationValidationRules, sortingValidationRules)
        {
            _guildId = guildId;
            _discordIdValidator = discordIdValidator;
            if (!string.IsNullOrEmpty(name))
                Query.Where(role => EF.Functions.Like(role.Name.ToLower(), $"%{name.ToLower()}%"));

        }
        public override IHttpResult<ISpecification<Role>> Apply()
        => Result.ValidationResult(_discordIdValidator.Validate(_guildId))
        .Tap(_ => Query.Where(role => role.GuildId == ulong.Parse(_guildId)))
        .Bind(_ => base.Apply());
    }
}

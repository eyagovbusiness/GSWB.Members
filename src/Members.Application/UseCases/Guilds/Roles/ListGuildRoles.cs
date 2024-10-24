using Ardalis.Specification;
using Common.Application.DTOs.Roles;
using Members.Application.Mapping;
using Members.Domain.Contracts.Repositories.ReadOnly;
using Members.Domain.Entities;
using TGF.CA.Application.UseCases;
using TGF.Common.ROP.HttpResult;

namespace Members.Application.UseCases.Guilds.Roles
{
    /// <summary>
    /// Use case to list roles
    /// </summary>
    public class ListGuildRoles(IRoleQueryRepository roleQueryRepository)
        : IUseCase<IHttpResult<IEnumerable<RoleDTO>>, ISpecification<Role>>
    {
        public async Task<IHttpResult<IEnumerable<RoleDTO>>> ExecuteAsync(ISpecification<Role> request, CancellationToken cancellationToken = default)
        => await roleQueryRepository.GetListAsync(request, cancellationToken)
        .Map(roles => roles.Select(role => role.ToDto()));
    }
}

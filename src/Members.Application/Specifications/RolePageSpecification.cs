
using Ardalis.Specification;
using Members.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TGF.CA.Application.Specifications;
using TGF.CA.Application.Validation;

namespace Members.Application.Specifications
{
    public class RolePageSpecification : SortedAndPagedSpecification<Role>
    {
        public RolePageSpecification(
        int? page, int? pageSize,
        string? sortBy, ListSortDirection? sortDirection,
        PaginationValidator paginationValidationRules, SortingValidator<Role> sortingValidationRules,
        string? name
        )
            : base(page, pageSize, sortBy, sortDirection, paginationValidationRules, sortingValidationRules)
        {
            if (!string.IsNullOrEmpty(name))
                Query.Where(role => EF.Functions.Like(role.Name.ToLower(), $"%{name.ToLower()}%"));

        }
    }
}

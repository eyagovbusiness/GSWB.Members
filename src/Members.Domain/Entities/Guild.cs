using System.Diagnostics.CodeAnalysis;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Guild aggegate
    /// </summary>
    /// <remarks>
    /// - The Id of the entitiy matches the Id of the guild server from Discord.<br />
    /// - It is a considreable size aggregate and it will be one fo the most accessed ones, if in the future if the aggegate size becomes an issue CQRS or event sourcing can mitigate performance issues with this aggregate.<br />
    /// </remarks>
    public partial class Guild : Entity<ulong>
    {
        public required string Name { get; set; }
        public required string IconUrl { get; set; }

        [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Guild(string id, string Name, string iconUrl)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.SetId(ulong.Parse(id));
            this.Name = Name;
            this.IconUrl = iconUrl;
        }

        internal Guild() { }

    }
}

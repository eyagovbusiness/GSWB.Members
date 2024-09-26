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
    }
}

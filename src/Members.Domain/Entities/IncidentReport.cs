using System.ComponentModel.DataAnnotations;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents an incident report where a member is reporting an incident with another member.
    /// </summary>
    public class IncidentReport : Entity<Guid>
    {
        /// <summary>
        /// Title of this incident being reported.
        /// </summary>
        [MaxLength(64)]
        public required string Title { get; set; }

        /// <summary>
        /// Description of the incident.
        /// </summary>
        [MaxLength(1024)]
        public required string Description { get; set; }

        /// <summary>
        /// Member who reported the incident.
        /// </summary>
        public required Guid AccuserId { get; set; }

        /// <summary>
        /// Member who is being reported.
        /// </summary>
        public required Guid AccusedId { get; set; }

        /// <summary>
        /// Final sentence of this sentence after being evaluated.
        /// </summary>
        public Sentence? Sentence { get; set; }

    }
}

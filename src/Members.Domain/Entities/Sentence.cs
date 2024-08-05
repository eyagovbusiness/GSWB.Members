using Members.Domain.ValueObjects;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents a sentence of an incident report.
    /// </summary>
    public class Sentence : Entity<Guid>
    {
        /// <summary>
        /// Description and arguments of the sentence. 
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Member who managed the incident and expedited this sentence.
        /// </summary>
        public required Guid JudgeId { get; set; }

        /// <summary>
        /// Type of the sentence with the final repercussion.
        /// </summary>
        public required SentenceTypeEnum SentenceType { get; set; }

    }
}

using Common.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents a member account (user account) in the system. Contains information about the DiscordUser who created this account, the in-game handles and the list of application Role assigned to this member which determines its permissions.
    /// </summary>
    public partial class Member: Entity<Guid>
    {
        /// <summary>
        /// Discord user ID from OAuth with discord.
        /// </summary>
        [Required]
        public ulong DiscordUserId { get; set; }

        /// <summary>
        /// The display name of this member in the guild's discord server. The value is taken from the global display name from the discord account or the in-server nickname override if it is set.
        /// </summary>
        /// <remarks> Members are not allowed to set it manually, but they can request a fetch update with 24h cooldown.The bot will get the user by id and update the nickname changes.</remarks>
        [MaxLength(32)]
        [MinLength(2)]
        [Required]
        public string DiscordGuildDisplayName { get; set; }

        /// <summary>
        /// Discord Avatar image URL.
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string DiscordAvatarUrl { get; set; }

        ///// <summary>
        ///// The guild of this member.
        ///// </summary>
        //public Guild Guild { get; set; }

        /// <summary>
        /// The Community Moniker displays under your Handle in Spectrum forum posts. You can change your moniker anytime, as many times as you like. This is not your character name in game and is used solely for interactions on our forums and chat channels on the Spectrum website.
        /// </summary>
        public string? SpectrumCommunityMoniker { get; set; }

        /// <summary>
        /// The handle is your nickname seen by friends in game and in Spectrum. You can change your Handle one time for free through your Account. After that initial change, you will need to purchase a Handle Change Pass. 
        /// The Handle must be unique across all RSI accounts.Your handle cannot contain a dash, underscore, or space at the beginning or end, and certain accented letters and other punctuation are not supported at this time.
        /// </summary>
        public string? GameHandle { get; set; }

        /// <summary>
        /// Member status<see cref="MemberStatusEnum"/>
        /// </summary>
        public MemberStatusEnum Status { get; set; }

        /// <summary>
        /// Users can verify the ownership of the game account they claimed by automatic verification or human verification with a guild administrator.
        /// </summary>
        /// <remarks>User DiscordUserId as verification code seed.</remarks>
        public bool IsGameHandleVerified { get; set; }

        //ctor for EF
        public Member(ulong DiscordUserId, string DiscordGuildDisplayName, string DiscordAvatarUrl)
        {
            this.DiscordUserId = DiscordUserId;
            this.DiscordGuildDisplayName = DiscordGuildDisplayName;
            this.DiscordAvatarUrl = DiscordAvatarUrl;
        }

        //public Member(ulong DiscordUserId, string DiscordGuildDisplayName, string DiscordAvatarUrl, ICollection<Role> Roles)
        //    : this(DiscordUserId, DiscordGuildDisplayName, DiscordAvatarUrl)
        //{
        //    this.Roles = Roles;
        //}
        //public Member(string DiscordUserId, string DiscordGuildDisplayName, string DiscordAvatarUrl, ICollection<Role> Roles)
        //    : this(ulong.Parse(DiscordUserId), DiscordGuildDisplayName, DiscordAvatarUrl, Roles)
        //{

        //}
        //public Member(string DiscordUserId, string DiscordGuildDisplayName, string DiscordAvatarUrl, string? GameHandle, string? SpectrumCommunityMoniker, ICollection<Role> Roles)
        //    : this(DiscordUserId, DiscordGuildDisplayName, DiscordAvatarUrl, Roles)
        //{
        //    if (GameHandle != null)
        //        this.GameHandle = GameHandle;
        //    if (SpectrumCommunityMoniker != null)
        //        this.SpectrumCommunityMoniker = SpectrumCommunityMoniker;
        //}
    }
}

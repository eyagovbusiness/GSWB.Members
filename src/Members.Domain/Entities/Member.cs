using Common.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TGF.CA.Domain.Primitives;

namespace Members.Domain.Entities
{
    /// <summary>
    /// Represents a member account (user account) in the system. Contains information about the DiscordUser who created this account, the in-game handles and the list of application Role assigned to this member which determines its permissions.
    /// </summary>
    public partial class Member: EntityBase
    {
        public ulong GuildId { get; private init; }
        public ulong UserId { get; private init; }
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
        /// <remarks>User UserId as verification code seed.</remarks>
        public bool IsGameHandleVerified { get; set; }

        ///// <summary>
        ///// The verification code generated for this member enitiy.
        ///// </summary>
        //public virtual VerifyCode? VerifyCode { get; set; }

        /// <summary>
        /// 6-digit verification code used to verify the current <see cref="GameHandle"/> set in this Member.
        /// </summary>
        [MaxLength(6)]
        public string GameHandleVerificationCode { get; set; } = "000000";

        /// <summary>
        /// Expiry date of the current <see cref="GameHandleVerificationCode"/>
        /// </summary>
        public DateTimeOffset VerificationCodeExpiryDate { get; set; }

        #region Constructors
        protected Member() { DiscordAvatarUrl = default!; DiscordGuildDisplayName = default!; }
        //ctor for EF
        public Member(ulong userId, ulong guildId, string discordGuildDisplayName, string discordAvatarUrl)
        {
            GuildId = guildId;
            UserId = userId;
            this.DiscordGuildDisplayName = discordGuildDisplayName;
            this.DiscordAvatarUrl = discordAvatarUrl;
        }

        public Member(string userId, string guildId, string discordGuildDisplayName, string discordAvatarUrl)
            : this(ulong.Parse(userId), ulong.Parse(guildId), discordGuildDisplayName, discordAvatarUrl)
        {

        }
        [SetsRequiredMembers]
        public Member(string userId, string guildId, string discordGuildDisplayName, string discordAvatarUrl, string? gameHandle, string? spectrumCommunityMoniker)
            : this(userId, guildId, discordGuildDisplayName, discordAvatarUrl)
        {
            if (GameHandle != null)
                this.GameHandle = GameHandle;
            if (SpectrumCommunityMoniker != null)
                this.SpectrumCommunityMoniker = SpectrumCommunityMoniker;
        }
        #endregion

        #region Business Logic
        /// <summary>
        /// Updates basic member profile fields, GameHandle and SpectrumCommunityMoniker.
        /// </summary>
        /// <param name="aGameHandle">New value to update the GameHandle with.</param>
        /// <param name="aSpectrumCommunityMoniker">New value to update the SpectrumCommunityMoniker with.</param>
        /// <returns>True if any field was updated, otherwise false.</returns>
        public bool UpdateProfile(string? aGameHandle, string? aSpectrumCommunityMoniker)
        {
            bool lHasBeenUpdated = false;

            if (aGameHandle != null && GameHandle != aGameHandle)
            {
                GameHandle = aGameHandle;
                IsGameHandleVerified = false;
                lHasBeenUpdated = true;
            }

            if (aSpectrumCommunityMoniker != null && SpectrumCommunityMoniker != aSpectrumCommunityMoniker)
            {
                SpectrumCommunityMoniker = aSpectrumCommunityMoniker;
                lHasBeenUpdated = true;
            }

            return lHasBeenUpdated;
        }

        /// <summary>
        /// Verifies the GameHandle if the VerificationCodeExpiryDate has not expired yet, setting IsGameHandleVerified to true.
        /// </summary>
        /// <returns>True in case IsGameHandleVerified was false and got updated to tue, otherwise false.</returns>
        public bool Verify()
        {
            if (!IsGameHandleVerified
                && VerificationCodeExpiryDate > DateTimeOffset.Now)
            {
                IsGameHandleVerified = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update game handle verification code ONLY if the game handle was not verified already and the code has expired already.
        /// </summary>
        /// <returns>True if GameHandleVerificationCode was updated, otherwise false.</returns>
        public bool TryRefreshGameHandleVerificationCode()
        {
            if (IsGameHandleVerified || VerificationCodeExpiryDate > DateTimeOffset.Now)
                return false;
            GameHandleVerificationCode = new Random().Next(100000, 999999).ToString();
            VerificationCodeExpiryDate = DateTimeOffset.Now.AddMinutes(1);
            return true;
        }
        #endregion

    }
}

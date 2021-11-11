using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IAuthAPI
	{
		Task<(bool Ok, ITokenCredential Token)> Verify(CancellationToken ct = default);
		Task<bool> IfExist(
			string email = null, 
			string displayName = null,
			UserID userId = null,
			UserID excludeUserId = null,
			CancellationToken ct = default);
		Task<bool> VerifyTwoFactorAuth(string code, CancellationToken ct = default);
		Task<bool> VerifyTwoFactorAuthWithRecoveryCode(string code, CancellationToken ct = default);
		Task<CurrentUser> GetCurrentUser(CancellationToken ct = default);
		Task<ResponseMessage> Logout(CancellationToken ct = default);
		Task<CurrentUser> DeleteAccount(CancellationToken ct = default);
	}
}
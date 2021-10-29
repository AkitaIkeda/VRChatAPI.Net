using System.Collections.Generic;

namespace VRChatAPI.Interfaces
{
	interface ISessionFactory
	{
		IEnumerable<ISession> Sessions { get; }
		ISession CreateSession(ITokenCredential cred);
	}
}

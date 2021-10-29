using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VRChatAPI.Extentions.DependancyInjection;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Implementations
{
	internal class SessionFactory : ISessionFactory, IDisposable
	{
		private readonly VRCAPIOptions options;
		private readonly IServiceScopeFactory scopeFactory;
		private List<Session> sessions;

		public SessionFactory(
			VRCAPIOptions options,
			IServiceScopeFactory scopeFactory)
		{
			this.options = options;
			this.scopeFactory = scopeFactory;
			sessions = new List<Session>();
		}

		public IEnumerable<ISession> Sessions => sessions.AsReadOnly();

		public ISession CreateSession(ITokenCredential cred)
		{
			var sess = new Session(scopeFactory.CreateScope(), cred);
			sessions.Add(sess);
			return sess;
		}

		public void Dispose()
		{
			foreach(var s in sessions)
				s.Dispose();
		}
	}
}

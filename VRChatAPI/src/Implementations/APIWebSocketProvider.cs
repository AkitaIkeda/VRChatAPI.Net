using System;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Implementations{
	public class APIWebSocketProvider : IAPIWebSocketProvider, IDisposable
	{
		private APIWebSocket ws;

		public async Task<IAPIWebSocket> Create(Uri uri, CancellationToken ct)
		{
			ws = new APIWebSocket();
			await ws.Connect(uri, ct);
			return ws;
		}

		public void Dispose() => 
			DisposeWS();

		public void DisposeWS() => 
			ws.Dispose();
	}
}
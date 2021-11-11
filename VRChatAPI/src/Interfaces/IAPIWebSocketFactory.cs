using System;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces{
	public interface IAPIWebSocketProvider {
		Task<IAPIWebSocket> Create(Uri uri, CancellationToken ct);
		void DisposeWS();
	}
}
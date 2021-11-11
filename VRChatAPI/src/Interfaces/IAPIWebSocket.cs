using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces
{
	public interface IAPIWebSocket
	{
		WebSocketState State { get; }
		Task<WebSocketReceiveResult> Receive(ArraySegment<byte> buffer, CancellationToken cancellationToken);
	}
}
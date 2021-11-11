using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Implementations
{
	public class APIWebSocket : IAPIWebSocket, IDisposable
	{
		private readonly ClientWebSocket ws;

		public APIWebSocket() => 
			ws = new ClientWebSocket();

		internal Task Connect(Uri uri, CancellationToken ct) =>
			ws.ConnectAsync(uri, ct);

		public WebSocketState State => ws.State;

		public void Dispose() => 
			ws.Dispose();

		public Task<WebSocketReceiveResult> Receive(ArraySegment<byte> buffer, CancellationToken cancellationToken) =>
			ws.ReceiveAsync(buffer, cancellationToken);
	}
}
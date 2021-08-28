using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Utils
{
	public class AsyncSequentialReader<T> : IAsyncEnumerable<T>
	{
		public delegate Task<IEnumerable<T>> ReadFunc(uint offset, uint n);

		private ReadFunc reader;
		public uint bufferSize { get; set; }

		/// <summary>
		/// Constructs a new SequentialReader
		/// </summary>
		/// <param name="f">Read function</param>
		/// <param name="buffer">Buffer size</param>
		public AsyncSequentialReader(ReadFunc f, [Range(1, 100)] uint buffer = 100)
		{
			reader = f;
			bufferSize = buffer;
		}

		public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			uint i = 0;
			while (true){
				var ret = await reader(i, bufferSize);
				var c = 0;
				foreach (var item in ret)
				{
					c++;
					yield return item;
				}
				
				if (c < bufferSize) 
					yield break;

				i += bufferSize;
			}
		}
	}
}
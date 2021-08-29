using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Utils
{
	public class SequentialReader<T> : IEnumerable<T>
	{
		public delegate IEnumerable<T> ReadFunc(uint offset, uint n);

		protected ReadFunc reader;
		public uint bufferSize { get; set; }

		/// <summary>
		/// Constructs a new SequentialReader
		/// </summary>
		/// <param name="f">Read function</param>
		/// <param name="buffer">Buffer size</param>
		public SequentialReader(ReadFunc f, [Range(1, 100)] uint buffer = 100)
		{
			reader = f;
			bufferSize = buffer;
		}

		public IEnumerator<T> GetEnumerator()
		{
			uint i = 0;
			while (true){
				var ret = reader(i, bufferSize);
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

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
	}

	public class AsyncSequentialReader<T> : SequentialReader<T>
	{
		public delegate Task<IEnumerable<T>> AsyncReadFunc(uint offset, uint n);

		/// <summary>
		/// Constructs a new AsyncSequentialReader
		/// </summary>
		/// <param name="f">Read function</param>
		/// <param name="buffer">Buffer size</param>
		public AsyncSequentialReader(ReadFunc f, [Range(1, 100)] uint buffer = 100) : base(f, buffer) {}
		public AsyncSequentialReader(AsyncReadFunc f, [Range(1, 100)] uint buffer = 100) : base((uint offset, uint n) => f(offset, n).Result, buffer){}
	}
}
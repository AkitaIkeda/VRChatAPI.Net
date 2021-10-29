namespace VRChatAPI.Logging
{
	public static class LogEventID
	{
		#region System Phases
		public const int SystemInitialize = 1000;
		public const int SystemStart = 1001;
		public const int SystemStop = 1002;
		public const int SystemCancel = 1003;
		public const int SystemFinish = 1004;
		public const int SystemDispose = 1005;
		#endregion

		#region Connections
		public const int Connect = 2000;
		public const int Send = 2001;
		public const int Receive = 2002;
		public const int Close = 2003;
		#endregion

		#region Serialize/Deserialize
		public const int Serialize = 3000;
		public const int Deserialize = 3001;
		public const int DeserializationError = 3002;
		#endregion

		#region http
		public const int Get = 4000;
		public const int Put = 4001;
		public const int Post = 4002;
		public const int Delete = 4003;
		#endregion

		#region misc
		public const int EventIgnition = 6000;
		public const int NotSupported = 6001;
		#endregion
	}
}

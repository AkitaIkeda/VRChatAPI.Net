using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VRChatAPI.Objects
{
	internal class AcceptNoneDatatimeConverter: IsoDateTimeConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
				if ((string) reader.Value == "none") 
					return null;
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}
	}
}
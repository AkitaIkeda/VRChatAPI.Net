﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Serialization
{
	public class ResponseMessageConverter : JsonConverter<ResponseMessage>
	{
		public override ResponseMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject)
				throw new JsonException();
			reader.Read();

			var s = reader.GetString();
			Console.WriteLine(s);
			var r = new ResponseMessage();
			var t = JsonSerializer.Deserialize<JsonElement>(ref reader, options);
			if(!Enum.TryParse<EResponseType>(s, out var ty))
				throw new JsonException();
			r.MessageType = ty;
			r.Message = t.GetProperty("message").GetString();
			r.StatusCode = t.GetProperty("status_code").GetInt32();
			reader.Read();
			return r;
		}

		public override void Write(Utf8JsonWriter writer, ResponseMessage value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
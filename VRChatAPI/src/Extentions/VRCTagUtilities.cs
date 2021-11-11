using System;
using System.Collections.Generic;
using System.Linq;
using VRChatAPI.Enums;

namespace VRChatAPI.Extentions
{
	public static class TagsUtilities
	{
		[Flags]
		private enum TrustLevel : uint
		{
			visitor = 0,
			system_trust_basic = 0b00000001,
			system_trust_known = 0b00000010,
			system_trust_trusted = 0b00000100,
			system_trust_veteran = 0b00001000,
			system_trust_legend = 0b00010000, // Unused
			system_legend = 0b00100000, // Unused
		}
		private static ETrustLevel GetTrustLevel(TrustLevel tl)
		{
			if(tl == TrustLevel.system_legend)
					return ETrustLevel.Legend;
			if (tl == TrustLevel.system_trust_legend)
				return ETrustLevel.Veteran;
			if (tl == TrustLevel.system_trust_veteran)
				return ETrustLevel.Trusted;
			if (tl == TrustLevel.system_trust_trusted)
				return ETrustLevel.Known;
			if (tl == TrustLevel.system_trust_known)
				return ETrustLevel.User;
			if (tl == TrustLevel.system_trust_basic)
				return ETrustLevel.NewUser;
			return ETrustLevel.Visitor;
		}

		private static IEnumerable<ELangage> GetLanguages(IEnumerable<string> tags) => tags
			.Select(v => v.Substring("langage_".Length))
			.Select(v => (ELangage)Enum.Parse(typeof(ELangage), v));

		public static (
			ETrustLevel TrustLevel, 
			IEnumerable<ELangage> Languages,
			EUserTag UserTags,
			IEnumerable<string> Others)
			GetUserInfoFromTags(this IEnumerable<string> tags)
		{
			TrustLevel tl = 0;
			EUserTag usertag = 0;
			var tmp = new List<string>();
			foreach (var t in tags)
				if (Enum.TryParse<TrustLevel>(t, out var o))
					tl |= o;
				else if (Enum.TryParse<EUserTag>(t, out var p))
					usertag |= p;
				else tmp.Add(t);
			var lang = tmp.Where(t => t.IndexOf("langage_") == 0);
			return (
				GetTrustLevel(tl),
				GetLanguages(lang),
				usertag,
				tmp.Where(t => t.IndexOf("langage_") != 0));
		}

		public static (
			IEnumerable<string> AuthorTags,
			EContentTag ContentTags,
			IEnumerable<string> Others) 
			GetWorldInfoFromTags(this IEnumerable<string> tags)
		{
			var authorTags = tags
				.Where(t => t.IndexOf("author_tag_") == 0)
				.Select(t => t.Substring("author_tag_".Length));
			var others = tags.Where(t => t.IndexOf("author_tag_") != 0);
			var contentTags = others
				.Where(t => t.IndexOf("content_") == 0)
				.Select(t => t.Substring("content_".Length))
				.Select(v => (EContentTag)Enum.Parse(typeof(EContentTag), v))
				.Aggregate((a, b) => a | b);
			return (authorTags, contentTags, others.Where(t => t.IndexOf("content_") != 0));
		}
	}
}

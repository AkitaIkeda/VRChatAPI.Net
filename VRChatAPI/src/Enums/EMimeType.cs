using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EMimeType
	{
		[EnumMember(Value = "image/jpeg")]
		image_jpeg,
		[EnumMember(Value = "image/jpg")]
		image_jpg,
		[EnumMember(Value = "image/png")]
		image_png,
		[EnumMember(Value = "image/webp")]
		image_webp,
		[EnumMember(Value = "image/gif")]
		image_gif,
		[EnumMember(Value = "image/bmp")]
		image_bmp,
		[EnumMember(Value = "image/svg+xml")]
		image_svg_xml,
		[EnumMember(Value = "image/tiff")]
		image_tiff,
		[EnumMember(Value = "application/x-avatar")]
		application_x_avatar,
		[EnumMember(Value = "application/x-world")]
		application_x_world,
		[EnumMember(Value = "application/gzip")]
		application_gzip,
		[EnumMember(Value = "application/x-rsync-signature")]
		application_x_rsync_signature,
		[EnumMember(Value = "application/x-rsync-delta")]
		application_x_rsync_delta,
		[EnumMember(Value = "application/octet-stream")]
		application_octet_stream,
	}
}

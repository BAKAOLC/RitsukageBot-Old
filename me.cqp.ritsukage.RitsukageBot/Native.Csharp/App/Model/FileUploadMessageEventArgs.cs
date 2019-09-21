using System;
using Native.Csharp.Sdk.Cqp.Model;

namespace Native.Csharp.App.Model
{
	public class FileUploadMessageEventArgs : EventArgsBase
	{
		/// <summary>
		/// 发送时间
		/// </summary>
		public DateTime SendTime { get; set; }
		/// <summary>
		/// 来源群号
		/// </summary>
		public long FromGroup { get; set; }
		/// <summary>
		/// 上传文件信息
		/// </summary>
		public GroupFile File { get; set; }
	}
}

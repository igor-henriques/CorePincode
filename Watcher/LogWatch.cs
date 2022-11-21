using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using CorePinCash.Data;
using CorePincash.Model;
using CorePinCash.Model;
using CorePinCash.Repository;
using CorePinCash.Server;
using Microsoft.Extensions.Logging;
using CorePincode.Repository;

namespace CorePinCash.Watcher;

public class LogWatch
{
	private readonly ILogger<LogWatch> logger;

	private long lastSize;

	private readonly string path;

	private readonly Timer logWatch = new Timer(1000.0);

	private readonly IPinCashRepository _pincashContext;

	private readonly IPinItemRepository _pinitemContext;

	private readonly PlayerInteractionController playerController;

	private readonly ServerConnection _server;

	public LogWatch(ILogger<LogWatch> logger, IPinCashRepository _pincashContext, IPinItemRepository _pinitemContext, ServerConnection server, PlayerInteractionController playerController)
	{
		this.logger = logger;
		_server = server;
		this._pincashContext = _pincashContext;
		this._pinitemContext = _pinitemContext;
		path = Path.Combine(_server.logsPath, "world2.chat");
		logWatch.Elapsed += LogWatch_Elapsed;
		lastSize = GetFileSize(path);
		this.playerController = playerController;
		logWatch.Start();
	}

	private async void LogWatch_Elapsed(object sender, ElapsedEventArgs e)
	{
		try
		{
			long fileSize = GetFileSize(path);
			if (fileSize > lastSize)
			{
				await ReadTail(path, UpdateLastFileSize(fileSize));
			}
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
	}

	private async Task ReadTail(string filename, long offset)
	{
		try
		{
			List<PlayerInteraction> playersInteractions = new List<PlayerInteraction>();
			byte[] bytes;
			using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				fs.Seek(offset * -1, SeekOrigin.End);
				bytes = new byte[offset];
				fs.Read(bytes, 0, (int)offset);
			}
			List<string> logs = (from x in Encoding.Default.GetString(bytes).Split((new string[1] { "\n" })[0])
				where !string.IsNullOrEmpty(x.Trim())
				select x).ToList();
			logs.ForEach(delegate(string x)
			{
				playersInteractions.Add(DecodeMessage(x).Result);
			});
			if (playersInteractions.Where((PlayerInteraction x) => x != null).Count() > 0)
			{
				await playerController.AnalysePlayerInteractions(playersInteractions);
			}
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
	}

	private async Task<EPincode> DeterminePincodeType(string pincode)
	{
		if (await _pincashContext.GetPinCashByPincode(pincode) != null)
		{
			return EPincode.CASH;
		}
		if (await _pinitemContext.GetPinItemByPincode(pincode) != null)
		{
			return EPincode.ITEM;
		}
		return EPincode.UNKNOWN;
	}

	private async Task<PlayerInteraction> DecodeMessage(string encodedMessage)
	{
		try
		{
			if (!encodedMessage.Contains("src=-1"))
			{
				string message = Encoding.Unicode.GetString(Convert.FromBase64String(Regex.Match(encodedMessage, "msg=([\\s\\S]*)").Value.Replace("msg=", "")));
				if (message.Contains("!PIN"))
				{
					int roleID = int.Parse(Regex.Match(encodedMessage, "src=([0-9]*)").Value.Replace("src=", "").Trim());
					string pincode = message.Replace("!PIN", string.Empty);
					PlayerInteraction playerInteraction = new PlayerInteraction
					{
						Pincode = pincode.Trim(),
						RoleID = roleID
					};
					PlayerInteraction playerInteraction2 = playerInteraction;
					playerInteraction2.PincodeType = await DeterminePincodeType(pincode.Trim());
					return playerInteraction;
				}
			}
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
		return null;
	}

	private long UpdateLastFileSize(long fileSize)
	{
		long result = fileSize - lastSize;
		lastSize = fileSize;
		return result;
	}

	private long GetFileSize(string fileName)
	{
		return new FileInfo(fileName).Length;
	}
}

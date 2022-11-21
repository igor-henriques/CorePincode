using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CorePinCash.Data;
using CorePincash.Model;
using CorePinCash.Model;
using CorePinCash.Repository;
using CorePinCash.Server;
using PWToolKit;
using PWToolKit.API.GDeliveryd;
using CorePincode.Repository;

namespace CorePinCash.Watcher;

public class PlayerInteractionController
{
	private readonly IPinCashRepository _pincashContext;

	private readonly IPinItemRepository _pinitemContext;

	private List<string> pincodesAvailables;

	private readonly PrizeManager prizeManager;

	private readonly ServerConnection _server;

	public PlayerInteractionController(IPinCashRepository _pincashContext, IPinItemRepository pinitemContext, ServerConnection server)
	{
		_server = server;
		this._pincashContext = _pincashContext;
		_pinitemContext = pinitemContext;
		prizeManager = new PrizeManager(_pincashContext, _pinitemContext, _server);
		pincodesAvailables = _pincashContext.GetAllPincodes().Result;
		pincodesAvailables.AddRange(_pinitemContext.GetAllPincodes().Result);
		PWGlobal.UsedPwVersion = _server.PwVersion;
	}

	public async Task AnalysePlayerInteractions(List<PlayerInteraction> playerInteractions)
	{
		try
		{
			List<PlayerInteraction> possibleWinners = GetWinners(pincodesAvailables, playerInteractions);
			await SendWinnersPrize(await FilterRealWinners(possibleWinners));
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
	}

	private async Task SendWinnersPrize(List<PlayerInteraction> winners)
	{
		foreach (PlayerInteraction winner in winners)
		{
			EPincode pinType2 = winner.PincodeType;
			if (1 == 0)
			{
			}
			object obj;
			if (pinType2 == EPincode.CASH)
			{
				obj = await _pincashContext.GetPinCashByPincode(winner.Pincode);
			}
			else
			{
				EPincode pinType = pinType2;
				if (pinType == EPincode.ITEM)
				{
					obj = await _pinitemContext.GetPinItemByPincode(winner.Pincode);
				}
				else
				{
					if (pinType2 != 0)
					{
						if (1 == 0)
						{
						}
						throw new SwitchExpressionException(pinType2);
					}
					obj = null;
				}
			}
			if (1 == 0)
			{
			}
			dynamic pincode = obj;
			if (pincode != null)
			{
				pincode.Obtained = true;
				pincode.ObtainedBy = winner.RoleID;
				pincode.DateObtained = DateTime.Now;
				bool flag = ((!(pincode.GetType().Equals(typeof(PinCash)) ? true : false)) ? (await prizeManager.SendPinitem((PinItem)pincode)) : (await prizeManager.SendPincash((PinCash)pincode)));
				_ = flag;
			}
		}
	}

	private async Task<List<PlayerInteraction>> FilterRealWinners(List<PlayerInteraction> possibleWinners)
	{
		List<PlayerInteraction> winners = new List<PlayerInteraction>();
		foreach (PlayerInteraction possibleWinner in possibleWinners)
		{
			if (await CheckWin(possibleWinner))
			{
				winners.Add(possibleWinner);
			}
		}
		return winners;
	}

	private async Task<bool> CheckWin(PlayerInteraction possibleWinner)
	{
		bool response = false;
		List<string> list = ((!possibleWinner.PincodeType.Equals(EPincode.CASH)) ? (await _pinitemContext.GetAllPincodes()) : (await _pincashContext.GetAllPincodes()));
		List<string> allPincodes = list;
		if (!allPincodes.Contains(possibleWinner.Pincode))
		{
			PrivateChat.Send(_server.GDeliveryd, possibleWinner.RoleID, "O código " + possibleWinner.Pincode + " não existe.");
			LogWriter.Write($"Personagem ID {possibleWinner.RoleID} tentou sacar o código {possibleWinner.Pincode}, que não existe.");
		}
		EPincode pincodeType4 = possibleWinner.PincodeType;
		if (1 == 0)
		{
		}
		EPincode pincodeType3 = pincodeType4;
		Task<bool> task;
		if (pincodeType3.Equals(EPincode.CASH))
		{
			task = Task.Run(async delegate
			{
				if (!(await _pincashContext.GetNotObtained()).Select((PinCash x) => x.Code).Contains(possibleWinner.Pincode))
				{
					PrivateChat.Send(_server.GDeliveryd, possibleWinner.RoleID, "O código " + possibleWinner.Pincode + " já foi sacado.");
					Console.WriteLine("O código " + possibleWinner.Pincode + " já foi sacado.");
				}
				else
				{
					response = true;
				}
				return response;
			});
		}
		else
		{
			EPincode pincodeType2 = pincodeType4;
			if (pincodeType2.Equals(EPincode.ITEM))
			{
				task = Task.Run(async delegate
				{
					if (!(await _pinitemContext.GetNotObtained()).Select((PinItem x) => x.Code).Contains(possibleWinner.Pincode))
					{
						PrivateChat.Send(_server.GDeliveryd, possibleWinner.RoleID, "O código " + possibleWinner.Pincode + " já foi sacado.");
						Console.WriteLine("O código " + possibleWinner.Pincode + " já foi sacado.");
					}
					else
					{
						response = true;
					}
					return response;
				});
			}
			else
			{
				EPincode pincodeType = pincodeType4;
				task = ((!pincodeType.Equals(EPincode.UNKNOWN)) ? Task.FromResult(result: false) : Task.FromResult(result: false));
			}
		}
		if (1 == 0)
		{
		}
		Task<bool> result = task;
		return result.Result;
	}

	public async Task UpdateAvailablePincodes()
	{
		pincodesAvailables = await _pincashContext.GetAvailablePincodes();
	}

	private List<PlayerInteraction> GetWinners(List<string> pincodesAvailables, List<PlayerInteraction> possibleWinners)
	{
		List<PlayerInteraction> list = new List<PlayerInteraction>();
		foreach (string pincodesAvailable in pincodesAvailables)
		{
			foreach (PlayerInteraction possibleWinner in possibleWinners)
			{
				if (possibleWinner.Pincode.Equals(pincodesAvailable))
				{
					list.Add(possibleWinner);
				}
			}
		}
		return list;
	}
}

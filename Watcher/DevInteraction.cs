using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorePinCash.Data;
using CorePincash.Model;
using CorePinCash.Model;
using CorePinCash.Repository;
using CorePinCash.Server;
using CorePincode.Repository;

namespace CorePinCash.Watcher;

public class DevInteraction
{
	private readonly IPinCashRepository _pincashContext;

	private readonly IPinItemRepository _pinitemContext;

	private readonly ServerConnection _server;

	public DevInteraction(IPinCashRepository pincashContext, IPinItemRepository pinitemContext, ServerConnection server)
	{
		_pincashContext = pincashContext;
		_pinitemContext = pinitemContext;
		_server = server;
	}

	public async Task Start()
	{
		try
		{
			Console.WriteLine("ENTRANDO EM MODO INTERATIVO.");
			await Task.Delay(1000);
			Console.WriteLine("ENTRANDO EM MODO INTERATIVO..");
			await Task.Delay(1000);
			Console.WriteLine("ENTRANDO EM MODO INTERATIVO...");
			await Task.Delay(1000);
			Console.WriteLine("\n");
			await ShowRemainingPin();
			if (AskForCreating())
			{
				string pincodeType = AskPincodeType();
				bool flag = ((!pincodeType.Equals("CASH")) ? (await CreatePinitem()) : (await CreatePincash()));
				_ = flag;
			}
			Console.WriteLine("\nSAINDO DE MOTO INTERATIVO.");
			await Task.Delay(1000);
			Console.WriteLine("SAINDO DE MOTO INTERATIVO..");
			await Task.Delay(1000);
			Console.WriteLine("SAINDO DE MOTO INTERATIVO...");
			await Task.Delay(1000);
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
	}

	private bool AskForCreating()
	{
		Console.WriteLine("\nDeseja criar novos pincodes? Y/N");
		if (GetYesNoAnswer().Equals("Y"))
		{
			return true;
		}
		return false;
	}

	private string AskPincodeType()
	{
		Console.WriteLine("Qual tipo de pincode deseja criar? CASH/ITEM");
		return GetCashItemAnswer();
	}

	private async Task<bool> CreatePincash()
	{
		Console.WriteLine("Quantos pincashes deseja criar?");
		int amount = GetIntAnswer();
		for (int i = 0; i < amount; i++)
		{
			await GeneratePinCash();
		}
		List<PinCash> pincodes = await _pincashContext.GetNotObtained();
		StringBuilder sb = new StringBuilder();
		pincodes.ForEach(delegate(PinCash x)
		{
			sb.AppendLine($"Código: {x.Code}\tCash: {x.CashAmount}");
		});
		await File.WriteAllTextAsync("./Configurations/Pincash.txt", sb.ToString());
		Console.WriteLine("\nArquivo salvo em './Configurations/Pincash.txt'");
		return true;
	}

	private async Task<bool> CreatePinitem()
	{
		Console.WriteLine("Quantos pinitems deseja criar?");
		int amount = GetIntAnswer();
		for (int i = 0; i < amount; i++)
		{
			if (!(await GeneratePinItem()))
			{
				return false;
			}
		}
		List<PinItem> pincodes = await _pinitemContext.GetNotObtained();
		StringBuilder sb = new StringBuilder();
		pincodes.ForEach(delegate(PinItem x)
		{
			sb.AppendLine($"Código: {x.Code}\tItem: {x.ItemAmount}x {x.ItemName}({x.ItemId})");
		});
		await File.WriteAllTextAsync("./Configurations/Pinitem.txt", sb.ToString());
		Console.WriteLine("\nArquivo salvo em './Configurations/Pinitem.txt'");
		return true;
	}

	private async Task ShowRemainingPin()
	{
		PinCodes currentPincodes = new PinCodes(await _pincashContext.GetNotObtained(), await _pinitemContext.GetNotObtained());
		if (currentPincodes.TotalRecords <= 0)
		{
			return;
		}
		Console.WriteLine($"Há {currentPincodes.TotalRecords} pincodes que não foram sacados. Deseja obtê-los? Y/N");
		string answer = GetYesNoAnswer();
		if (!answer.Equals("Y"))
		{
			return;
		}
		Console.WriteLine("\n");
		StringBuilder sbCash = new StringBuilder();
		StringBuilder sbItem = new StringBuilder();
		foreach (PinCash item2 in currentPincodes.Pincashes)
		{
			string line2 = $"Código: {item2.Code}\tCash: {item2.CashAmount}\n";
			Console.WriteLine(line2);
			sbCash.AppendLine(line2);
		}
		foreach (PinItem item in currentPincodes.PinItems)
		{
			string line = $"Código: {item.Code}\tItem: {item.ItemAmount}x {item.ItemName}({item.ItemId})\n";
			Console.WriteLine(line);
			sbItem.AppendLine(line);
		}
		await File.WriteAllTextAsync("./Configurations/Pincash.txt", sbCash.ToString());
		await File.WriteAllTextAsync("./Configurations/Pinitem.txt", sbItem.ToString());
		Console.WriteLine("\nArquivos salvos em ./Configurations");
	}

	private string GetYesNoAnswer()
	{
		string text = null;
		while (text != "Y" && text != "N")
		{
			text = Console.ReadLine()!.Trim().ToUpper();
		}
		return text;
	}

	private string GetCashItemAnswer()
	{
		string text = null;
		while (text != "CASH" && text != "ITEM")
		{
			text = Console.ReadLine()!.Trim().ToUpper();
		}
		return text;
	}

	private int GetIntAnswer()
	{
		int result = 0;
		while (result == 0)
		{
			if (!int.TryParse(Console.ReadLine()!.Trim(), out result))
			{
				Console.WriteLine("O número precisa ser um inteiro natural MAIOR QUE 0.");
			}
		}
		return result;
	}

	private async Task GeneratePinCash()
	{
		try
		{
			List<string> existingCodes = await _pincashContext.GetAllPincodes();
			string code = existingCodes.FirstOrDefault();
			while (existingCodes.Contains(code) || string.IsNullOrEmpty(code))
			{
				code = new string((from s in Enumerable.Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 8)
					select s[new Random().Next(s.Length)]).ToArray());
			}
			Console.WriteLine("Pincode " + code + " gerado. Digite a quantidade de cash que será premiado por esse código:");
			int cashAmount = GetIntAnswer();
			PinCash newPin = new PinCash
			{
				Code = code,
				CashAmount = cashAmount,
				DateGenerated = DateTime.Now
			};
			await _pincashContext.Add(newPin);
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
	}

	private async Task<bool> GeneratePinItem()
	{
		try
		{
			List<string> existingCodes = await _pinitemContext.GetAllPincodes();
			string code = existingCodes.FirstOrDefault();
			while (existingCodes.Contains(code) || string.IsNullOrEmpty(code))
			{
				code = new string((from s in Enumerable.Repeat("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789", 8)
					select s[new Random().Next(s.Length)]).ToArray());
			}
			Console.WriteLine("Pincode " + code + " gerado. Digite o ID do item que será premiado por esse código. PS: O ITEM PRECISA ESTAR CADASTRADO NO JSON!");
			int itemId = GetIntAnswer();
			Console.WriteLine("Digite a quantidade de itens recompensado por esse código:");
			int itemCount2 = GetIntAnswer();
			Item itemChoosed = _server.ItemsReward.Where((Item x) => x.Id.Equals(itemId)).FirstOrDefault();
			itemCount2 = ((itemCount2 > itemChoosed.Stack) ? itemChoosed.Stack : itemCount2);
			if (itemChoosed != null)
			{
				PinItem newPin = new PinItem
				{
					Code = code,
					ItemId = itemId,
					ItemAmount = itemCount2,
					ItemName = itemChoosed.Name,
					DateGenerated = DateTime.Now
				};
				await _pinitemContext.Add(newPin);
				return true;
			}
			Console.WriteLine($"\nO ID {itemId} digitado não foi encontrado na lista dos itens cadastrados em ./Configurations/ItemsAward.json");
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			LogWriter.Write(ex.ToString());
		}
		return false;
	}
}

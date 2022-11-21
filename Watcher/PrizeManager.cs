using System.Linq;
using System.Threading.Tasks;
using CorePinCash.Data;
using CorePinCash.Model;
using CorePinCash.Repository;
using CorePinCash.Server;
using CorePincode.Repository;
using PWToolKit;
using PWToolKit.API.Gamedbd;
using PWToolKit.API.GDeliveryd;
using PWToolKit.API.GProvider;
using PWToolKit.Enums;
using PWToolKit.Models;

namespace CorePinCash.Watcher;

public class PrizeManager
{
	private readonly IPinCashRepository _pincashContext;

	private readonly IPinItemRepository _pinitemContext;

	private readonly ServerConnection _server;

	public PrizeManager(IPinCashRepository pincashContext, IPinItemRepository pinitemContext, ServerConnection server)
	{
		_pincashContext = pincashContext;
		_pinitemContext = pinitemContext;
		_server = server;
		PWGlobal.UsedPwVersion = _server.PwVersion;
	}

	public async Task<bool> SendPincash(PinCash PinCash)
	{
		await UpdatePincode(PinCash);
		DebugAddCash.Add(_server.Gamedbd, PinCash.ObtainedBy, PinCash.CashAmount * 100);
		LogWriter.Write($"Foi creditado {PinCash.CashAmount} gold na conta do personagem {PinCash.ObtainedBy}.");
		Notify(PinCash);
		return true;
	}

	public async Task<bool> SendPinitem(PinItem PinItem)
	{
		await UpdatePincode(PinItem);
		GRoleInventory item = GenerateItem(PinItem);
		if (item != null)
		{
			SysSendMail.Send(_server.GDeliveryd, PinItem.ObtainedBy, "PRÊMIO POR TOKEN", "Você resgatou o código " + PinItem.Code, item);
			LogWriter.Write($"Foi enviado {PinItem.ItemAmount}x {PinItem.ItemName} para caixa de correio do jogador {PinItem.ObtainedBy}.");
			Notify(PinItem);
		}
		else
		{
			LogWriter.Write($"FALHA AO RELACIONAR ITEM DE ID {PinItem.ItemId} VINDO DA TABELA COM OS REGISTROS EM JSON. PROVAVELMENTE O REGISTRO DO ITEM FOI APAGADO DO JSON APÓS SER CADASTRADO NO MODO INTERAÇÃO");
		}
		return true;
	}

	private GRoleInventory GenerateItem(PinItem pinItem)
	{
		Item item = _server.ItemsReward.Where((Item x) => x.Id.Equals(pinItem.ItemId)).FirstOrDefault();
		if (item != null)
		{
			GRolePocket gRolePocket = GetRolePocket.Get(_server.Gamedbd, pinItem.ObtainedBy);
			return new GRoleInventory
			{
				Id = item.Id,
				MaxCount = item.Stack,
				Pos = ((gRolePocket == null) ? 1 : (gRolePocket.Items.Length + 1)),
				Proctype = int.Parse(item.Proctype),
				Octet = item.Octet,
				Mask = int.Parse(item.Mask),
				Count = pinItem.ItemAmount
			};
		}
		return null;
	}

	private async Task UpdatePincode(PinCash PinCash)
	{
		await _pincashContext.Update(PinCash);
	}

	private async Task UpdatePincode(PinItem PinItem)
	{
		await _pinitemContext.Update(PinItem);
	}

	private void Notify(PinCash PinCash)
	{
		PrivateChat.Send(_server.GDeliveryd, PinCash.ObtainedBy, $"Foi creditado {PinCash.CashAmount} gold em sua conta. Relogue o personagem para validar.");
	}

	private void Notify(PinItem PinItem)
	{
		PrivateChat.Send(_server.GDeliveryd, PinItem.ObtainedBy, $"Foi enviado {PinItem.ItemAmount}x {PinItem.ItemName} para sua caixa de correios. Cheque-a!");
	}

	private void GlobalNotify(PinCash PinCash)
	{
		GRoleBase gRoleBase = GetRoleBase.Get(_server.Gamedbd, PinCash.ObtainedBy);
		if (gRoleBase != null)
		{
			ChatBroadcast.Send(_server.GProvider, BroadcastChannel.System, $"{gRoleBase.Name} sacou o código {PinCash.Code} e obteve {PinCash.CashAmount} em sua conta.");
		}
	}
}

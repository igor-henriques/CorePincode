using System.Collections.Generic;
using System.IO;
using CorePinCash.Factory;
using CorePinCash.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PWToolKit;

namespace CorePinCash.Server;

public class ServerConnection
{
	public string logsPath { get; set; }

	public PwVersion PwVersion { get; set; }

	public Gamedbd Gamedbd { get; set; }

	public GDeliveryd GDeliveryd { get; set; }

	public GProvider GProvider { get; set; }

	public List<Item> ItemsReward { get; set; }

	public ServerConnection(ItemAwardFactory itemFactory)
	{
		JObject jObject = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("./Configurations/ServerConnection.json"));
		logsPath = jObject["LOGS_PATH"].ToObject<string>();
		PwVersion = (PwVersion)jObject["PW_VERSION"].ToObject<int>();
		GDeliveryd = new GDeliveryd(jObject["GDELIVERYD"]["HOST"].ToObject<string>(), jObject["GDELIVERYD"]["PORT"].ToObject<int>());
		Gamedbd = new Gamedbd(jObject["GAMEDBD"]["HOST"].ToObject<string>(), jObject["GAMEDBD"]["PORT"].ToObject<int>());
		GProvider = new GProvider(jObject["GPROVIDER"]["HOST"].ToObject<string>(), jObject["GPROVIDER"]["PORT"].ToObject<int>());
		ItemsReward = itemFactory.Get();
	}
}

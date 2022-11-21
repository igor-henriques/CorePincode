using System.Collections.Generic;
using System.IO;
using CorePinCash.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CorePinCash.Factory;

public class ItemAwardFactory
{
	private List<Item> ItemsAward = new List<Item>();

	public ItemAwardFactory()
	{
		JObject jObject = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("./Configurations/ItensAward.json"));
		foreach (KeyValuePair<string, JToken> item2 in jObject)
		{
			Item item = new Item
			{
				Id = int.Parse(item2.Key),
				Name = item2.Value["NOME"].ToObject<string>(),
				Amount = 0,
				Stack = item2.Value["STACK"].ToObject<int>(),
				Octet = item2.Value["OCTET"].ToObject<string>(),
				Proctype = item2.Value["PROCTYPE"].ToObject<string>(),
				Mask = item2.Value["MASK"].ToObject<string>()
			};
			ItemsAward.Add(item);
		}
	}

	public List<Item> Get()
	{
		return ItemsAward;
	}
}

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CorePincode.Model;

public class Definitions
{
	public bool IsSystemWarningActive { get; set; }

	public Definitions()
	{
		JObject jObject = (JObject)JsonConvert.DeserializeObject(File.ReadAllText("./Configurations/Definitions.json"));
		IsSystemWarningActive = jObject["AVISO EM SYSTEM"].ToObject<bool>();
	}
}

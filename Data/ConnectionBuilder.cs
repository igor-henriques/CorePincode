using System.IO;
using CorePinCash.Model;
using Newtonsoft.Json;

namespace CorePinCash.Data;

public class ConnectionBuilder
{
	public static string GetConnectionString()
	{
		DatabaseConnection databaseConnection = JsonConvert.DeserializeObject<DatabaseConnection>(File.ReadAllText("./Configurations/Database.json"));
		return $"Server={databaseConnection.HOST};Port={databaseConnection.PORT};Database={databaseConnection.DB};Uid={databaseConnection.USER};Pwd={databaseConnection.PASSWORD};ConvertZeroDateTime=True";
	}
}

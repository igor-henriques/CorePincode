using CorePincash.Model;

namespace CorePinCash.Model;

public class PlayerInteraction
{
	public int RoleID { get; set; }

	public string Pincode { get; set; }

	public EPincode PincodeType { get; set; }
}

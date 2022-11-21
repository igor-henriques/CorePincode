using System.Collections.Generic;
using CorePinCash.Model;

namespace CorePincash.Model;

public class PinCodes
{
	public List<PinCash> Pincashes { get; set; }

	public List<PinItem> PinItems { get; set; }

	public int TotalRecords { get; private set; }

	public PinCodes(List<PinCash> pinCashes, List<PinItem> pinItems)
	{
		Pincashes = pinCashes;
		PinItems = pinItems;
		TotalRecords = Pincashes.Count + PinItems.Count;
	}
}

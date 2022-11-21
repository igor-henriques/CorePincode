using System;
using System.ComponentModel.DataAnnotations;

namespace CorePinCash.Model;

public class PinItem
{
	[Required]
	public int Id { get; set; }

	[Required]
	public string Code { get; set; }

	[Required]
	public int ItemId { get; set; }

	[Required]
	public int ItemAmount { get; set; }

	[MaxLength(50)]
	public string ItemName { get; set; }

	public DateTime DateGenerated { get; set; }

	public bool Obtained { get; set; }

	public int ObtainedBy { get; set; }

	public DateTime DateObtained { get; set; }
}

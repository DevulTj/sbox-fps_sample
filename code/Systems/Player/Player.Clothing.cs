using Sandbox;

namespace Facepunch.Gunfight;

public partial class Player
{
	public ClothingContainer Clothing { get; protected set; }

	/// <summary>
	/// Set the clothes to whatever the player is wearing
	/// </summary>
	public void UpdateClothes()
	{
		Clothing = new();

		Clothing.ClearEntities();
		Clothing.LoadFromClient( Client );
		Clothing.DressEntity( this );
	}
}

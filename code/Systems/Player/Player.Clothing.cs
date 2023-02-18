namespace GameTemplate;

public partial class Player
{
	public ClothingContainer Clothing { get; protected set; }

	/// <summary>
	/// Set the clothes to whatever the player is wearing
	/// </summary>
	public void SetupClothing()
	{
		Clothing = new();

		Clothing.ClearEntities();
		Clothing.LoadFromClient( Client );
		Clothing.DressEntity( this );
	}
}

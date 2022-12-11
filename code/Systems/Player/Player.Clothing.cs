using Sandbox;

namespace Facepunch.Gunfight;

public partial class Player
{
	public ClothingContainer Clothing { get; protected set; }

	string armyOutfitJson = "[{\"id\":502735166},{\"id\":-1330488900},{\"id\":1558172885},{\"id\":181315396},{\"id\":-1492226718},{\"id\":930368271},{\"id\":1356410853},{\"id\":-236248938}]";
	string terrorOutfitJson = "[{\"id\":-69855493},{\"id\":1356410853},{\"id\":-1293797531},{\"id\":-100519886},{\"id\":1299698732},{\"id\":1761917151},{\"id\":-1027298053},{\"id\":-230983670}]";

	/// <summary>
	/// Set the clothes to whatever the player is wearing
	/// </summary>
	public void UpdateClothes()
	{
		Clothing = new();

		var outfit = Game.Clients.Count % 2 == 0 ? armyOutfitJson : terrorOutfitJson;
		Clothing.Deserialize( outfit );
		Clothing.DressEntity( this );
	}
}

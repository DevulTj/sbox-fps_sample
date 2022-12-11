using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Ammo : WeaponComponent, ISingletonComponent
{
	[Net] public int AmmoCount { get; set; }

	protected ComponentData Data => Weapon.WeaponData.Ammo;

	public bool IsFull
	{
		get => AmmoCount >= ( Data.AllowChamber ? Data.MaximumAmmo + 1 : Data.MaximumAmmo ); 
	}

	public override void Initialize( Weapon weapon )
	{
		// TODO - this is shit
		if ( Game.IsServer )
			AmmoCount = weapon.WeaponData.Ammo.DefaultAmmo;
	}

	public void Fill()
	{
		if ( AmmoCount == Data.MaximumAmmo && Data.AllowChamber )
		{
			++AmmoCount;
			return;
		}

		AmmoCount = Data.DefaultAmmo;
	}

	public bool HasEnoughAmmo( int amount = 1 )
	{
		return AmmoCount >= amount;
	}

	public bool TakeAmmo( int amount = 1 )
	{
		if ( AmmoCount >= amount )
		{
			AmmoCount -= amount;
			return true;
		}

		return false;
	}

	public struct ComponentData
	{
		public int DefaultAmmo { get; set; }
		public int MaximumAmmo { get; set; }
		public bool AllowChamber { get; set; }
	}
}

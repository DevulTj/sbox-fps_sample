using GameTemplate.Weapons;

namespace GameTemplate;

/// <summary>
/// The player's inventory holds a player's weapons, and holds the player's current weapon.
/// It also drives functionality such as weapon switching.
/// </summary>
public partial class Inventory : EntityComponent<Player>, ISingletonComponent
{
	[Net] public IList<Weapon> Weapons { get; set; }
	[Net, Predicted] public Weapon ActiveWeapon { get; set; }

	public bool AddWeapon( Weapon weapon, bool makeActive = true )
	{
		if ( Weapons.Contains( weapon ) ) return false;

		Weapons.Add( weapon );

		if ( makeActive )
			SetActiveWeapon( weapon );

		return true;
	}

	public bool RemoveWeapon( Weapon weapon, bool drop = false )
	{
		var success = Weapons.Remove( weapon );
		if ( success && drop )
		{
			// TODO - Drop the weapon on the ground
		}

		return success;
	}

	public void SetActiveWeapon( Weapon weapon )
	{
		var currentWeapon = ActiveWeapon;
		if ( currentWeapon.IsValid() )
		{
			// Can reject holster if we're doing an action already
			if ( !currentWeapon.CanHolster( Entity ) )
			{
				return;
			}

			currentWeapon.OnHolster( Entity );
			ActiveWeapon = null;
		}

		// Can reject deploy if we're doing an action already
		if ( !weapon.CanDeploy( Entity ) )
		{
			return;
		}

		ActiveWeapon = weapon;

		weapon?.OnDeploy( Entity );
	}

	protected override void OnDeactivate()
	{
		if ( Game.IsServer )
		{
			Weapons.ToList()
				.ForEach( x => x.Delete() );
		}
	}

	public Weapon GetSlot( int slot )
	{
		return Weapons.ElementAtOrDefault( slot ) ?? null;
	}

	protected int GetSlotIndexFromInput( InputButton slot )
	{
		return slot switch
		{
			InputButton.Slot1 => 0,
			InputButton.Slot2 => 1,
			InputButton.Slot3 => 2,
			InputButton.Slot4 => 3,
			InputButton.Slot5 => 4,
			_ => -1
		};
	}

	protected void TrySlotFromInput( InputButton slot )
	{
		if ( Input.Pressed( slot ) )
		{
			Input.SuppressButton( slot );

			if ( GetSlot( GetSlotIndexFromInput( slot ) ) is Weapon weapon )
			{
				Entity.ActiveWeaponInput = weapon;
			}
		}
	}

	public void BuildInput()
	{
		TrySlotFromInput( InputButton.Slot1 );
		TrySlotFromInput( InputButton.Slot2 );
		TrySlotFromInput( InputButton.Slot3 );
		TrySlotFromInput( InputButton.Slot4 );
		TrySlotFromInput( InputButton.Slot5 );

		ActiveWeapon?.BuildInput();
	}

	public void Simulate( IClient cl )
	{
		if ( Entity.ActiveWeaponInput != null && ActiveWeapon != Entity.ActiveWeaponInput )
		{
			SetActiveWeapon( Entity.ActiveWeaponInput as Weapon );
			Entity.ActiveWeaponInput = null;
		}

		ActiveWeapon?.Simulate( cl );
	}

	public void FrameSimulate( IClient cl )
	{
		ActiveWeapon?.FrameSimulate( cl );
	}
}

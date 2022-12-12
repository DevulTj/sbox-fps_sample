using Facepunch.Gunfight.WeaponSystem;
using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Facepunch.Gunfight;

/// <summary>
/// The player's inventory holds a player's weapons, and holds the player's current weapon.
/// It also drives functionality such as weapon switching.
/// </summary>
public partial class Inventory : BaseNetworkable
{
	[Net] public Player Owner { get; set; }
	[Net] protected IList<Weapon> Weapons { get; set; }
	[Net, Predicted] public Weapon ActiveWeapon { get; set; }

	public Inventory() { }
	public Inventory( Player player ) => Owner = player;

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
		// SetActiveWeapon can only be called on the server realm.
		Game.AssertServer();

		var currentWeapon = ActiveWeapon;
		if ( currentWeapon.IsValid() )
		{
			// Can reject holster if we're doing an action already
			if ( !currentWeapon.CanHolster( Owner ) )
			{
				return;
			}

			currentWeapon.OnHolster( Owner );
			// Make the weapon invisible to everyone.
			currentWeapon.EnableDrawing = false;

			ActiveWeapon = null;
		}

		// Can reject deploy if we're doing an action already
		if ( !weapon.CanDeploy( Owner ) )
		{
			return;
		}

		ActiveWeapon = weapon;
		weapon?.OnDeploy( Owner );

		// Parent the weapon to its new owner, and make it visible to everyone.
		weapon.SetParent( Owner, true );
		weapon.Owner = Owner;
		weapon.EnableDrawing = true;
	}

	public void Delete()
	{
		Weapons.ToList().ForEach( x => x.Delete() );
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
				Owner.ActiveWeaponInput = weapon;
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
	}

	public void Simulate( IClient cl )
	{
		if ( Game.IsServer )
		{
			if ( Owner.ActiveWeaponInput != null && ActiveWeapon != Owner.ActiveWeaponInput )
			{
				SetActiveWeapon( Owner.ActiveWeaponInput as Weapon );
			}
		}

		ActiveWeapon?.Simulate( cl );
	}

	public void FrameSimulate( IClient cl )
	{
		ActiveWeapon?.FrameSimulate( cl );
	}
}

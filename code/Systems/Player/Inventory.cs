using Facepunch.Gunfight.WeaponSystem;
using Sandbox;
using System.Collections.Generic;

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
}

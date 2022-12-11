using Sandbox;
using System;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class WeaponComponent : EntityComponent<Weapon>
{
	/// <summary>
	/// Accessor.
	/// </summary>
	protected Weapon Weapon => Entity;
	protected Player Player => Weapon.Owner as Player;

	[Net, Predicted] public bool IsActive { get; protected set; }
	[Net, Predicted] public TimeSince TimeSinceActivated { get; protected set; }

	protected virtual bool UseLagCompensation => false;

	/// <summary>
	/// Called when the owning player has used this weapon.
	/// </summary>
	/// <param name="player"></param>
	protected virtual void OnActivated( Player player )
	{
		TimeSinceActivated = 0;
	}

	/// <summary>
	/// Dictates whether this entity is usable by given user.
	/// </summary>
	/// <param name="player"></param>
	/// <returns>Return true if the given entity can use/interact with this entity.</returns>
	protected virtual bool CanActivate( Player player )
	{
		return true;
	}

	/// <summary>
	/// Called every tick.
	/// </summary>
	/// <param name="cl"></param>
	/// <param name="player"></param>
	public virtual void Simulate( IClient cl, Player player )
	{
		var before = IsActive;

		if ( CanActivate( player ) )
		{
			if ( UseLagCompensation )
			{
				using ( Sandbox.Entity.LagCompensation() )
				{
					OnActivated( player );
					IsActive = true;
				}
			}
			else
			{
				OnActivated( player );
				IsActive = true;
			}
		}
		else if ( before )
		{
			IsActive = false;
			OnDeactivated( player );
		}
	}

	protected virtual void OnDeactivated( Player player )
	{
		//
	}

	public virtual void Initialize( Weapon weapon )
	{
		//
	}
}

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

	public virtual string Name => info.Name.Replace( " ", "" );
	protected virtual bool UseLagCompensation => false;
	protected virtual bool EnableActivateEvents => true;

	DisplayInfo info;
	public WeaponComponent()
	{
		info = DisplayInfo.For( this );
	}

	/// <summary>
	/// Accessor to grab components from the weapon
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T GetComponent<T>() where T : WeaponComponent
	{
		return Weapon.GetComponent<T>();
	}

	/// <summary>
	/// Run a weapon event
	/// </summary>
	/// <param name="eventName"></param>
	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}

	/// <summary>
	/// Called when the owning player has used this weapon.
	/// </summary>
	/// <param name="player"></param>
	protected virtual void OnActivated( Player player )
	{
		TimeSinceActivated = 0;

		if ( EnableActivateEvents )
			RunGameEvent( $"{Name}.activate" );
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

		if ( !IsActive && CanActivate( player ) )
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
		else if ( before && !CanActivate( player ) )
		{
			IsActive = false;
			OnDeactivated( player );
		}
	}

	protected virtual void OnDeactivated( Player player )
	{
		if ( EnableActivateEvents )
			RunGameEvent( $"{Name}.deactivate" );
	}

	public virtual void Initialize( Weapon weapon )
	{
		//
	}

	public virtual void OnGameEvent( string eventName )
	{
		//
	}
}

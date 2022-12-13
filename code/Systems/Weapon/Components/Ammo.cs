using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Ammo : WeaponComponent, ISingletonComponent
{
	[Net] public int AmmoCount { get; set; }

	protected bool ReloadLock { get; set; } = false;
	public TimeUntil TimeUntilReloaded { get; set; }

	public ComponentData Data => Weapon.WeaponData.Ammo;

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

	public override void OnGameEvent( string eventName )
	{
		if ( eventName == "primaryfire.activate" )
		{
			TakeAmmo();
		}
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

	/////
	/// Reloading
	/////
	protected override bool CanActivate( Player player )
	{
		if ( ReloadLock ) return false;

		return Input.Pressed( InputButton.Reload );
	}

	protected override void OnActivated( Player player )
	{
		base.OnActivated( player );

		if ( Weapon.GetComponent<Ammo>()?.IsFull ?? false )
		{
			return;
		}

		TimeUntilReloaded = Data.ReloadTime;
		ReloadLock = true;

		StartReloading();
	}

	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );

		if ( ReloadLock && TimeUntilReloaded )
		{
			FinishReloading( player );
			ReloadLock = false;
		}
	}

	protected void StartReloading()
	{
		Player?.SetAnimParameter( "b_reload", true );
		Weapon.Tags.Set( "reloading", true );

		using ( Prediction.Off() )
			StartReloadEffects( To.Single( Player.Client ) );
	}

	[ClientRpc]
	public static void StartReloadEffects()
	{
		WeaponViewModel.Current?.SetAnimParameter( "reload", true );
	}

	protected void FinishReloading( Player player )
	{
		Weapon.Tags.Set( "reloading", false );
		Weapon.GetComponent<Ammo>().Fill();
	}

	public struct ComponentData
	{
		public int DefaultAmmo { get; set; }
		public int MaximumAmmo { get; set; }
		public bool AllowChamber { get; set; }

		[Category( "Reloading" )]
		public float ReloadTime { get; set; }
	}
}

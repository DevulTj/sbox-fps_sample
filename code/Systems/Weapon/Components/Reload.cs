using Sandbox;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Reload : WeaponComponent, ISingletonComponent
{
	protected bool Lock { get; set; } = false;

	public TimeUntil TimeUntilReloaded { get; set; }

	public ComponentData Data => Weapon.WeaponData.Reload;

	protected override bool CanActivate( Player player )
	{
		return Input.Pressed( InputButton.Reload );
	}

	protected override void OnActivated( Player player )
	{
		if ( Weapon.GetComponent<Ammo>()?.IsFull ?? false )
		{
			return;
		}

		TimeUntilReloaded = Data.ReloadTime;
		Lock = true;

		StartReloading();
	}

	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );

		if ( Lock && TimeUntilReloaded )
		{
			FinishReloading( player );
			Lock = false;
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

	/// <summary>
	/// Data asset information.
	/// </summary>
	public struct ComponentData
	{
		public float ReloadTime { get; set; }
	}
}

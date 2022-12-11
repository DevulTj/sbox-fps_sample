using Sandbox;
using System.Collections.Generic;

namespace Facepunch.Gunfight.WeaponSystem;

[GameResource( "Weapon", "weapon", "A data asset for a weapon.",
	Icon = "track_changes", IconBgColor = "#4953a7", IconFgColor = "#2a3060" )]
public partial class WeaponData : GameResource
{
	[Category( "Basic Information" )]
	public string Name { get; set; } = "My weapon name";

	[Category( "Basic Information" ), ResourceType( "vmdl" )]
	public string Model { get; set; }

	[Category( "Basic Information" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; }

	internal Model CachedModel;
	internal Model CachedViewModel;

	[Category( "Basic Information" ), ResourceType( "jpg" )]
	public string Icon { get; set; }

	[Category( "Animation" )]
	public HoldType HoldType { get; set; } = HoldType.Pistol;

	[Category( "Animation" )]
	public Handedness Handedness { get; set; } = Handedness.Both;

	[Category( "Basic Information" )]
	public List<string> Components { get; set; }

	public ViewModelData ViewModelData { get; set; }

	// Component Information
	public PrimaryFire.ComponentData PrimaryFire { get; set; }
	public Aim.ComponentData Aim { get; set; }
	public Reload.ComponentData Reload { get; set; }
	public Ammo.ComponentData Ammo { get; set; }
	public Recoil.ComponentData Recoil { get; set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		Log.Info( $"Registering weapon ({ResourcePath}, {Name})" );

		if ( !All.Contains( this ) )
			All.Add( this );

		if ( !string.IsNullOrEmpty( Model ) )
			CachedModel = Sandbox.Model.Load( Model );

		if ( !string.IsNullOrEmpty( ViewModel ) )
			CachedViewModel = Sandbox.Model.Load( ViewModel );
	}
}

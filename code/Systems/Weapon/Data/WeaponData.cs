namespace GameTemplate.Weapons;

[GameResource( "Weapon", "weapon", "A data asset for a weapon.", Icon = "track_changes", IconBgColor = "#4953a7", IconFgColor = "#2a3060" )]
public partial class WeaponData : GameResource
{
	[Category( "Basic Information" )]
	public string Name { get; set; } = "My weapon name";

	[Category( "Basic Information" ), ResourceType( "vmdl" )]
	public string Model { get; set; }

	[Category( "Basic Information" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; }

	[Category( "Basic Information" ), ResourceType( "jpg" )]
	public string Icon { get; set; }

	[Category( "Animation" )]
	public WeaponHoldType HoldType { get; set; } = WeaponHoldType.Pistol;

	[Category( "Animation" )]
	public WeaponHandedness Handedness { get; set; } = WeaponHandedness.Both;

	[Category( "Basic Information" )]
	public List<string> Components { get; set; }

	public ViewModelData ViewModelData { get; set; }

	// Component Information
	public PrimaryFire.ComponentData PrimaryFire { get; set; }
	
	internal Model CachedModel;
	internal Model CachedViewModel;

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

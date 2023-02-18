namespace GameTemplate.Weapons;

[Title( "ViewModel" ), Icon( "pan_tool" )]
public partial class WeaponViewModel : AnimatedEntity
{
	/// <summary>
	/// All active view models.
	/// </summary>
	public static WeaponViewModel Current;

	protected Weapon Weapon { get; init; }

	public WeaponViewModel( Weapon weapon )
	{
		if ( Current.IsValid() )
		{
			Current.Delete();
		}

		Current = this;
		EnableShadowCasting = false;
		EnableViewmodelRendering = true;
		Weapon = weapon;
	}

	protected override void OnDestroy()
	{
		Current = null;
	}

	[Event.Client.PostCamera]
	public void PlaceViewmodel()
	{
		if ( Game.IsRunningInVR )
			return;

		Camera.Main.SetViewModelCamera( 80f, 1, 500 );
		AddRecoilEffects();
		AddEffects();
	}

	public override Sound PlaySound( string soundName, string attachment )
	{
		if ( Owner.IsValid() )
			return Owner.PlaySound( soundName, attachment );

		return base.PlaySound( soundName, attachment );
	}
}

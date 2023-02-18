namespace GameTemplate.Weapons;

public partial class WeaponComponent : EntityComponent<Weapon>
{
	protected Weapon Weapon => Entity;
	protected Player Player => Weapon.Owner as Player;
	protected string Identifier => DisplayInfo.ClassName.Trim();
	protected virtual bool UseGameEvents => true;

	/// <summary>
	/// Is the weapon component active? Could mean are we shooting, reloading, aiming..
	/// </summary>
	[Net, Predicted] public bool IsActive { get; protected set; }

	/// <summary>
	/// Time (in seconds) since IsActive = true
	/// </summary>
	[Net, Predicted] public TimeSince TimeSinceActivated { get; protected set; }

	DisplayInfo? displayInfo;

	/// <summary>
	/// Cached DisplayInfo for this weapon, so we don't fetch it every single time we fire events.
	/// </summary>
	public DisplayInfo DisplayInfo
	{
		get
		{
			displayInfo ??= DisplayInfo.For( this );
			return displayInfo.Value;
		}
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
	protected virtual void OnStart( Player player )
	{
		TimeSinceActivated = 0;

		if ( UseGameEvents )
			RunGameEvent( $"{Identifier}.start" );
	}

	/// <summary>
	/// Dictates whether this entity is usable by given user.
	/// </summary>
	/// <param name="player"></param>
	/// <returns>Return true if the given entity can use/interact with this entity.</returns>
	protected virtual bool CanStart( Player player )
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

		if ( !IsActive && CanStart( player ) )
		{
			using ( Sandbox.Entity.LagCompensation() )
			{
				OnStart( player );
				IsActive = true;
			}
		}
		else if ( before && !CanStart( player ) )
		{
			IsActive = false;
			OnStop( player );
		}
	}

	/// <summary>
	/// Called when the component action stops. See <see cref="Simulate(IClient, Player)"/>
	/// </summary>
	/// <param name="player"></param>
	protected virtual void OnStop( Player player )
	{
		if ( UseGameEvents )
			RunGameEvent( $"{Identifier}.stop" );
	}

	/// <summary>
	/// Called when the weapon gets made on the server.
	/// NOTE: Need to remove this as we should just be able to use OnActivated
	/// </summary>
	/// <param name="weapon"></param>
	public virtual void Initialize( Weapon weapon )
	{
		//
	}

	/// <summary>
	/// Called when a game event is sent to the player.
	/// </summary>
	/// <param name="eventName"></param>
	public virtual void OnGameEvent( string eventName )
	{
		//
	}

	/// <summary>
	/// Called every Weapon.BuildInput
	/// </summary>
	public virtual void BuildInput()
	{
		//
	}
}

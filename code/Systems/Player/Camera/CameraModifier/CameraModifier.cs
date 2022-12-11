using Sandbox;
using System.Collections.Generic;

namespace Facepunch.Gunfight;

public abstract class CameraModifier
{
	internal static List<CameraModifier> List = new();

	internal static void Apply()
	{
		for ( int i = List.Count; i > 0; i-- )
		{
			var entry = List[i - 1];
			var keep = entry.Update();

			if ( !keep )
			{
				entry.OnRemove();
				List.RemoveAt( i - 1 );
			}
		}
	}

	protected virtual void OnRemove()
	{
		//
	}

	public static void ClearAll()
	{
		List.Clear();
	}

	public CameraModifier()
	{
		if ( Prediction.FirstTime )
		{
			List.Add( this );
		}
	}

	public abstract bool Update();
}

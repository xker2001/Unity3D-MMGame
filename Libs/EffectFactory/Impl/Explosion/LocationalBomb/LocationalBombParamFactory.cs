
#if PLUGIN_TRIGGEREVENTPRO
using UnityEngine;
using MMGame;
using MMGameLibs.Custom.Combat;
using MMGameLibs.Produce.Indicator;

namespace MMGameLibs.Produce.Bomb  {
[System.Serializable]
/** 爆炸参数 */
public class LocationalBombParamFactory: PlayOneShotParamFactory {
	[SerializeField]
	private ExplosionParamFactory explosion;
	public ExplosionParamFactory Explosion { get { return explosion; } }

	[SerializeField]
	private RangeIndicatorParamFactory rangeIndicator;
	public RangeIndicatorParamFactory RangeIndicator { get { return rangeIndicator; } }

	/**
	 * Delay from indicator been shown to fading out
	 */
	[SerializeField]
	private float delayOfFadeout = 1;
	public float DelayOfFadeout { get { return delayOfFadeout; } }


	/**
	 * Delay from indicator been shown to exploding
	 */
	[SerializeField]
	private float delayOfExploding = 2;
	public float DelayOfExploding { get { return delayOfExploding; } }

	[SerializeField]
	private CampSide side = CampSide.None;
	public CampSide Side {
		get {
			return side;
		}
		set {
			side = value;
		}
	}


	// interface methods

	//
	public override bool IsNull() {
		return Explosion.Range <= Mathf.Epsilon && Explosion.ExplodeEffect.IsNull();
	}


	//
	public override ParamObject Produce() {
		Transform xform = PoolManager.Spawn (ExplosionParamSettings.Params.PoolName, Prefab.Load ("LocationalBombParamObject"));
		LocationalBombParamObject obj = xform.GetComponent<LocationalBombParamObject>();
		obj.SetParameters (this);
		return obj;
	}
}
}
#endif
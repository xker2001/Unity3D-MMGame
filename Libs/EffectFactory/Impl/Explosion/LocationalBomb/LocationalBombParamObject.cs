
#if PLUGIN_TRIGGEREVENTPRO
using UnityEngine;
using MMGame;
using System.Collections;
using MMGameLibs.Custom.Combat;
using MMGameLibs.Produce.Indicator;

namespace MMGameLibs.Produce.Bomb  {
public class LocationalBombParamObject : PlayOneShotParamObject {
	// private
	private LocationalBombParamFactory _factory;
	private RangeIndicatorParamObject _indicateObj;
	private PlayOneShotParamObject _explodeObj;
	private bool _isPlaying;


	public AdditionalHitResultParameters AdditionalHitResult { get; set; }


	// public methods

	//
	public void SetParameters (LocationalBombParamFactory factory) {
		_factory = factory;
		_factory.Explosion.Side = _factory.Side;
		_factory.RangeIndicator.SetRadius (_factory.Explosion.Range);
	}


	// process methods

	//
	protected override void OnEnable() {
		_indicateObj = null;
	}


	//
	protected override void OnDisable() {
		AdditionalHitResult = default (AdditionalHitResultParameters);
		CancelInvoke();
	}


	// interfac methods

	//
	public override void PlayAndDestroy() {
		if (_factory.IsNull()) {
			DespawnParamObject();
			return;
		}

		if (_isPlaying) {
			return;
		}

		if (!_factory.RangeIndicator.IsNull()) {
			_indicateObj = _factory.RangeIndicator.Create (transform);
			_indicateObj.Show();
		}

		Invoke ("_DoFadeout", _factory.DelayOfFadeout);
		Invoke ("_DoExplode", _factory.DelayOfExploding);
	}


	//
	public override void Destroy() {
		CancelInvoke();

		if (_indicateObj) {
			_indicateObj.Destroy();
			_indicateObj = null;
		}

		if (_explodeObj) {
			_explodeObj.Destroy();
			_explodeObj = null;
		}

		_isPlaying = false;
		DespawnParamObject();
	}


	// private methods

	//
	private void _DoFadeout() {
		if (_indicateObj) {
			_indicateObj.transform.parent = null;
			_indicateObj.Destroy();
		}
	}


	//
	private void _DoExplode() {
		if (!_factory.Explosion.IsNull()) {
			ExplosionParamObject bomb = (ExplosionParamObject)
										_factory.Explosion.Create (transform.position);
			bomb.AdditionalHitResult = AdditionalHitResult;
			bomb.Owner = gameObject;
			bomb.PlayAndDestroy();
		}

		_indicateObj = null;
		_explodeObj = null;
		_isPlaying = false;
		DespawnParamObject();
	}
}
}
#endif
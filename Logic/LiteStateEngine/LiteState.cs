using System.Collections;
using System.Collections.Generic;

public abstract class LiteState {
	public LiteStateEngine stateEngine {
		get; private set;
	}
	public void InitByStateEngine(LiteStateEngine liteStateEngine) {
		stateEngine = liteStateEngine;
	}
	//used to dispose things
	public virtual void SetStateActive(bool isActive) { }
	public virtual void OnEnter() { }
	public virtual void OnExit() { }
	public virtual void OnResume() { }
	public virtual void OnPause() { }
}



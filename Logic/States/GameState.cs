using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class GameState : LiteState {
	public abstract void OnTick();
	public abstract void Render();
}

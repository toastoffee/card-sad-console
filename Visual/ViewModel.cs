using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CardConsole.Visual;

public enum GameStateType {
	Battle,
	Route
}

public class RouteOptionViewModel {
	public string description;
	public int index;
}

public class ViewModel {
	public int turn;
	public int eng;
	public int maxEng;
	public int playerHp;
	public int maxPlayerHp;
	public int playerShield; // 添加玩家护盾值
	public PlayerProp playerProp; // 添加玩家总属性
	public GameStateType currentStateType; // 添加状态类型标识

	// Route状态相关数据
	public string routeDescription = "";
	public List<RouteOptionViewModel> routeOptions = new List<RouteOptionViewModel>();

	public List<EnemyViewModel> enemies = new List<EnemyViewModel>(); // 添加敌人视图模型列表
	public List<CardViewModel> handCards = new List<CardViewModel>(); // 玩家手牌列表

	public List<string> gameLogs = new List<string>();    // 游戏相关的日志（如战斗、回合等）
	public List<string> systemLogs = new List<string>();  // 系统相关的日志（如程序运行、错误等）
}

public class EnemyViewModel {
	public string name;
	public int hp;
	public int maxHp;
	public string intention;
}

public class CardViewModel {
	public string name;
	public int cost;
	public string description;
}

using CardConsole.Visual;
using SadConsole.Configuration;

Settings.WindowTitle = "My SadConsole Game";

AutoModelTableImpl.InitTables();

Builder configuration = new Builder()
    .SetScreenSize(240, 70)
    .SetStartingScreen<RootScreen>()
    .IsStartingScreenFocused(true)
    ;

Game.Create(configuration);
Game.Instance.Run();
Game.Instance.Dispose();
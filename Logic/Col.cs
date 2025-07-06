using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Col {
  public static Color cardWhite => Color.White * 0.9f;
  public static Color magicGreen => new Color(200, 255, 200);

  public static Dictionary<CardType, Color> cardTypeColors = new Dictionary<CardType, Color> {
    { CardType.WEAPON, Color.AnsiRedBright * 0.8f },
    { CardType.ARMOR, Color.AnsiBlueBright * 0.8f },
    { CardType.HELMET, Color.AnsiGreenBright * 0.8f },
    { CardType.SHOE, Color.AnsiYellowBright * 0.8f },
    { CardType.TRINKET, Color.AnsiWhiteBright * 0.8f },
    { CardType.MAGIC, magicGreen },
  };
}
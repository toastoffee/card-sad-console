
public static class BuffExtension {
  [Obsolete] //until u really need this
  public static CharPropSchema.Modifier ToModifier(this CharObject.Buff buff) {
    switch(buff.buffId) {
      case BattleContext.BuffId.攻击力:
        return new CharPropSchema.Modifier(schema => schema.atk) { type = ModifierType.Add, value = buff.stack };
    }
    return null;
  }
  [Obsolete] //until u really need this
  public static CharObject.Buff TryGetBuff(this List<CharObject.Buff> buffs, BattleContext.BuffId buffId) {
    var buff = buffs.FirstOrDefault(buff => buff.buffId == buffId);

    return buff;
  }
}
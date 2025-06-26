using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class RogueBattleState {
	private void PreUseCard(int idx) {
		var card = tokens[idx];
		if (card.name == "SWAP") {
			PreuseCard_SWAP(card);
			return;
		}

		UseCard(card);
	}

	private void UseCard(CardObjcet card) {
		if (card.cost > mana) {
			Log.Push("not enough energy.");
			return;
		}


		if (card != null) {
			Log.Push($"use hand：[{card.cardModel.modelId}]");
			tokens.Remove(card);
		}

		mana -= card.cost;

		if (card.cardModel.modelId == nameof(CardDefine.Strike)) {
			Log.PushSys("attacking");
			DoDamageToEnemy(0, 6);
		}

		if (card.cardModel.modelId == nameof(CardDefine.Defend)) {
			Log.PushSys("defending");
			ApplyShieldToPlayer(5);
		}

		if (card.cardModel.modelId == nameof(CardDefine.Hit)) {
			DoDamageToEnemy(0, 15);
		}

		if (card.cardModel.modelId == nameof(CardDefine.Swap)) {
			UseCard_SWAP();
		}

		yard.Add(card);
	}


	private void DoDamageToEnemy(int idx, int dmg) {
		var enemy = enemys[idx];
		DoAttack(new AttackParam {
			attacker = playerCharObj,
			deffender = enemy,
			dmg = dmg,
		});
	}

	private void ApplyShieldToPlayer(int shield) {
		GainShiled(playerCharObj, shield);
	}

	private class SWAP_Context {
		public CardType type;
	}

	private void PreuseCard_SWAP(CardObjcet card) {
		GotoCardSelect();
		var ctx = new SWAP_Context();
		selectCardHandler = (input) => {
			var selectCard = input.card;
			if (input.isBreak) {
				GotoIdle();
				return;
			}
			if (selectCard.type != CardType.WEAPON && selectCard.type != CardType.ARMOR) {
				Log.Push($"只能弃掉武器或防具卡牌，当前卡牌类型: {EnumTrans.Get(selectCard.type)}");
				return;
			}
			ctx.type = selectCard.type;
			Log.Push($"选择了卡牌：[{selectCard.cardModel.modelId}]，弃掉所有[{EnumTrans.Get(selectCard.type)}]");
			GotoIdle();
			UseCard(card);
		};
		selectCardCtx = ctx;
	}

	private void UseCard_SWAP() {
		var ctx = selectCardCtx as SWAP_Context;
		var drawType = ctx.type == CardType.WEAPON ? CardType.ARMOR : CardType.WEAPON;
		var discardCnt = 0;
		for (int i = 0; i < tokens.Count; i++) {
			if (tokens[i].type == ctx.type) {
				discardCnt++;
				DiscardCard(tokens[i]);
			}
		}
		for (int i = 0; i < discardCnt; i++) {
			var draw = deck.Find(x => x.type == drawType);
			if (draw != null) {
				PickFromDeck(draw);
			}
		}
	}

	public struct AttackParam {
		public CharObject attacker;
		public CharObject deffender;
		public int dmg;
	}

	private void DoAttack(AttackParam param) {
		if (param.deffender == playerCharObj) {
			Trigger_OnBeforePlayerBeAttack(param);
		}

		var shieldDmg = (int)MathF.Min(param.dmg, param.deffender.shield);
		param.deffender.shield -= shieldDmg;
		param.deffender.hp -= param.dmg - shieldDmg;
		Log.Push($"[{param.attacker.name}]deal [{param.dmg}] dmg to [{param.deffender.name}]，remain hp {param.deffender.hp} shield {param.deffender.RemainShieldStr()}");

		if (param.deffender == playerCharObj) {
			Trigger_OnAfterPlayerBeAttack(param);
		}
	}
	private void GainShiled(CharObject cha, int shield) {
		cha.shield += shield;
		Log.Push($"[{cha.name}] gain [{shield}] shield，current: {cha.RemainShieldStr()}");
	}

	private void Trigger_OnBeforePlayerBeAttack(AttackParam param) {

	}

	private void Trigger_OnAfterPlayerBeAttack(AttackParam param) {

	}
}
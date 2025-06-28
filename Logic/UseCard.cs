using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class RogueBattleState {
	public void PreUseCard(int idx) {
		var card = tokens[idx];
		if (card.name == nameof(CardDefine.Swap)) {
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
			DoDamageToEnemy(0, 6);
		}

		if (card.cardModel.modelId == nameof(CardDefine.Defend)) {
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
		Log.Push("chose a card to swap (Weapon or Armod.)");
		var ctx = new SWAP_Context();
		selectCardHandler = (input) => {
			var selectCard = input.card;
			if (input.isBreak) {
				GotoIdle();
				return;
			}
			if (selectCard.type != CardType.WEAPON && selectCard.type != CardType.ARMOR) {
				Log.Push($"u can only select Weapon or Armor.");
				return;
			}
			ctx.type = selectCard.type;
			Log.Push($"chosen：[{selectCard.cardModel.modelId}]，discard all [{EnumTrans.Get(selectCard.type)}]");
			selectCardCtx = ctx;
			UseCard(card);
			GotoIdle();
		};
		selectCardCtx = ctx;
	}

	private void UseCard_SWAP() {
		var ctx = selectCardCtx as SWAP_Context;
		if (ctx == null) {
			Log.Push("[ERROR] SWAP context is null!");
			return;
		}

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
			// 清理上下文
			selectCardCtx = null;
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
		Log.Push($"[{param.attacker.name}]deal [{param.dmg}] dmg to [{param.deffender.name}]，remain hp {param.deffender.hp} shield {param.deffender.shield}");

		if (param.deffender == playerCharObj) {
			Trigger_OnAfterPlayerBeAttack(param);
		}
	}
	private void GainShiled(CharObject cha, int shield) {
		cha.shield += shield;
		Log.Push($"[{cha.name}] gain [{shield}] shield，current: {cha.shield}");
	}

	private void Trigger_OnBeforePlayerBeAttack(AttackParam param) {

	}

	private void Trigger_OnAfterPlayerBeAttack(AttackParam param) {

	}
}
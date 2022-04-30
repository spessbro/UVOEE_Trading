using System;
using System.Collections.Generic;
using System.Linq;
using Outposts;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;


namespace UVOEE_Trading{
	public class Trading_Outpost : Outpost{
		public override void Produce(){
			base.Produce();
			List<Thing> Sellables = Things.Where(t => t.def != ThingDefOf.Silver && (t.def.tradeability ==Tradeability.Sellable || t.def.tradeability ==Tradeability.All)).ToList();
			if(totalWealth < sellCapacity){
				foreach(Thing t in Sellables){ TakeItem(t);}
				return;
			}

			Sellables.Sort((a, b) => SellValue(a).CompareTo(SellValue(b)));
			int remainingCapacity = sellCapacity;
			foreach(Thing t in Sellables){
				if(SellValue(t) > remainingCapacity){
					t.SplitOff((int)Math.Ceiling((float)(remainingCapacity/SellValue(t)*t.stackCount)));
					break;
				}
				remainingCapacity -= SellValue(t);
				TakeItem(t);
			}


		}


		public int totalWealth => Things.Where(t => t.def != ThingDefOf.Silver && (t.def.tradeability ==Tradeability.Sellable || t.def.tradeability ==Tradeability.All)).Sum(t => SellValue(t));
		public int sellCapacity => CapablePawns.Sum(p => p.skills.GetSkill(SkillDefOf.Social).levelInt)*400;
		public override List<ResultOption> ResultOptions {
			get{
				List<ResultOption> outy = base.ResultOptions;
				outy[0].BaseAmount = totalWealth >= sellCapacity ? sellCapacity : totalWealth;
				return outy;

			}


		}
		public float NegotiatorBonus => CapablePawns.Max(p => p.TradePriceImprovementOffsetForPlayer);
		public int SellValue(Thing t) => (int)( t.stackCount*TradeUtility.GetPricePlayerSell(t,1,1,NegotiatorBonus,0,0,0));

		public override string ProductionString(){
			string outy = base.ProductionString();
			outy += "\nCan sell up to " + sellCapacity.ToString() + " silver of goods";
			return outy;
		}

	}



}

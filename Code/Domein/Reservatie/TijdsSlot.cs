﻿using System;

namespace Domein {

	public class TijdsSlot {
		public DateTime StartTijd { get; }
		public DateTime EindTijd { get; }
		public static double SlotTijdUur { get; set; } = -1;

		private const int _tijdsZone = 2;

		public TijdsSlot(DateTime startTijd) {
			ControlleerStartTijd(startTijd);
			StartTijd = startTijd.ToUniversalTime().AddHours(_tijdsZone);
			EindTijd = StartTijd.AddHours(SlotTijdUur);
		}

		private void ControlleerStartTijd(DateTime startTijd) {
			if (SlotTijdUur == -1) throw new TijdsSlotException("Het SlotTijdUur is nog niet correct ingesteld.");
			if (startTijd.TimeOfDay < DateTime.Now.TimeOfDay) throw new TijdsSlotException($"Het TijdsSlot: {startTijd.ToString("f")} mag niet in het verleden zijn.");
		}
	}
}
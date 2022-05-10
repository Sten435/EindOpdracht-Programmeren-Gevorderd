using System;

namespace Domein {

	public class TijdsSlot {
		public DateTime StartTijd { get; }
		public DateTime EindTijd { get; }
		public static double SlotTijdUur { get; set; } = -1;
		public static int LowerBoundUurReservatie { get; set; } = -1;
		public static int UpperBoundUurReservatie { get; set; } = -1;

		public const int TijdsZone = 2;

		public TijdsSlot(DateTime startTijd) {
			ControlleerTijd(startTijd);
			StartTijd = startTijd.ToUniversalTime().AddHours(TijdsZone);
			EindTijd = StartTijd.AddHours(SlotTijdUur);
		}

		public TijdsSlot(DateTime startTijd, DateTime eindTijd) {
			StartTijd = startTijd;
			EindTijd = eindTijd;
		}

		private void ControlleerTijd(DateTime startTijd) {
			if (SlotTijdUur == -1) throw new ConfigException("(Config) Het SlotTijdUur is nog niet correct ingesteld in de DB.");
			if (LowerBoundUurReservatie == -1) throw new ConfigException("(Config) Het LowerBoundUurReservatie is nog niet correct ingesteld in de DB.");
			if (UpperBoundUurReservatie == -1) throw new ConfigException("(Config) Het UpperBoundUurReservatie is nog niet correct ingesteld in de DB.");
			if (startTijd < DateTime.Now) throw new TijdsSlotException($"Het TijdsSlot: {startTijd.ToString("f")} mag niet in het verleden zijn.");
		}
	}
}
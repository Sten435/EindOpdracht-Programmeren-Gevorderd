using System.Collections.Generic;

namespace Domein {

	public class Reservatie {
		public int ReservatieNummer { get; set; }
		public Klant Klant { get; }
		public TijdsSlot TijdsSlot { get; }
		public Toestel Toestel { get; }

		private UniekeCode uniekeCode = UniekeCode.Instance;

		public Reservatie(Klant klant, TijdsSlot tijdsSlot, Toestel toestel) {
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
			ReservatieNummer = uniekeCode.GenereerRandomCode();
		}

		public override string ToString() {
			return $" Nr: {ReservatieNummer} - {Toestel.ToestelType} - Datum: {TijdsSlot.StartTijd:d} - Tijdslot: {TijdsSlot.StartTijd:t}/uur -> {TijdsSlot.EindTijd:t}/uur ";
		}
	}
}
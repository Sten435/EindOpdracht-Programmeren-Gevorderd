using System.Collections.Generic;

namespace Domein {

	public class Reservatie {
		public int ReservatieNummer { get; }
		public Klant Klant { get; }
		public List<TijdsSlot> TijdsSlot { get; }
		public Toestel Toestel { get; }

		public Reservatie(int reservatieNummer, Klant klant, List<TijdsSlot> tijdsSlot, Toestel toestel) {
			ReservatieNummer = reservatieNummer;
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
		}
	}
}
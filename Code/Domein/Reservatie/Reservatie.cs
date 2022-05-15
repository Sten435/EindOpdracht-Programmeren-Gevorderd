using System;
using System.Collections.Generic;

namespace Domein {

	public class Reservatie {
		public int? ReservatieNummer { get; set; }
		public Klant Klant { get; }
		public TijdsSlot TijdsSlot { get; }
		public Toestel Toestel { get; }

		public Reservatie(int? reservatieNummer, Klant klant, TijdsSlot tijdsSlot, Toestel toestel) {
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
			ReservatieNummer = reservatieNummer;
		}

		public override string ToString() => $"{TijdsSlot.StartTijd.ToString("dd/MM/yyyy")} \t\t {TijdsSlot.StartTijd:HH:mm} -> {TijdsSlot.EindTijd:HH:mm} \t\t {Toestel.ToestelType}";

		public override bool Equals(object obj) {
			return obj is Reservatie reservatie &&
				   ReservatieNummer == reservatie.ReservatieNummer &&
				   EqualityComparer<Klant>.Default.Equals(Klant, reservatie.Klant) &&
				   EqualityComparer<TijdsSlot>.Default.Equals(TijdsSlot, reservatie.TijdsSlot) &&
				   EqualityComparer<Toestel>.Default.Equals(Toestel, reservatie.Toestel);
		}

		public override int GetHashCode() {
			return HashCode.Combine(ReservatieNummer, Klant, TijdsSlot, Toestel);
		}
	}
}
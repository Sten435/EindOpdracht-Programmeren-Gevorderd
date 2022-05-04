using System;
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
			int reservatieNummer = uniekeCode.GenereerRandomCode();
			ControlleerReservatieNummer(reservatieNummer);
			ReservatieNummer = reservatieNummer;
		}

		private void ControlleerReservatieNummer(int reservatienummer) {
			if (reservatienummer < 0) throw new ReservatieNummerException("ReservatieNummer moet groter dan 0 zijn.");
		}

		public override string ToString() => $" Nr: {ReservatieNummer} - {Toestel.ToestelType} - Klant: {Klant.Voornaam} {Klant.Achternaam} - Datum: {TijdsSlot.StartTijd:d} - Tijdslot: {TijdsSlot.StartTijd:t}/uur -> {TijdsSlot.EindTijd:t}/uur ";

		public override bool Equals(object obj) {
			return obj is Reservatie reservatie &&
				   ReservatieNummer == reservatie.ReservatieNummer &&
				   EqualityComparer<Klant>.Default.Equals(Klant, reservatie.Klant) &&
				   EqualityComparer<TijdsSlot>.Default.Equals(TijdsSlot, reservatie.TijdsSlot) &&
				   EqualityComparer<Toestel>.Default.Equals(Toestel, reservatie.Toestel) &&
				   EqualityComparer<UniekeCode>.Default.Equals(uniekeCode, reservatie.uniekeCode);
		}

		public override int GetHashCode() {
			return HashCode.Combine(ReservatieNummer, Klant, TijdsSlot, Toestel, uniekeCode);
		}
	}
}
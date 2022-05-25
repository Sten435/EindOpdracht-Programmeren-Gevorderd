using System;
using System.Collections.Generic;

namespace Domein {

	public class Reservatie {
		public int? ReservatieNummer { get; set; }
		public Klant Klant { get; }
		public TijdsSlot TijdsSlot { get; }
		public Toestel Toestel { get; }
		public static int AantalDagenInToekomstReserveren { get; set; } = -1;

		public Reservatie(int? reservatieNummer, Klant klant, TijdsSlot tijdsSlot, Toestel toestel) {
			if (AantalDagenInToekomstReserveren == -1) throw new ConfigException("DagenInToekomst niet ingesteld in DB");
			Klant = klant;
			TijdsSlot = tijdsSlot;
			Toestel = toestel;
			ReservatieNummer = reservatieNummer;
		}

		public string ToString(bool parsed = false) {
			if (!parsed) {
				return $"Id: {ReservatieNummer} Dag: {TijdsSlot.StartTijd.ToString("dd/MM/yyyy")} StartSlot: {TijdsSlot.StartTijd:HH:mm} EindSlot: {TijdsSlot.EindTijd:HH:mm} Toestel: {Toestel.ToestelType}";
			}
			return $"{ReservatieNummer}@|@{TijdsSlot.StartTijd.ToString("dd/MM/yyyy")}@|@{TijdsSlot.StartTijd:HH:mm}@|@{TijdsSlot.EindTijd:HH:mm}@|@{Toestel.ToestelType}";
		}

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
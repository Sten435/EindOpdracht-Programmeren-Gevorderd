using System;
using System.Collections.Generic;

namespace Domein {

	public interface IReservatieRepository {

		HashSet<Reservatie> GeefAlleReservaties(bool vandaagPlusToekomst = false, bool alleenVandaag = false, int klantenNummer = -1);

		int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string ToestelNaam);

		List<DateTime> GeefReservatiesPerToestel(string naam, DateTime? dag = null);

		int VoegReservatieToe(Reservatie reservatie);

		void VerwijderReservatie(int reservatieId);
	}
}
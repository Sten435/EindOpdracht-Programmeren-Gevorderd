using System;
using System.Collections.Generic;

namespace Domein {

	public interface IReservatieRepository {

		List<Reservatie> GeefAlleReservaties(bool vandaagPlusToekomst = false, bool alleenVandaag = false, int klantenNummer = -1);

		int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string ToestelNaam);

		List<Reservatie> GeefReservatiesPerToestel(string naam, DateTime? dag = null);

		int VoegReservatieToe(Reservatie reservatie);

		void VerwijderReservatie(int reservatieId);
	}
}
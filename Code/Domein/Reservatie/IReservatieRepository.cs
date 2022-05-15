using System;
using System.Collections.Generic;

namespace Domein {

	public interface IReservatieRepository {

		List<Reservatie> GeefAlleReservaties(bool metVerwijderedeToestellen = false, bool vanafVandaag = false);

		int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string ToestelNaam);

		void VoegReservatieToe(Reservatie reservatie);

		void VerwijderReservatie(Reservatie reservatie);
	}
}
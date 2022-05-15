using Domein;
using System;
using System.Collections.Generic;

namespace Persistentie {

	public class ReservatieRepository : IReservatieRepository {

		public List<Reservatie> GeefAlleReservaties(bool metVerwijderedeToestellen = false, bool vanafVandaag = false) => ReservatieMapper.GeefAlleReservaties(metVerwijderedeToestellen, vanafVandaag);

		public int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string toestelNaam) => ReservatieMapper.GeefBeschikbaarToestelOpTijdsSlot(dag, toestelNaam);

		public void VerwijderReservatie(Reservatie reservatie) => ReservatieMapper.VerwijderReservatie(reservatie);

		public void VoegReservatieToe(Reservatie reservatie) => ReservatieMapper.VoegReservatieToe(reservatie);
	}
}
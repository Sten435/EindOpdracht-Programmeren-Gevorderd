using Domein;
using System;
using System.Collections.Generic;

namespace Persistentie {

	public class ReservatieRepository : IReservatieRepository {

		public List<Reservatie> GeefAlleReservaties(bool vandaagPlusToekomst = false, bool alleenVandaag = false, int klantenNummer = -1) => ReservatieMapper.GeefAlleReservaties(vandaagPlusToekomst, alleenVandaag, klantenNummer);

		public List<Reservatie> GeefReservatiesPerToestel(string naam, DateTime? dag = null) => ReservatieMapper.GeefReservatiesPerToestel(naam, dag);

		public int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string toestelNaam) => ReservatieMapper.GeefBeschikbaarToestelOpTijdsSlot(dag, toestelNaam);

		public void VerwijderReservatie(int reservatie) => ReservatieMapper.VerwijderReservatie(reservatie);

		public int VoegReservatieToe(Reservatie reservatie) => ReservatieMapper.VoegReservatieToe(reservatie);
	}
}
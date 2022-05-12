using Domein;
using System.Collections.Generic;

namespace Persistentie {

	public class ReservatieRepository : IReservatieRepository {

		public List<Reservatie> GeefAlleReservaties(bool metVerwijderedeToestellen = false) => ReservatieMapper.GeefAlleReservaties(metVerwijderedeToestellen);

		public void VerwijderReservatie(Reservatie reservatie) => ReservatieMapper.VerwijderReservatie(reservatie);

		public void VoegReservatieToe(Reservatie reservatie) => ReservatieMapper.VoegReservatieToe(reservatie);
	}
}
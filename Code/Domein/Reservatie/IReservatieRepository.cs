using System.Collections.Generic;

namespace Domein {

	public interface IReservatieRepository {

		List<Reservatie> GeefAlleReservaties(bool metVerwijderedeToestellen = false);

		void VoegReservatieToe(Reservatie reservatie);

		void VerwijderReservatie(Reservatie reservatie);
	}
}
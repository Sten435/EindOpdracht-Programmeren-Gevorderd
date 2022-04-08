using System.Collections.Generic;

namespace Domein {

	public interface IReservatieRepository {
		public List<Reservatie> Reservaties { get; }

		List<Reservatie> GeefAlleReservaties();

		void VoegReservatieToe(Reservatie reservatie);

		void VerwijderReservatie(Reservatie reservatie);
	}
}
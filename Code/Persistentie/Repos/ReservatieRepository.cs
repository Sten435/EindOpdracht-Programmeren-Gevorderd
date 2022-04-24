using Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class ReservatieRepository : IReservatieRepository {
		private ReservatieMapper _mapper = new();

		public List<Reservatie> GeefAlleReservaties() => _mapper.Reservaties;

		public void VerwijderReservatie(Reservatie reservatie) => _mapper.Reservaties.Remove(reservatie);

		public void VoegReservatieToe(Reservatie reservatie) => _mapper.Reservaties.Add(reservatie);
	}
}

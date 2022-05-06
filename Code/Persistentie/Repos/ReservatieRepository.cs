using Domein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class ReservatieRepository : IReservatieRepository {

		public List<Reservatie> GeefAlleReservaties() => ReservatieMapper.GeefAlleReservaties();

		public void VerwijderReservatie(Reservatie reservatie) => ReservatieMapper.VerwijderReservatie(reservatie);

		public void VoegReservatieToe(Reservatie reservatie) => ReservatieMapper.VoegReservatieToe(reservatie);
	}
}

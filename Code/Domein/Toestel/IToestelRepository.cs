using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {
		public List<Toestel> Toestellen { get; set; }

		List<Toestel> GeefAlleToestellen();

		void VoegToestelToe(string naam);

		void ZetToestelInHerstelling(Toestel toestel);

		void VerwijderToestel(Toestel toestel);
	}
}
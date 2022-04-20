using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {

		List<Toestel> GeefAlleToestellen();

		void VoegToestelToe(string naam);

		void ZetToestelInOfUitHerstelling(Toestel toestel);

		void VerwijderToestel(Toestel toestel);
	}
}
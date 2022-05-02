using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {

		List<Toestel> GeefAlleToestellen();

		void VoegToestelToe(string naam);

		void ZetToestelInOfUitHerstelling(long toestelId, bool nieuweHerstellingValue);

		void VerwijderToestel(Toestel toestel);

		void UpdateToestelOpId(long toestelId, string toestelNaam);
		bool GeefToestelHerstelStatusOpId(long toestelId);
	}
}
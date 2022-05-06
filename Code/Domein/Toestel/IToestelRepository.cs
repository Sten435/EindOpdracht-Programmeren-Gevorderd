using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {

		List<Toestel> GeefAlleToestellen();

		void VoegToestelToe(string naam);

		void ZetToestelInOfUitHerstelling(int toestelId, bool nieuweHerstellingValue);

		void VerwijderToestel(Toestel toestel);

		void UpdateToestelNaamOpId(int toestelId, string toestelNaam);
		bool GeefToestelHerstelStatusOpId(int toestelId);
	}
}
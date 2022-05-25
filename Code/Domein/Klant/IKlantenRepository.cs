using System.Collections.Generic;

namespace Domein {

	public interface IKlantenRepository {

		List<Klant> GeefAlleKlanten();

		Klant GeefKlant(int klantenNummer);

		void RegistreerKlant(Klant klant);

		Klant Login(string email);
	}
}
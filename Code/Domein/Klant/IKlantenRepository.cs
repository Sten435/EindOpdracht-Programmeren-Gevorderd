using System.Collections.Generic;

namespace Domein {

	public interface IKlantenRepository {
		List<Klant> Klanten { get; }

		List<Klant> GeefAlleKlanten();

		void RegistreerKlant(Klant klant);

		Klant Login(string email);

		void VerwijderKlant(Klant klant);
	}
}
using Domein;
using System.Collections.Generic;
using System.Linq;

namespace Persistentie {

	public class KlantenRepository : IKlantenRepository {

		public List<Klant> GeefAlleKlanten() => KlantenMapper.GeefAlleKlanten();

		public Klant Login(string Txt) {
			Txt = Txt.Trim().ToLower();
			if (string.IsNullOrEmpty(Txt)) throw new EmailExpection("Email of Klantennummer mag niet leeg zijn.");
			if (Txt.Contains("@")) {
				try {
					var addr = new System.Net.Mail.MailAddress(Txt);
					if (addr.Address != Txt) throw new EmailExpection("Email of Klantennummer is niet toegelaten.");
				} catch {
					throw new EmailExpection("Email of Klantennummer is niet toegelaten.");
				}
			}

			Klant Klant = GeefAlleKlanten().FirstOrDefault(_klant => _klant.Email.ToLower() == Txt || _klant.KlantenNummer.ToString() == Txt.ToString());
			if (Klant == null) throw new LoginException("Email of Klantennummer is niet gevonden.");
			return Klant;
		}

		public void RegistreerKlant(Klant klant) => KlantenMapper.VoegKlantToe(klant);
	}
}
using Domein;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class KlantenRepository : IKlantenRepository {
		public List<Klant> GeefAlleKlanten() => KlantenMapper.GeefAlleKlanten();

		public Klant Login(string email) {
			email = email.Trim().ToLower();
			if (string.IsNullOrEmpty(email)) throw new EmailExpection("Email mag niet leeg zijn.");
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				if (addr.Address != email) throw new EmailExpection("Email is niet toegelaten.");
			} catch {
				throw new EmailExpection("Email is niet toegelaten.");
			}

			Klant Klant = GeefAlleKlanten().FirstOrDefault(_klant => _klant.Email.ToLower() == email);
			if (Klant == null) throw new LoginException("Email is niet gevonden.");
			return Klant;
		}

		public void RegistreerKlant(Klant klant) => KlantenMapper.VoegKlantToe(klant);
	}
}

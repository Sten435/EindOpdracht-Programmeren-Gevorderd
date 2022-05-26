using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domein {
	public interface IControll {
		public HashSet<string> GeefKlantReservaties(bool parsed = false);
		public string VoegReservatieToe(DateTime tijdsSlotDatum, int toestelId);
		public (List<int>, bool) GeefBeschikbareReservatieUren(DateTime dag, string naam);
		public int? GeefEenVrijToestelIdOpNaam(string toestelNaam, DateTime tijdsSlot);
		public List<string> GeefAlleKlanten();
		public void RegistreerKlant(string voornaam, string achternaam, string email, DateTime geboorteDatum, List<string> interesses, string typeKlant, string straat, string plaats, string huisNummer, int postCode);
		public void VerwijderReservatie(int reservatieNummer);

		public void Login(string email);
		public void Logout();

		public List<string> GeefBeschikbareToestellen();
		public List<string> GeefAlleToestellen(bool metVerwijderde = false);
		public List<string> GeefToestellenZonderReservatie();
		public int VoegNieuwToestelToe(string naam);
		public void VerwijderToestelOpId(int toestelId);
		public void ZetToestelInOfUitHerstelling(int toestelId);
	}
}

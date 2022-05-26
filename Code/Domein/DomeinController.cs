using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domein {

	public class DomeinController : IControll {
		private readonly FitnessController fitnessController;

		public bool LoggedIn { get => fitnessController.LoggedIn; }
		public bool isAdmin { get => fitnessController.isAdmin;  }
		public string KlantOmschrijving { get => fitnessController.KlantOmschrijving; }
		public string KlantOmschrijvingParsable { get => fitnessController.KlantOmschrijvingParsable; }

		public DomeinController(IReservatieRepository reservatieRepo, IKlantenRepository klantenRepo, IToestelRepository toestselRepo, IConfigRepository configRepo) {
			fitnessController = new FitnessController(reservatieRepo, klantenRepo, toestselRepo, configRepo);
		}

		#region Reservatie

		public HashSet<string> GeefKlantReservaties(bool parsed = false) => fitnessController.GeefKlantReservaties(parsed);

		public void UpdateklantReservaties() => fitnessController.UpdateklantReservaties();

		public string VoegReservatieToe(DateTime tijdsSlotDatum, int toestelId) => fitnessController.VoegReservatieToe(tijdsSlotDatum, toestelId);

		public void VerwijderReservatie(int reservatieNummer) => fitnessController.VerwijderReservatie(reservatieNummer);

		#endregion Reservatie

		#region TijdsSlot

		public (List<int>, bool) GeefBeschikbareReservatieUren(DateTime dag, string naam) => fitnessController.GeefBeschikbareReservatieUren(dag, naam);

		public int? GeefEenVrijToestelIdOpNaam(string toestelNaam, DateTime tijdsSlot) => fitnessController.GeefEenVrijToestelIdOpNaam(toestelNaam, tijdsSlot);

		#endregion TijdsSlot

		#region Klant

		public List<string> GeefAlleKlanten() {
			return fitnessController.GeefAlleKlanten();
		}

		public void RegistreerKlant(string voornaam, string achternaam, string email, DateTime geboorteDatum, List<string> interesses, string typeKlant, string straat, string plaats, string huisNummer, int postCode) => fitnessController.RegistreerKlant(voornaam, achternaam, email, geboorteDatum, interesses, typeKlant, straat, plaats, huisNummer, postCode);

		public void Login(string email) => fitnessController.Login(email);

		public void Logout() => fitnessController.Logout();

		#endregion Klant

		#region Toestel

		public List<string> GeefBeschikbareToestellen() => fitnessController.GeefBeschikbareToestellen();

		public List<string> GeefToestellenZonderReservatie() => fitnessController.GeefToestellenZonderReservatie();

		public List<string> GeefAlleToestellen(bool metVerwijderde = false) => fitnessController.GeefAlleToestellen(metVerwijderde);

		public int VoegNieuwToestelToe(string naam) => fitnessController.VoegNieuwToestelToe(naam);

		public List<DateTime> GeefBeschikbareDagen() => fitnessController.GeefBeschikbareDagen();

		public void VerwijderToestelOpId(int toestelId) => fitnessController.VerwijderToestelOpId(toestelId);

		public void ZetToestelInOfUitHerstelling(int toestelId) => fitnessController.ZetToestelInOfUitHerstelling(toestelId);

		#endregion Toestel
	}
}
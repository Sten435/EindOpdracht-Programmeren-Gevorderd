﻿using Domein;
using System.Collections.Generic;

namespace Persistentie {

	public class ToestellenRepository : IToestelRepository {

		public List<Toestel> GeefAlleToestellen(bool metVerwijderdeToestellen = false) => ToestellenMapper.GeefToestellen(metVerwijderdeToestellen);

		public Toestel GeefToestel(int toestelNummer) => ToestellenMapper.GeefToestel(toestelNummer);

		public List<Toestel> GeefToestellenZonderReservatie() => ToestellenMapper.GeefToestellenZonderReservatie();

		public int VoegToestelToe(string naam) => ToestellenMapper.VoegToestelToe(new(naam[0].ToString().ToUpper() + naam.ToLower().Substring(1)));

		public void VerwijderToestel(int toestelId) => ToestellenMapper.VerwijderToestel(toestelId);

		public void ZetToestelInOfUitHerstelling(int toestelId) {
			Toestel huidigToestel = ToestellenMapper.GeefToestelOpId(toestelId);

			if (huidigToestel != null)
				ToestellenMapper.UpdateToestelInHerstelling(huidigToestel);
			else throw new ToestelException("Toestel niet gevonden");
		}

		public List<Toestel> GeefAlleBeschikbareToestellenOpNaam(string naam) => ToestellenMapper.GeefAlleBeschikbareToestellenOpNaam(naam);
	}
}
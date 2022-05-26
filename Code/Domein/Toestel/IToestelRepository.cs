﻿using System;
using System.Collections.Generic;

namespace Domein {

	public interface IToestelRepository {

		List<Toestel> GeefAlleToestellen(bool metVerwijderdeToestellen = false);

		Toestel GeefToestel(int toestelNummer);

		List<Toestel> GeefToestellenZonderReservatie();

		int VoegToestelToe(string naam);

		void ZetToestelInOfUitHerstelling(int toestelId);

		void VerwijderToestel(int toestelId);

		List<Toestel> GeefAlleBeschikbareToestellenOpNaam(string naam);
	}
}
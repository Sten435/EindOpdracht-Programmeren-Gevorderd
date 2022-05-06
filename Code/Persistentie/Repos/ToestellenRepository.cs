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
	public class ToestellenRepository : IToestelRepository {

		public List<Toestel> GeefAlleToestellen() => ToestellenMapper.GeefToestellen();

		public void VoegToestelToe(string naam) => ToestellenMapper.VoegToestelToe(new(naam[0].ToString().ToUpper() + naam.ToLower().Substring(1)));

		public void VerwijderToestel(Toestel toestel) => ToestellenMapper.VerwijderToestel(toestel);

		public void UpdateToestelNaamOpId(int toestelId, string toestelNaam) {
			Toestel huidigToestel = GeefAlleToestellen().Find(t => t.IdentificatieCode == toestelId);

			if (huidigToestel != null) {
				huidigToestel.ToestelType = toestelNaam;

				ToestellenMapper.UpdateToestelNaam(huidigToestel);
			}
		}

		public void ZetToestelInOfUitHerstelling(int toestelId, bool nieuweHerstellingValue) {
			Toestel huidigToestel = GeefAlleToestellen().Find(t => t.IdentificatieCode == toestelId);

			if (huidigToestel != null) {
				huidigToestel.InHerstelling = nieuweHerstellingValue;

				ToestellenMapper.UpdateToestelInHerstelling(huidigToestel);
			}
		}

		public bool GeefToestelHerstelStatusOpId(int toestelId) => GeefAlleToestellen().Find(t => t.IdentificatieCode == toestelId).InHerstelling;
	}
}

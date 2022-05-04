using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistentie {
	public class ConfigMapper {
		public void LoadConfig() {
			//Laad Config Table en zet alle statiche propperties die aanpasbaar zijn door de beheerder gelijk.
			//TijdsSlot.SlotTijdUur = SELECT SlotTijdUur FROM Config;
			//Toestel.StandaardInherstelling = SELECT StandaardInherstelling FROM Config;

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT SlotTijdUur, StandaardInherstelling FROM Config;", connection);
				using SqlDataReader dataFromQuery = command.ExecuteReader();

				if (dataFromQuery.HasRows) {
					while (dataFromQuery.Read()) {
						double SlotTijdUur = (Int64)dataFromQuery["SlotTijdUur"];
						bool StandaardInherstelling = (bool)dataFromQuery["StandaardInherstelling"];
						TijdsSlot.SlotTijdUur = SlotTijdUur;
						Toestel.StandaardInherstelling = StandaardInherstelling;
					}
				} else throw new NoDataInDbException("Geen data in de databank.");
			} catch (Exception err) {
				throw new SelectFromDbException($"Fout bij select config data uit DB: {err.Message}");
			}
		}
	}
}

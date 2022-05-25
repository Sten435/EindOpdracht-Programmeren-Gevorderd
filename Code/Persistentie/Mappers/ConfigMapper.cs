using Domein;
using System;
using System.Data.SqlClient;

namespace Persistentie {

	public class ConfigMapper {

		public void LoadConfig() {
			//Laad Config Table en zet alle statiche propperties die aanpasbaar zijn door de beheerder gelijk.
			//TijdsSlot.SlotTijdUur = SELECT SlotTijdUur FROM Config;
			//Toestel.StandaardInherstelling = SELECT StandaardInherstelling FROM Config;

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("SELECT * FROM Config;", connection);
				using SqlDataReader dataFromQuery = command.ExecuteReader();

				if (dataFromQuery.HasRows) {
					while (dataFromQuery.Read()) {
						double slotTijdUur = (long)dataFromQuery["SlotTijdUur"];
						bool standaardInherstelling = (bool)dataFromQuery["StandaardInherstelling"];
						int lowerBoundUurReservatie = (int)dataFromQuery["LowerBoundUurReservatie"];
						int upperBoundUurReservatie = (int)dataFromQuery["UpperBoundUurReservatie"];
						int aantalDagenInToekomstReserveren = (int)dataFromQuery["AantalDagenInToekomstReserveren"];

						TijdsSlot.SlotTijdUur = slotTijdUur;
						Toestel.StandaardInherstelling = standaardInherstelling;
						TijdsSlot.LowerBoundUurReservatie = lowerBoundUurReservatie;
						TijdsSlot.UpperBoundUurReservatie = upperBoundUurReservatie;
						Reservatie.AantalDagenInToekomstReserveren = aantalDagenInToekomstReserveren;
					}
				} else throw new ConfigException("(Config) Er bevind zich geen config data in de databank.");
			} catch (SqlException) {
				throw new ConfigException("(Config) Fout met query naar config Db.");
			} catch (Exception) {
				throw new ConfigException("(Config) Fout in config Db.");
			}
		}
	}
}
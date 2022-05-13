using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Persistentie {

	public static class ToestellenMapper {

		public static List<Toestel> GeefToestellen(bool metVerwijderedeToestellen = false) {
			List<Toestel> toestellen = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command;

				if (!metVerwijderedeToestellen)
					command = new("SELECT * FROM Toestellen WHERE Verwijderd = 0 ORDER BY IdentificatieCode ASC;", connection);
				else
					command = new("SELECT * FROM Toestellen WHERE Verwijderd = 1 ORDER BY IdentificatieCode ASC;", connection);

				using SqlDataReader dataReader = command.ExecuteReader();

				if (dataReader.HasRows) {
					while (dataReader.Read()) {
						int identificatieCode = (int)dataReader["IdentificatieCode"];
						string toestelType = (string)dataReader["ToestelType"];
						bool inHerstelling = (bool)dataReader["InHerstelling"];
						bool verwijderd = (bool)dataReader["Verwijderd"];

						Toestel toestel = new(identificatieCode, toestelType, inHerstelling);

						toestellen.Add(toestel);
					}
				}
			} catch (SqlException) {
				throw new ToestelException("(Select) Fout met query naar toestellen Db.");
			} catch (Exception) {
				throw new ToestelException("(Select) Fout in toestellen Db.");
			}

			return toestellen;
		}

		public static void VoegToestelToe(string naam) {
			Toestel.ControlleerToestelNaam(naam);
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("INSERT INTO Toestellen (ToestelType, InHerstelling) VALUES (@ToestelType, @InHerstelling);", connection);

				command.Parameters.AddWithValue("@ToestelType", naam.Trim());
				command.Parameters.AddWithValue("@InHerstelling", Toestel.StandaardInherstelling);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ToestelException("(Insert) Fout met query naar toestellen Db.");
			} catch (Exception) {
				throw new ToestelException("(Insert) Fout in toestellen Db.");
			}
		}

		public static void VerwijderToestel(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET Verwijderd = 1 WHERE ToestelType = @ToestelType AND IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);
				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ToestelException("(Delete) Fout met query naar toestellen Db.");
			} catch (Exception) {
				throw new ToestelException("(Delete) Fout in toestellen Db.");
			}
		}

		public static void UpdateToestelNaam(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET ToestelType = @ToestelType WHERE IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);
				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ToestelException("(Update Naam) Fout met query naar toestellen Db.");
			} catch (Exception) {
				throw new ToestelException("(Update Naam) Fout in toestellen Db.");
			}
		}

		public static void UpdateToestelInHerstelling(Toestel huidigToestel) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("UPDATE Toestellen SET InHerstelling = @InHerstelling WHERE ToestelType = @ToestelType AND IdentificatieCode = @IdentificatieCode;", connection);

				command.Parameters.AddWithValue("@IdentificatieCode", huidigToestel.IdentificatieCode);
				command.Parameters.AddWithValue("@ToestelType", huidigToestel.ToestelType);
				command.Parameters.AddWithValue("@InHerstelling", huidigToestel.InHerstelling);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ToestelException("(Update Herstel) Fout met query naar toestellen Db.");
			} catch (Exception) {
				throw new ToestelException("(Update Herstel) Fout in toestellen Db.");
			}
		}
	}
}
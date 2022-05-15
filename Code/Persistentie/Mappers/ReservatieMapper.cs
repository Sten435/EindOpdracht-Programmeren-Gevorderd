using Domein;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Persistentie {

	public static class ReservatieMapper {

		public static List<Reservatie> GeefAlleReservaties(bool metVerwijderedeToestellen = false, bool vanafVandaag = false) {
			List<Reservatie> reservaties = new();

			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command;

				if (vanafVandaag)
					command = new("SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer WHERE DAY(StartTijd) >= DAY(GETDATE()) AND MONTH(StartTijd) >= MONTH(GETDATE()) AND YEAR(StartTijd) >= YEAR(GETDATE()) ORDER BY ReservatieNummer ASC;", connection);
				else
					command = new("SELECT * FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer ORDER BY ReservatieNummer ASC;", connection);

				Reservatie reservatie;

				using SqlDataReader reader = command.ExecuteReader();
				if (reader.HasRows) {
					List<Klant> klanten = KlantenMapper.GeefAlleKlanten();
					List<Toestel> toestellen = ToestellenMapper.GeefToestellen(metVerwijderedeToestellen);

					while (reader.Read()) {
						int reservatieNummer = (int)reader["ReservatieNummer"];
						int klantenNummer = (int)reader["Klant_KlantenNummer"];
						int toestelNummer = (int)reader["Toestel_IdentificatieCode"];
						DateTime startTijd = (DateTime)reader["StartTijd"];
						DateTime eindTijd = (DateTime)reader["EindTijd"];

						Klant klant = klanten.Find(klant => klant.KlantenNummer == klantenNummer);
						Toestel toestel = toestellen.Find(toestel => toestel.IdentificatieCode == toestelNummer);
						if (toestel != null) {
							TijdsSlot tijdsSlot = new(startTijd, eindTijd);

							reservatie = new(reservatieNummer, klant, tijdsSlot, toestel);
							reservaties.Add(reservatie);
						}
					}
				}
			} catch (SqlException) {
				throw new ReservatieException("(Select) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Select) Fout in reservatie Db.");
			}
			return reservaties;
		}

		public static int? GeefBeschikbaarToestelOpTijdsSlot(DateTime dag, string toestelNaam) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new($"SELECT IdentificatieCode FROM Toestellen WHERE IdentificatieCode NOT IN (SELECT DISTINCT Toestel_IdentificatieCode FROM Reservaties r join TijdSloten ts on ts.Reservatie_ReservatieNummer = ReservatieNummer " +
					$"WHERE StartTijd = CAST(@dag AS datetime) " +
					$"OR(StartTijd = CAST(@dag AS datetime) AND DAY(StartTijd) >= DAY(GETDATE()) " +
					$"AND MONTH(StartTijd) >= MONTH(GETDATE()) " +
					$"AND YEAR(StartTijd) >= YEAR(GETDATE()))) AND ToestelType = @toestelNaam;", connection);

				command.Parameters.AddWithValue("@dag", dag.ToString("yyyy-MM-dd HH:mm:ss"));
				command.Parameters.AddWithValue("@toestelNaam", toestelNaam);

				using SqlDataReader reader = command.ExecuteReader();

				if (reader.HasRows) {
					while (reader.Read()) {
						return (int)reader["IdentificatieCode"];
					}
				}
			} catch (SqlException) {
				throw new ReservatieException("(Select) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Select) Fout in reservatie Db.");
			}
			return null;
		}

		public static void VerwijderReservatie(Reservatie reservatie) {
			try {
				using SqlConnection connection = new(ConfigRepository.ConnectionString);
				connection.Open();

				SqlCommand command = new("DELETE FROM Reservaties WHERE ReservatieNummer = @ReservatieNummer", connection);

				command.Parameters.AddWithValue("@ReservatieNummer", reservatie.ReservatieNummer);

				command.ExecuteNonQuery();
			} catch (SqlException) {
				throw new ReservatieException("(Delete) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Delete) Fout in reservatie Db.");
			}
		}

		public static void VoegReservatieToe(Reservatie reservatie) {
			using SqlConnection connection = new(ConfigRepository.ConnectionString);
			connection.Open();

			SqlTransaction sqlTransaction = connection.BeginTransaction();
			try {
				SqlCommand command = new("INSERT INTO Reservaties (Klant_KlantenNummer, Toestel_IdentificatieCode) VALUES (@Klant_KlantenNummer, @Toestel_IdentificatieCode)SELECT SCOPE_IDENTITY()", connection, sqlTransaction);

				command.Parameters.AddWithValue("@Klant_KlantenNummer", reservatie.Klant.KlantenNummer);
				command.Parameters.AddWithValue("@Toestel_IdentificatieCode", reservatie.Toestel.IdentificatieCode);

				int reservatieNummer = (int)(decimal)command.ExecuteScalar();
				reservatie.ReservatieNummer = reservatieNummer;

				command = new("INSERT INTO TijdSloten (StartTijd, EindTijd, Reservatie_ReservatieNummer) VALUES (@StartTijd, @EindTijd, @Reservatie_ReservatieNummer);", connection, sqlTransaction);

				command.Parameters.AddWithValue("@StartTijd", reservatie.TijdsSlot.StartTijd);
				command.Parameters.AddWithValue("@EindTijd", reservatie.TijdsSlot.EindTijd);
				command.Parameters.AddWithValue("@Reservatie_ReservatieNummer", reservatieNummer);

				command.ExecuteNonQuery();

				sqlTransaction.Commit();
			} catch (SqlException) {
				throw new ReservatieException("(Insert) Fout met query naar reservatie Db.");
			} catch (Exception) {
				throw new ReservatieException("(Insert) Fout in reservatie Db.");
			}
		}
	}
}
using Domein;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUI {
	public class InitializeKlant {
		private static IKlantenRepository _klantenRepository;
		private static IReservatieRepository _reservatieRepository;
		private static IToestelRepository _toestelRepository;
		private static IConfigRepository _configRepository;

		private static DomeinController _domeinController;

		static void Main(string[] args) {
			do {
				try {
					_klantenRepository = new KlantenRepository();
					_reservatieRepository = new ReservatieRepository();
					_toestelRepository = new ToestellenRepository();
					_configRepository = new ConfigRepository();

					_domeinController = new(_reservatieRepository, _klantenRepository, _toestelRepository, _configRepository);

					new KlantProgram(_domeinController).Start();

				} catch (NullReferenceException error) {
					Utility.Logger.Error($"Onbekende fout <001> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (ArgumentOutOfRangeException error) {
					Utility.Logger.Error($"Onbekende fout <002> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (ArgumentException error) {
					Utility.Logger.Error($"Onbekende fout <003> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (FormatException error) {
					Utility.Logger.Error($"Onbekende fout <004> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (IndexOutOfRangeException error) {
					Utility.Logger.Error($"Onbekende fout <005> (Contacteer een beheerder): {error}", clearConsole: true);
				} catch (Exception error) {
					Utility.Logger.Error($"Onbekende fout <006> (Contacteer een beheerder): {error}", clearConsole: true);
				}
			} while (true);
		}
	}
}

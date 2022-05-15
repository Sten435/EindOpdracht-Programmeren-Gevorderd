using Domein;
using Persistentie;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		private static IKlantenRepository _klantenRepository;
		private static IReservatieRepository _reservatieRepository;
		private static IToestelRepository _toestelRepository;
		private static IConfigRepository _configRepository;

		private DomeinController _domeinController;
		private void StartApplicatie(object sender, StartupEventArgs e) {
			_klantenRepository = new KlantenRepository();
			_reservatieRepository = new ReservatieRepository();
			_toestelRepository = new ToestellenRepository();
			_configRepository = new ConfigRepository();

			_domeinController = new(_reservatieRepository, _klantenRepository, _toestelRepository, _configRepository);
			LoginWindow loginWindow = new(_domeinController);
			loginWindow.Title = "Login";
			loginWindow.Show();
		}

		private void LaatsteExceptieKans(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			e.Handled = true;
			MessageBox.Show(e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}

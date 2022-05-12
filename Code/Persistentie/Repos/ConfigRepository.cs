using Domein;

namespace Persistentie {

	public class ConfigRepository : IConfigRepository {
		public const string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Fitness;Integrated Security=True;TrustServerCertificate=True";
		private readonly ConfigMapper _mapper = new();

		public void LoadConfig() => _mapper.LoadConfig();
	}
}